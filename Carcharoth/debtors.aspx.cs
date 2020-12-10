using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Threading;

namespace Carcharoth
{
    public partial class debtors : System.Web.UI.Page
    {
        protected int index_dynamics_columns=7;
        private SqlConnection conn = new SqlConnection(index.CM);
        protected bool isAllow = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            SortGrid(false);
            if (!IsPostBack)
            {
                Добавить_Ответственное_Лицо();
            }
            Обновить_Статистику();
            if (Request.IsAuthenticated)
            {
                if (Session["Level"] == null)
                    Response.Redirect("/catalog.aspx");
                else
                {
                    if ((int)Session["Level"] >= 1 &&
                        (((string)Session["CurrentProject"]).Contains("Взносы")
                        || ((string)Session["CurrentProject"]).Contains("Все проекты")))
                    {
                        PanelAddDebts.Enabled = true;
                        PanelAddDebts.Visible = true;
                        isAllow = true;

                        Обновить_Список_Сотрудников();
                        Обновить_Кнопки_Удаления_Столбцов();
                        //Обновить_Таблицу_();
                    }
                    else isAllow = false;
                }
            }
            else
            {
                PanelAddDebts.Enabled = false;
                PanelAddDebts.Visible = false;
                isAllow = false;
            }
            Заполнить_Стили_Таблицы();
        }
        protected int GetIDBailee()
        {
            int ID = 0;
            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
            using (SqlCommand sqlCommand = new SqlCommand("SELECT ID FROM Debtors WHERE ФИО=@ФИО", sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@ФИО", (string)Session["FIO"]);
                sqlConnection.Open();
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ID= (int)reader["ID"];
                    }
                }
                sqlConnection.Close();
            }
            return ID;
        }
        protected bool AllowBailee(string column)
        {
            if (!isAllow) return false;
            bool allowcolumn = false;
            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
            using (SqlCommand sqlCommand = new SqlCommand("SELECT Bailee FROM Debtors WHERE ФИО=@ФИО", sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@ФИО", (string)Session["FIO"]);
                sqlConnection.Open();
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                       var bailee = (object)reader["Bailee"]==DBNull.Value?"": (string)reader["Bailee"];
                        if (bailee.Contains(column) || ((int)Session["Level"] >= 10 && ((string)Session["CurrentProject"]).Contains("Все проекты")))
                        { 
                            allowcolumn = true;
                        }
                    }
                }
                sqlConnection.Close();
            }
            return allowcolumn;
        }
        protected void ДобавитьВзнос_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(SumDebtTB.Text))
            {
                var isdouble = Double.TryParse(SumDebtTB.Text.Replace(".",","), out double resSum);
                if (isdouble)
                {
                    string description = DescriptionDebt.Text.Trim();
                    if (!String.IsNullOrEmpty(description))
                    {
                        if (description.Contains("|") || description.Contains("{Скрыто}")) {
                            StatusDebtInfo.Text = "Описание взноса не может содержать '|' и '{Скрыто}'."; return; }
                        try
                        {
                            conn.Open();
                            string head = (isHiddenDebt.Checked == true ? "{Скрыто}" : String.Empty) + " " + description + " | " + resSum.ToString();
                            SqlCommand cmd = new SqlCommand("ALTER TABLE [Debtors] ADD \"" + head + "\" text NULL;", conn);
                            SqlCommand cmd2 = new SqlCommand("UPDATE Debtors SET [" + head + "]='0' WHERE ID>0 AND Учет=1", conn);
                            SqlCommand cmd22 = new SqlCommand("UPDATE Debtors SET [" + head + "]='Неучет' WHERE ID>0 AND Учет=0", conn);
                            string bailee = String.Empty;
                            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                            using (SqlCommand sqlCommand = new SqlCommand("SELECT Bailee FROM Debtors WHERE ID=" + GetIDBailee(), sqlConnection))
                            {
                                sqlCommand.Parameters.AddWithValue("@ФИО", (string)Session["FIO"]);
                                sqlConnection.Open();
                                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        bailee = (object)reader["Bailee"] == DBNull.Value ? "" : (string)reader["Bailee"];
                                    }
                                }
                                sqlConnection.Close();
                            }
                            bailee += ";"+ head;
                            SqlCommand cmd3 = new SqlCommand("UPDATE Debtors SET [Bailee]=@bailee WHERE ID="+ GetIDBailee(), conn);
                            cmd.ExecuteNonQuery();
                            cmd2.ExecuteNonQuery();
                            cmd22.ExecuteNonQuery();
                            cmd3.Parameters.AddWithValue("@bailee", bailee);
                            cmd3.ExecuteNonQuery();
                            conn.Close();
                            StatusDebtInfo.Text = "Колонка взносов с именем \"" + description + "\" - добавлена.";
                            ПереРассчитатьДолгВсехСотрудников();
                            GVDataDebtorsSQL.DataBind();
                            Добавить_Кнопку_Удаления_Столбцов(head);
                            Обновить_Статистику();
                            infoaboutserver.AddHistory("Debtors: добавлен взнос "+ description);
                        }
                        catch (SqlException ex)
                        {
                            //if (ex.ErrorCode == -2146232060)
                            //    StatusDebtInfo.Text = "Колонка с таким именем уже существует!";
                            //else
                                StatusDebtInfo.Text = ex.ToString();
                        }
                    }
                }
            }
            Заполнить_Стили_Таблицы();
        }
        class EmployeesClass
        {
            public int ID { get; set; }
            public string Направление { get; set; }
            public string ФИО { get; set; }
        }
        protected void Обновить_Список_Сотрудников()
        {
            List<EmployeesClass> listEmployees = new List<EmployeesClass>();
            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
            using (SqlCommand sqlCommand = new SqlCommand("SELECT ID,Direction,FIO from Users", sqlConnection))
            {
                sqlConnection.Open();
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listEmployees.Add(new EmployeesClass
                        {
                            ID = (int)reader["ID"],
                            Направление= (object)reader["Direction"]==DBNull.Value?"": (string)reader["Direction"],
                            ФИО = (string)reader["FIO"],
                        });
                    }
                }
                sqlConnection.Close();
            }
            //get debtors
            List<EmployeesClass> listDebtors = new List<EmployeesClass>();
            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
            using (SqlCommand sqlCommand = new SqlCommand("SELECT ID,Направление,ФИО from Debtors", sqlConnection))
            {
                sqlConnection.Open();
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listDebtors.Add(new EmployeesClass
                        {
                            ID = (int)reader["ID"],
                            Направление = (object)reader["Направление"]==DBNull.Value?"": (string)reader["Направление"],
                            ФИО = (string)reader["ФИО"],
                        });
                    }
                }
                sqlConnection.Close();
            }
            //delete if not exist in employees
            foreach (var d in listDebtors)
            {
                if(!listEmployees.Exists(x=>x.ID==d.ID))
                using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                using (SqlCommand sqlCommand = new SqlCommand("DELETE FROM Debtors WHERE ID=@ID", sqlConnection))
                {
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@ID", d.ID);
                    sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();
                }
            }
            //Update debtors
            foreach (var d in listDebtors)
                foreach (var e in listEmployees)
                {
                    if (e.ID == d.ID && (e.ФИО != d.ФИО || d.Направление!=e.Направление))
                    {
                        using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                        using (SqlCommand sqlCommand = new SqlCommand("UPDATE Debtors SET ФИО=@ФИО,Направление=@Направление WHERE ID=@ID", sqlConnection))
                        {
                            sqlConnection.Open();
                            sqlCommand.Parameters.AddWithValue("@ID", e.ID);
                            sqlCommand.Parameters.AddWithValue("@Направление", e.Направление);
                            sqlCommand.Parameters.AddWithValue("@ФИО", e.ФИО);
                            sqlCommand.ExecuteNonQuery();
                            sqlConnection.Close();
                        }
                    }
                }
            //add
            foreach (var e in listEmployees)
            {
                if (!listDebtors.Exists(x => x.ID == e.ID))
                {
                    using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("INSERT INTO Debtors(ID,Направление,Учет,ФИО,Долг) VALUES (@ID,@Направление,1,@ФИО,0)", sqlConnection))
                        {
                            sqlConnection.Open();
                            sqlCommand.Parameters.AddWithValue("@ID", e.ID);
                            sqlCommand.Parameters.AddWithValue("@Направление", e.Направление);
                            sqlCommand.Parameters.AddWithValue("@ФИО", e.ФИО);
                            sqlCommand.ExecuteNonQuery();
                            sqlConnection.Close();
                        }
                    }
                }
            }
        }
        protected void Добавить_Кнопку_Удаления_Столбцов(string name)
        {
            if (AllowBailee(name))
            {
                var new_link = new LinkButton();
                new_link.Text = name;
                new_link.Attributes.Add("Class", "btn btn-sm btn-outline-danger");
                new_link.Click += (q, e) =>
                {
                    using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                    using (SqlCommand sqlCommand = new SqlCommand("ALTER TABLE Debtors DROP COLUMN [" + name + "]", sqlConnection))
                    {
                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                        sqlConnection.Close();
                    }
                    ПанельУдаленияСтолбцов.Controls.Remove(((LinkButton)q));
                    ПереРассчитатьДолгВсехСотрудников();
                    GVDataDebtorsSQL.DataBind();
                    StatusDebtInfo.Text = "Колонка взносов с именем \"" + name + "\" - удалена.";
                    Заполнить_Стили_Таблицы();
                    Обновить_Статистику();
                    ///
                    conn.Open();
                    string bailee = String.Empty;
                    using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                    using (SqlCommand sqlCommand = new SqlCommand("SELECT Bailee FROM Debtors WHERE ID=" + GetIDBailee(), sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@ФИО", (string)Session["FIO"]);
                        sqlConnection.Open();
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bailee = (object)reader["Bailee"] == DBNull.Value ? "" : (string)reader["Bailee"];
                            }
                        }
                        sqlConnection.Close();
                    }
                    bailee = bailee.Replace(";"+name, String.Empty);
                    SqlCommand cmd3 = new SqlCommand("UPDATE Debtors SET [Bailee]=@bailee WHERE ID=" + GetIDBailee(), conn);
                    cmd3.Parameters.AddWithValue("@bailee", bailee);
                    cmd3.ExecuteNonQuery();
                    conn.Close();
                    infoaboutserver.AddHistory("Debtors: удален взнос " + name);
                };
                ПанельУдаленияСтолбцов.Controls.Add(new_link);
            }
        }
        protected void Обновить_Кнопки_Удаления_Столбцов()
        {
            var list_headers = new List<string>();
            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
            using (SqlCommand sqlCommand = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'Debtors' AND ORDINAL_POSITION>"+ index_dynamics_columns, sqlConnection))
            {
                sqlConnection.Open();
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list_headers.Add((string)reader["COLUMN_NAME"]);
                    }
                }
                sqlConnection.Close();
            }
            foreach (var s in list_headers)
            {
                Добавить_Кнопку_Удаления_Столбцов(s);
            }
            if (!IsPostBack)
                GVDataDebtorsSQL.DataBind();
        }
        protected string ПолучитьФИО_Bailee(string column)
        {
            string FIO = String.Empty;
            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
            using (SqlCommand sqlCommand = new SqlCommand("SELECT Bailee,ФИО FROM Debtors", sqlConnection))
            {
                sqlConnection.Open();
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var bailee = (object)reader["Bailee"] == DBNull.Value ? "" : (string)reader["Bailee"];
                        if (bailee.Contains(column))
                        {
                            FIO = (string)reader["ФИО"];
                            break;
                        }
                    }
                }
                sqlConnection.Close();
            }
            return "Добавил(а) взнос: "+FIO;
        }
        protected void Заполнить_Стили_Таблицы()
        {
            foreach (GridViewRow строка in GVDataDebtorsSQL.Rows)
            {
                bool isChecked = true;
                int ID = 0;
                //double sum = 0;
                foreach (DataControlFieldCell ячейка in строка.Cells)
                {
                    if (ячейка.ContainingField.HeaderText == "Производить учет")
                    {
                        var cb = ячейка.FindControl("УчетВзносовСотрудника") as CheckBox;
                        isChecked = cb.Checked;
                        cb.Enabled = isAllow;
                    }
                    if (ячейка.ContainingField.HeaderText == "ID")
                    {
                        ID = Convert.ToInt32(ячейка.Text);
                    }
                    double debt = 0;
                    if (ячейка.ContainingField.HeaderText == "Долг")
                    {
                        debt= Convert.ToInt32(ячейка.Text);
                        if (ячейка.Text != "0")
                        {
                            if (ячейка.Text == "-1")
                            {
                                ячейка.Controls.Add(new Image() { ImageUrl = "images/not.png" });
                            }
                            if (isChecked)
                            {
                                строка.Attributes.Add("style", "background-color:#ff00000f;text-decoration: none;");//red
                                ячейка.Attributes.Add("style", "background-color:#ff000057;color:White");//red dolg
                            }
                            else
                            {
                                строка.Attributes.Add("style", "background-color:#423f3f5c;");//gray
                                ячейка.Attributes.Add("style", "background-color:transparent;Color:White");//usually
                            }
                        }
                        else
                        {
                            if (isChecked)
                            {
                                строка.Attributes.Add("style", "background-color:#00ff000f;text-decoration: none;");//green
                                ячейка.Attributes.Add("style", "background-color:transparent;Color:White");//usually
                                if (!isAllow)
                                {
                                    ячейка.Controls.Add(new Image() { ImageUrl = "images/ok.png" });
                                }
                            }
                            else
                            {
                                строка.Attributes.Add("style", "background-color:#423f3f5c;");//gray
                                ячейка.Attributes.Add("style", "background-color:transparent;Color:White");//usually
                            }
                        }
                    }
                    //if (ячейка.Text == "0")
                    //    ячейка.Enabled = false;
                    var ColumnIndex = GetColumnIndexByHeaderText(GVDataDebtorsSQL, ячейка.ContainingField.HeaderText);
                    if (ColumnIndex > index_dynamics_columns)
                    {
                        var allowbailee = AllowBailee(ячейка.ContainingField.HeaderText);
                        ячейка.ToolTip = ПолучитьФИО_Bailee(ячейка.ContainingField.HeaderText);
                        if ((!isAllow || !allowbailee) && ячейка.Text == "Нал.")
                            ячейка.Controls.Add(new Image() { ImageUrl = "images/cash.png" });
                        else if ((!isAllow || !allowbailee) && ячейка.Text == "Безнал.")
                            ячейка.Controls.Add(new Image() { ImageUrl = "images/nocash.png" });
                        else if ((!isAllow || !allowbailee) && ячейка.Text == "Неучет")
                            ячейка.Controls.Add(new Image() { ImageUrl = "images/not.png" });
                        else if ((!isAllow || !allowbailee) && isChecked)
                            ячейка.Controls.Add(new Image() { ImageUrl = "images/decline.png" });
                        if (!allowbailee) continue;

                        var cost = Convert.ToDouble(ячейка.ContainingField.HeaderText.Split('|').LastOrDefault());
                        //if(ячейка.Text!= "&nbsp;")
                        //sum += Convert.ToDouble(ячейка.Text);
                        //var linkbtn = new LinkButton();
                        //var cost_lbl = new Label() {Text=cost.ToString()+" руб." };
                        var separator = new Label() { Text = " | "};
                        var linkbtn_cash = new LinkButton() { Text="Нал." };
                        var linkbtn_notcash = new LinkButton() { Text = "Безнал." };
                        var cell_учет = new LinkButton() { Text= ячейка.Text=="Неучет"?"Неучет":"Учет" };
                        
                        if (isAllow)
                        {
                           // linkbtn_notcash.Enabled=linkbtn_cash.Enabled = isChecked;
                        }
                        else linkbtn_notcash.Enabled = linkbtn_cash.Enabled = isAllow;

                        bool IsPaid = false;
                        if (ячейка.Text == "Нал." || ячейка.Text == "Безнал.")
                            IsPaid = true;
                        else IsPaid = false;

                        linkbtn_cash.Click += (ob, eb) =>
                        {
                            PayBtn(ячейка, debt, ID, "Нал.");
                        };
                        linkbtn_notcash.Click += (ob, eb) =>
                        {
                            PayBtn(ячейка, debt, ID, "Безнал.");
                        };
                        cell_учет.Click += (ob, eb) =>
                        {
                            var obj = (LinkButton)ob;
                            if (obj.Text != "Неучет")
                                PayBtn(ячейка, debt, ID, "Неучет");
                            else PayBtn(ячейка, debt, ID, "0");
                        };
                        if (!IsPaid && ячейка.Text!="Неучет")
                        {
                            ячейка.Controls.Add(cell_учет);
                            ячейка.Controls.Add(new Label() { Text = " | " });
                            ячейка.Controls.Add(linkbtn_cash);
                            ячейка.Controls.Add(separator);
                            ячейка.Controls.Add(linkbtn_notcash);
                        }
                        else
                        {
                            var linkbtn_return = new LinkButton() { Text = ячейка.Text };
                            linkbtn_return.Click += (ob, eb) =>
                            {
                                PayBtn(ячейка, debt, ID, "0");
                            };
                            ячейка.Controls.Add(linkbtn_return);
                        }
                        //style
                        if (ячейка.Text == "Нал." || ячейка.Text == "Безнал.")
                            ячейка.Attributes.Add("class", "debtorspaid");//yellow dolg
                        else if(ячейка.Text == "Неучет")
                            ячейка.Attributes.Add("style", "background-color:#423f3f5c");
                    }
                }
            }
            ПереРассчитатьДолгВсехСотрудников();
        }
        protected void PayBtn(DataControlFieldCell ячейка,double debt,int ID, string typePaid)
        {
            var getdebtsum = ячейка.ContainingField.HeaderText.Split('|')[1].Trim();
            if (typePaid==("0"))
            {
                using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                using (SqlCommand sqlCommand = new SqlCommand("UPDATE Debtors SET [" + ячейка.ContainingField.HeaderText + "]='0' WHERE ID=@ID", sqlConnection))
                {
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@ID", ID);
                    sqlCommand.Parameters.AddWithValue("@Столбец", ячейка.ContainingField.HeaderText);
                    sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();
                }
                ПереРассчитатьДолгСотрудника(debt - Convert.ToDouble(getdebtsum), ID);
                GVDataDebtorsSQL.DataBind();
                Заполнить_Стили_Таблицы();
            }
            else
            {
                using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                using (SqlCommand sqlCommand = new SqlCommand("UPDATE Debtors SET [" + ячейка.ContainingField.HeaderText + "]='"+ typePaid + "' WHERE ID=@ID", sqlConnection))
                {
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@ID", ID);
                    sqlCommand.Parameters.AddWithValue("@Столбец", ячейка.ContainingField.HeaderText);
                    sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();
                }
                ПереРассчитатьДолгСотрудника(debt + Convert.ToDouble(getdebtsum), ID);
                GVDataDebtorsSQL.DataBind();
                Заполнить_Стили_Таблицы();
            }
            GVDataDebtorsSQL.DataBind();
            Заполнить_Стили_Таблицы();
        }
        protected void ПереРассчитатьДолгСотрудника(double sum, int ID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
            using (SqlCommand sqlCommand = new SqlCommand("UPDATE Debtors SET Долг=@Долг WHERE ID=@ID", sqlConnection))
            {
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ID", ID);
                sqlCommand.Parameters.AddWithValue("@Долг", sum);
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
        }
        class ListSumClass
        {
            public int ID { get; set; }
            public bool Учет { get; set; }
            public double sum { get; set; }
        }
        protected void УстановитьДолгНовымСотрудникам(string column,int id)
        {
            if (column.Contains("{Скрыто}")) return;
            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
            using (SqlCommand sqlCommand = new SqlCommand("UPDATE Debtors SET "+column+"=@Долг WHERE ID_=@ID_", sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@ID_", id);
                sqlCommand.Parameters.AddWithValue("@Долг", (0).ToString());
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
        }
        protected void ПереРассчитатьДолгВсехСотрудников()
        {
            var list_headers = new List<string>();
            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
            using (SqlCommand sqlCommand = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'Debtors' AND ORDINAL_POSITION>"+ index_dynamics_columns, sqlConnection))
            {
                sqlConnection.Open();
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list_headers.Add("["+(string)reader["COLUMN_NAME"]+"]");
                    }
                }
                sqlConnection.Close();
            }
            //проверка на null
            foreach (var s in list_headers)
                using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                using (SqlCommand sqlCommand = new SqlCommand("SELECT ID_," + s + " FROM Debtors WHERE " + s + " IS NULL", sqlConnection))
                {
                    sqlConnection.Open();
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (((object)reader[s.Replace("[", String.Empty).Replace("]", String.Empty)]) == DBNull.Value)
                            {
                                УстановитьДолгНовымСотрудникам(s, (int)reader["ID_"]);
                            }
                        }
                    }
                    sqlConnection.Close();
                }
            //
            if (list_headers.Count() == 0)
            {
                using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                using (SqlCommand sqlCommand = new SqlCommand("UPDATE Debtors SET Долг=0 WHERE ID>0", sqlConnection))
                {
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();
                }
                Обновить_Статистику(); 
                return;
            }
            var list_sum = new List<ListSumClass>();
            foreach (var s in list_headers)
            {
                if (s.Contains("{Скрыто}")) continue;
                using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                using (SqlCommand sqlCommand = new SqlCommand("SELECT ID,Учет," + s + " from Debtors", sqlConnection))
                {
                    sqlConnection.Open();
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var doub = s.Split('|')[1].Trim().Replace("]", String.Empty);
                            if (!list_sum.Exists(x => x.ID == (int)reader["ID"]))
                            {
                                if (((string)reader[s.Replace("[",String.Empty).Replace("]",String.Empty)]).StartsWith("0"))
                                    list_sum.Add(new ListSumClass
                                    {
                                        ID = (int)reader["ID"],
                                        Учет = (bool)reader["Учет"],
                                        sum = Convert.ToDouble(doub),
                                    });
                                else
                                    list_sum.Add(new ListSumClass
                                    {
                                        ID = (int)reader["ID"],
                                        Учет = (bool)reader["Учет"],
                                        sum = 0,
                                    });
                            }
                            else
                            {
                                if (((string)reader[s.Replace("[",String.Empty).Replace("]",String.Empty)]).StartsWith("0"))
                                    foreach (var d in list_sum)
                                    {
                                        if (d.ID == (int)reader["ID"])
                                        {
                                            d.sum += Convert.ToDouble(doub);
                                        }
                                    }
                            }
                        }
                    }
                    sqlConnection.Close();
                }
            }
            foreach (var s in list_sum)
            {
                if (s.Учет)
                    using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                    using (SqlCommand sqlCommand = new SqlCommand("UPDATE Debtors SET Долг=@Долг WHERE ID=@ID", sqlConnection))
                    {
                        sqlConnection.Open();
                        sqlCommand.Parameters.AddWithValue("@Долг", s.sum);
                        sqlCommand.Parameters.AddWithValue("@ID", s.ID);
                        sqlCommand.ExecuteNonQuery();
                        sqlConnection.Close();
                    }
                else
                    using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                    using (SqlCommand sqlCommand = new SqlCommand("UPDATE Debtors SET Долг=@Долг WHERE ID=@ID", sqlConnection))
                    {
                        sqlConnection.Open();
                        sqlCommand.Parameters.AddWithValue("@Долг", -1);
                        sqlCommand.Parameters.AddWithValue("@ID", s.ID);
                        sqlCommand.ExecuteNonQuery();
                        sqlConnection.Close();
                    }
            }
            Обновить_Статистику();
        }
        protected void УчетВзносовСотрудника_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            GridViewRow gridViewRow = (GridViewRow)chk.NamingContainer;
            var row = gridViewRow.Cells;
            foreach (DataControlFieldCell s in row)
            {
                if (s.ContainingField.HeaderText == "ID")
                {
                    var isint = Int32.TryParse(s.Text, out int res);
                    if (isint)
                    {
                        using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                        using (SqlCommand sqlCommand = new SqlCommand("UPDATE Debtors SET Учет=@Учет WHERE ID=@ID", sqlConnection))
                        {
                            sqlConnection.Open();
                            sqlCommand.Parameters.AddWithValue("@ID", res);
                            sqlCommand.Parameters.AddWithValue("@Учет", chk.Checked == true ? true : false);
                            sqlCommand.ExecuteNonQuery();
                            sqlConnection.Close();
                        }
                        var list_headers = new List<string>();
                        using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                        using (SqlCommand sqlCommand = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'Debtors' AND ORDINAL_POSITION>" + index_dynamics_columns, sqlConnection))
                        {
                            sqlConnection.Open();
                            using (SqlDataReader reader = sqlCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    list_headers.Add("[" + (string)reader["COLUMN_NAME"] + "]");
                                }
                            }
                            sqlConnection.Close();
                        }
                        foreach (var h in list_headers)
                        {
                            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                            using (SqlCommand sqlCommand = new SqlCommand("UPDATE Debtors SET "+h+"=@hv WHERE ID=@ID", sqlConnection))
                            {
                                sqlConnection.Open();
                                sqlCommand.Parameters.AddWithValue("@ID", res);
                                sqlCommand.Parameters.AddWithValue("@h", h);
                                sqlCommand.Parameters.AddWithValue("@hv", chk.Checked == true ? "0":"Неучет");
                                sqlCommand.ExecuteNonQuery();
                                sqlConnection.Close();
                            }
                        }
                    }
                }
            }
            //GVDataDebtorsSQL.DataBind();
            Обновить_Статистику();
        }
        protected void Добавить_Ответственное_Лицо()
        {
            string label = String.Empty;
            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
            using (SqlCommand sqlCommand = new SqlCommand("SELECT FIO from UsersCarcharoth where ProjectDebtors=1", sqlConnection))
            {
                sqlConnection.Open();
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        label += (string)reader["FIO"]+"\n";
                    }
                }
                sqlConnection.Close();
            }
            if (!String.IsNullOrEmpty(label.Trim()))
            {
                var lb = label.Replace("\n", ", ");
                ResponsibilityUsersLabel.Text = "Ответственное лицо: " + lb.Remove(lb.Length-2,2);
            }
            else
            {
                ResponsibilityUsersLabel.Text = "Ответственное лицо: нет";
            }
        }
        class StatCashAndBank
        {
            public string column_name { get; set; }
            public double cost { get; set; }
            public int count_nal { get; set; }
            public int count_beznal { get; set; }
            public double sum_nal { get; set; }
            public double sum_beznal { get; set; }
        }
        protected void Обновить_Статистику()
        {
            var list_headers = new List<StatCashAndBank>();
            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
            using (SqlCommand sqlCommand = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'Debtors' AND ORDINAL_POSITION>" + index_dynamics_columns, sqlConnection))
            {
                sqlConnection.Open();
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (((string)reader["COLUMN_NAME"]).ToString().StartsWith("{Скрыто}")) continue;
                        var cost = (string)reader["COLUMN_NAME"].ToString().Split('|')[1].Trim();

                        list_headers.Add(new StatCashAndBank {
                            column_name= (string)reader["COLUMN_NAME"],
                            cost=Convert.ToDouble(cost),
                        });
                    }
                }
                sqlConnection.Close();
            }

            //
            int количество_сотрудников = 0;
            int учитываются = 0;
            int без_долгов = 0;
            int должников = 0;
            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
            using (SqlCommand sqlCommand = new SqlCommand("SELECT * from Debtors", sqlConnection))
            {
                sqlConnection.Open();
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //label += (string)reader["FIO"] + "\n";
                        количество_сотрудников++;
                        if ((bool)reader["Учет"] == true)
                            учитываются++;
                        if ((double)reader["Долг"] == 0 && (bool)reader["Учет"] == true)
                            без_долгов++;
                        if ((double)reader["Долг"] != 0 && (bool)reader["Учет"] == true)
                            должников++;
                        //

                        foreach (var s in list_headers)
                        {
                            if (s.column_name.StartsWith("{Скрыто}")) continue;
                            var value = (string)reader[s.column_name];
                            if (value != "Неучет")
                            {
                                if (value == "Нал.")
                                {
                                    s.count_nal = s.count_nal + 1;
                                    s.sum_nal += s.cost;
                                }
                                if (value == "Безнал.")
                                {
                                    s.count_beznal = s.count_beznal + 1;
                                    s.sum_beznal += s.cost;
                                }
                            }
                        }
                    }
                }
                sqlConnection.Close();
            }
            Статистика_Сотрудники_Всего.Text = количество_сотрудников.ToString();
            Статистика_Сотрудники_Учитываются.Text = учитываются.ToString();
            Статистика_Сотрудники_БезДолгов.Text = без_долгов.ToString();
            Статистика_Сотрудники_Должников.Text = должников.ToString();
            Взнос_DataList.DataSource = list_headers;
            Взнос_DataList.DataBind();
            //foreach(var s in list_headers)
            //{
            //    var column = new Label() { Text=s.column_name };
            //    var count_nal = new Label() { Text=s.count_nal.ToString() };
            //    var sum_nal = new Label() { Text = s.sum_nal.ToString() };
            //    var count_beznal = new Label() { Text = s.count_beznal.ToString() };
            //    var sum_beznal = new Label() { Text = s.sum_beznal.ToString() };

            //    Панель_взносов.Controls.Add(column);
            //    Панель_взносов.Controls.Add(count_nal);
            //    Панель_взносов.Controls.Add(sum_nal);
            //    Панель_взносов.Controls.Add(count_beznal);
            //    Панель_взносов.Controls.Add(sum_beznal);
            //}

        }
        protected void GVDataDebtorsSQL_Load(object sender, EventArgs e)
        {
            //ДобавитьКнопкиУправления();
        }
        protected void GVDataDebtorsSQL_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Visible = false;
                e.Row.Cells[2].Visible = false;
                e.Row.Cells[3].Visible = false;
                e.Row.Cells[4].Visible = false;
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    var f = e.Row.Cells[i] as DataControlFieldCell;
                    if (f != null)
                        if (!AllowBailee(f.ContainingField.HeaderText) && f.ContainingField.HeaderText.Contains("{Скрыто}"))
                        {
                            e.Row.Cells[i].Visible = false;
                            break;
                        }
                }

            }
        }
        public int GetColumnIndexByHeaderText(GridView aGridView, String ColumnText)
        {
            TableCell Cell;
            for (int Index = 0; Index < aGridView.HeaderRow.Cells.Count; Index++)
            {
                Cell = aGridView.HeaderRow.Cells[Index];
                if (Cell.Text.ToString() == ColumnText)
                    return Index;
            }
            return -1;
        }
        protected void AcceptGVDataSortBtn_Click(object sender, EventArgs e)
        {
            SortGrid(true);
            //GVDataDebtorsSQL.DataBind();
            //Заполнить_Стили_Таблицы();
        }
        private void SortGrid(bool isDo)
        {
            bool ischecked = false;
            var ses = Session["SortDebtors"];
            if (ses != null)
            {
                bool sesbool = (bool)ses;
                if (sesbool == true)
                {
                    ischecked = true;
                }
                else
                {
                    ischecked = false;
                }
            }
            else Session["SortDebtors"] = false;
            if (ischecked)
            {
                DataDebtorsSQL.SelectCommand = "SELECT * FROM [Debtors] ORDER BY [Учет] DESC, ФИО ASC";
                AcceptGVDataSortBtn.Text = "Выбрать сортировку по направлениям";
                ischecked = false;
            }
            else
            {
                AcceptGVDataSortBtn.Text = "Выбрать сортировку только по сотрудникам";
                DataDebtorsSQL.SelectCommand = "SELECT * FROM [Debtors] ORDER BY [Учет] DESC, CASE [Направление] WHEN 'Не определено' THEN 5 ELSE 0 END, [Направление] DESC, ФИО ASC";
                ischecked = true;
            }
            if (isDo)
            {
                Session["SortDebtors"] = ischecked;
                Response.Redirect(Request.RawUrl);
            }
        }
    }

    //infoaboutserver.AddHistory("CTR поиск: " + TextBoxSearchCCACCITSSQLS.Text);

}