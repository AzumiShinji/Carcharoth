using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Carcharoth
{
    public partial class employess : System.Web.UI.Page
    {
        private SqlConnection conn = new SqlConnection(index.CM);
        protected int Level = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                if (Session["Level"] == null)
                    Response.Redirect("/catalog.aspx");
                Level = (int)Session["Level"];
                if ((int)Session["Level"] >= 2)
                {
                    if (!IsPostBack)
                    {
                        GetData_();
                    }
                    EmployeesGrid.Columns[EmployeesGrid.Columns.Count - 1].Visible = false;
                    EmployeesGrid.Columns[EmployeesGrid.Columns.Count - 2].Visible = true;
                    EmployeesGridAdd.Enabled = true;
                    EmployeesGridAdd.Visible = true;
                    if ((int)Session["Level"] >= 3)
                    { 
                        EmployeesGrid.Columns[EmployeesGrid.Columns.Count - 1].Visible = true;
                    }
                }
                else
                {
                    if (!IsPostBack)
                    {
                        GetData_();
                    }
                    EmployeesGrid.Columns[EmployeesGrid.Columns.Count - 1].Visible = false;
                    EmployeesGrid.Columns[EmployeesGrid.Columns.Count - 2].Visible = false;
                    EmployeesGridAdd.Enabled = false;
                    EmployeesGridAdd.Visible = false;
                }
              
            }
            else
            {
                if (!IsPostBack)
                {
                    GetData_();
                }
                EmployeesGrid.Columns[EmployeesGrid.Columns.Count - 1].Visible = false;
                EmployeesGrid.Columns[EmployeesGrid.Columns.Count - 2].Visible = false;
                EmployeesGridAdd.Enabled = false;
                EmployeesGridAdd.Visible = false;
            }
        }

        protected void GetData_()
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("Select * from users ORDER BY Case WHEN Direction = N'Не определено' THEN NULL ELSE Direction END DESC, ID ASC", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            Session["EmployeesGridView"] = ds;
            conn.Close();

            if (ds.Tables[0].Rows.Count > 0)
            {
                DataView dv = new DataView(ds.Tables[0]);
                var sort = Session["SelectedSort"] as string;
                if (!String.IsNullOrEmpty(sort))
                {
                    dv.Sort = sort;
                    EmployeesGrid.DataSource = dv;
                }
                else
                    EmployeesGrid.DataSource = ds;
                EmployeesGrid.DataBind();
            }
            else
            {
                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                EmployeesGrid.DataSource = ds;
                EmployeesGrid.DataBind();
                int columncount = EmployeesGrid.Rows[0].Cells.Count;
                EmployeesGrid.Rows[0].Cells.Clear();
                EmployeesGrid.Rows[0].Cells.Add(new TableCell());
                EmployeesGrid.Rows[0].Cells[0].ColumnSpan = columncount;
                EmployeesGrid.Rows[0].Cells[0].Text = "Нет записей";
            }
            GetListForBirthDate(ds);
        }

        private const string ASCENDING = " ASC";
        private const string DESCENDING = " DESC";

        public SortDirection GridViewSortDirection
        {
            get
            {
                if (ViewState["sortDirection"] == null)
                    ViewState["sortDirection"] = SortDirection.Ascending;

                return (SortDirection)ViewState["sortDirection"];
            }
            set { ViewState["sortDirection"] = value; }
        }

        protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortExpression = e.SortExpression;

            if (GridViewSortDirection == SortDirection.Ascending)
            {
                GridViewSortDirection = SortDirection.Descending;
                SortGridView(sortExpression, DESCENDING);
            }
            else
            {
                GridViewSortDirection = SortDirection.Ascending;
                SortGridView(sortExpression, ASCENDING);
            }

        }

        private void SortGridView(string sortExpression, string direction)
        {
            //  You can cache the DataTable for improving performance
            //conn.Open();
            //SqlCommand cmd = new SqlCommand("Select * from users", conn);
            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataSet ds = new DataSet();
            //da.Fill(ds);
            //conn.Close();


            //DataTable dt = new DataTable(da,);

            DataSet ds = Session["EmployeesGridView"] as DataSet;
            DataView dv = new DataView(ds.Tables[0]);
            dv.Sort = sortExpression + direction;
            Session["SelectedSort"] = dv.Sort;
            EmployeesGrid.DataSource = dv;
            EmployeesGrid.DataBind();
        }
        //protected void EmployeesGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        //{
        //GridViewRow row = (GridViewRow)EmployeesGrid.Rows[e.RowIndex];
        //var id = EmployeesGrid.Rows[e.RowIndex].DataItemIndex;
        //string Email = "";
        //using (SqlConnection sqlConnection = new SqlConnection(index.CM))
        //using (SqlCommand sqlCommand = new SqlCommand("SELECT * from Users Where ID=@ID", sqlConnection))
        //{
        //    sqlCommand.Parameters.AddWithValue("@ID", Convert.ToInt32(EmployeesGrid.DataKeys[e.RowIndex].Value.ToString()));
        //    sqlConnection.Open();
        //    using (SqlDataReader reader = sqlCommand.ExecuteReader())
        //    {
        //        while (reader.Read())
        //        {
        //            Email = (string)reader["Email"];
        //        }
        //    }
        //    sqlConnection.Close();
        //}
        //conn.Open();
        //SqlCommand cmd = new SqlCommand("delete FROM users where ID='" + Convert.ToInt32(EmployeesGrid.DataKeys[e.RowIndex].Value.ToString()) + "'", conn);
        //cmd.ExecuteNonQuery();
        //conn.Close();
        //GetData();
        //StatusLabel.Text = Email+" - удален";
        //infoaboutserver.AddHistory("Удален сотрудник - " + Email);
        //}

        protected void EmployeesGrid_RowEditing(object sender, GridViewEditEventArgs e)
        {
            EmployeesGrid.Columns[EmployeesGrid.Columns.Count - 1].Visible = true;
            EmployeesGrid.EditIndex = e.NewEditIndex;
            

            Label lbDisplayName = (Label)EmployeesGrid.Rows[e.NewEditIndex].FindControl("Label_Direction");
            string name = lbDisplayName.Text;

            GetData_();

            GridViewRow gvr = EmployeesGrid.Rows[e.NewEditIndex];
            var dr = (ListBox)gvr.FindControl("Direction");
            foreach (var i in dr.Items)
            {
                try
                {
                    if(name.Contains(i.ToString()))
                    dr.Items.FindByText(i.ToString()).Selected = true;
                }
                catch { }
            }
        }
         
        protected void EmployeesGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int userid = Convert.ToInt32(EmployeesGrid.DataKeys[e.RowIndex].Value.ToString());
            GridViewRow row = (GridViewRow)EmployeesGrid.Rows[e.RowIndex];
            TextBox CodeTxt = (TextBox)row.Cells[1].Controls[0];
            TextBox FIOTxt = (TextBox)row.Cells[2].Controls[0];
            TextBox EmailTxt = (TextBox)row.Cells[3].Controls[0];
            if (EmailTxt.Text.Trim() == "" || !EmailTxt.Text.EndsWith("@fsfk.local"))
            {
                StatusLabel.Text = EmailTxt.Text + " - не соответствует правилам ('example@fsfk.local') и не может быть пустым!";
                return;
            }
            ListBox DirectionTxt = (ListBox)row.FindControl("Direction");
            var selected = DirectionTxt.Items.Cast<ListItem>().Where(x => x.Selected).ToList();
            TextBox PositionTxt = (TextBox)row.Cells[5].Controls[0];
            TextBox PhoneTxt = (TextBox)row.Cells[6].Controls[0];
            //TextBox BirthDateTxt = (TextBox)row.Cells[7].Controls[0];
            var BirthDateTextBox = row.FindControl("BirthDateTextBox") as TextBox;
            string dt = "";
            if (!String.IsNullOrEmpty(BirthDateTextBox.Text))
            {
                var _dt = DateTime.TryParseExact(BirthDateTextBox.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);
                if(_dt)
                {
                    dt = result.ToString("dd.MM.yyyy");
                }
                else
                {
                    StatusLabel.Text = "Формат даты неверен, должен быть в виде: ДД.ММ.ГГГГ (01.01.1992), попытка изменения: " + EmailTxt.Text;
                    return;
                }
            }
            var VacationsTextBox = row.FindControl("VacationsTextBox") as TextBox;
            string vacations = "";
            if(!String.IsNullOrEmpty(VacationsTextBox.Text))
            { vacations = VacationsTextBox.Text.Trim(); }
            //TextBox RestTxt = (TextBox)row.Cells[8].Controls[0];
            EmployeesGrid.EditIndex = -1;
            conn.Open();
            SqlCommand cmd = new SqlCommand("UPDATE [Users] SET Code=N'"+CodeTxt.Text+ "',FIO=N'" + FIOTxt.Text + "',Email=N'" + EmailTxt.Text + "',Direction=N'" + String.Join("/", selected) + "',Position=N'" + PositionTxt.Text + "',Phone=N'" + PhoneTxt.Text + "',BirthDate=N'" + dt + "',Rest=N'" + vacations + "' WHERE ID = '" + userid + "'", conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            GetData_();
            StatusLabel.Text = "Информация о сотруднике обновлена - " + EmailTxt.Text;
            infoaboutserver.AddHistory("Информация о сотруднике обновлена - "+ EmailTxt.Text);
        }
        protected void EmployeesGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            EmployeesGrid.PageIndex = e.NewPageIndex;
            GetData_();
        }
        protected void EmployeesGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            EmployeesGrid.EditIndex = -1;
            GetData_();
        }

        protected void AddNewEmployees_Click(object sender, EventArgs e)
        {
            if(InsertEmail.Text.Trim()=="" || !InsertEmail.Text.EndsWith("@fsfk.local"))
            {
                StatusLabel.Text = InsertEmail.Text + " - не соответствует правилам ('example@fsfk.local') и не может быть пустым!";
                return;
            }
            string dt = "";
            if (!String.IsNullOrEmpty(InsertBirthDate.Text))
            {
                var _dt = DateTime.TryParseExact(InsertBirthDate.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);
                if (_dt)
                {
                    dt = result.ToString("dd.MM.yyyy");
                }
                else
                {
                    StatusLabel.Text = "Формат даты неверен, должен быть в виде: ДД.ММ.ГГГГ (01.01.1992)";
                    return;
                }
            }
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = index.CM;

                con.Open();
                using (SqlCommand sqlCommand2 = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email = @Email", con))
                {
                    sqlCommand2.Parameters.AddWithValue("@Email", InsertEmail.Text ?? "");
                    int count = (int)sqlCommand2.ExecuteScalar();

                    if (count == 0)
                    {
                        var selected = InsertDirection.Items.Cast<ListItem>().Where(x=>x.Selected).ToList();
                        using (SqlCommand sqlCommand = new SqlCommand("INSERT INTO [Users] (Code,FIO,Email,Direction,Position,Phone,BirthDate) " +
                            "VALUES (@Code,@FIO,@Email,@Direction,@Position,@Phone,@BirthDate)", con))
                        {
                            sqlCommand.Parameters.AddWithValue("@Code", InsertCode.Text ?? "");
                            sqlCommand.Parameters.AddWithValue("@FIO", InsertFIO.Text ?? "");
                            sqlCommand.Parameters.AddWithValue("@Email", InsertEmail.Text ?? "");
                            sqlCommand.Parameters.AddWithValue("@Direction", String.Join("/",selected) ?? "");
                            sqlCommand.Parameters.AddWithValue("@Position", InsertPosition.Text ?? "");
                            sqlCommand.Parameters.AddWithValue("@Phone", InsertPhone.Text ?? "");
                            sqlCommand.Parameters.AddWithValue("@BirthDate", dt);
                            sqlCommand.ExecuteNonQuery();
                            GetData_();
                            StatusLabel.Text=InsertEmail.Text + " - добавлен!";
                        }
                    }
                    else
                    {
                        StatusLabel.Text=InsertEmail.Text + " - уже существует!";
                    }
                }
                con.Close();
                infoaboutserver.AddHistory("Добавлен новый сотрудник - " + InsertEmail.Text);
            }
            catch (Exception ex) { Response.Write(ex.ToString()); }
        }

        #region BirthDate
        public void GetListForBirthDate(DataSet ds)
        {
            BirthDayTodayLabel.Text = "";
            BirthDayNextLabel.Text = "";
            ListEmployees.Clear();
            var dsedited = ds.Tables[0].AsEnumerable().Select(r => new EmployeesClass
            {
                FIO = r.Field<string>("FIO"),
                BirthDate = r.Field<string>("BirthDate"),
                Rest= r.Field<string>("Rest"),
            });
            var listwh = dsedited.ToList();
            ListEmployees = listwh;
            DetectBirthDate();
            GetVacations();
        }

        public void DetectBirthDate()
        {
            List<_DateTime> _Month = new List<_DateTime>();
            foreach (var s in ListEmployees)
            {
                var date = DateTime.TryParse(s.BirthDate, out DateTime result);
                if (date)
                {
                    _Month.Add(new _DateTime
                    {
                        BirthDate = result,
                        CompareDayMonth = new DateTime(1900, result.Month, result.Day, 0, 0, 0),
                        FIO = s.FIO
                    });
                    if (result.Day == DateTime.Now.Day && result.Month == DateTime.Now.Month)
                    {
                        //var old = DateTime.Now.Year - result.Year;
                        BirthDayTodayLabel.Text += s.FIO + " ( " + result.ToString("dd MMMM")+" )"+"<br/>";
                    }
                }
            }
            if (BirthDayTodayLabel.Text == "")
                employees_birthdate_today_panel.Visible = false;
            else employees_birthdate_today_panel.Visible = true;
            _Month.Sort((a, b) => a.CompareDayMonth.Value.CompareTo(b.CompareDayMonth.Value));

            int i = 0;
            foreach (var s in _Month)
            {
                if (s.BirthDate.Value.Month == DateTime.Now.Month && s.BirthDate.Value.Day > DateTime.Now.Day)
                {
                    i++;
                    
                    //var old = DateTime.Now.Year - s.BirthDate.Value.Year;
                    BirthDayNextLabel.Text += s.FIO + " ( " + s.BirthDate.Value.ToString("dd MMMM") + " )" + "<br/>";

                }
                else
                if (s.BirthDate.Value.Month > DateTime.Now.Month)
                {
                    i++;
                    //var old = DateTime.Now.Year - s.BirthDate.Value.Year;
                    BirthDayNextLabel.Text += s.FIO + " ( " + s.BirthDate.Value.ToString("dd MMMM") + " )" + "<br/>";
                }
                if (i == 5)
                    break;
            }
            if (i < 5)
            {
                foreach (var s in _Month)
                {
                    for (int f = 0; f < 12; f++)
                    {
                        if (s.BirthDate.Value.Month == DateTime.Now.AddMonths(f).Month && !BirthDayNextLabel.Text.Contains(s.FIO))
                        {
                            i++;
                            BirthDayNextLabel.Text += s.FIO + " ( " + s.BirthDate.Value.ToString("dd MMMM") + " )" + "<br/>";
                        }
                        if (i == 5) break;
                    }
                    if (i == 5) break;
                }
            }
        }

        public List<EmployeesClass> ListEmployees = new List<EmployeesClass>();
        public class EmployeesClass
        {
            public string FIO { get; set; }
            public string BirthDate { get; set; }
            public string Rest { get; set; }
        }
        public class _DateTime
        {
            public DateTime? BirthDate { get; set; }
            public DateTime? CompareDayMonth { get; set; }
            public string FIO { get; set; }
        }

        public string ConvertDateTimeToShort(object dt)
        {
            if (dt != null && dt != DBNull.Value)
            {
                var string_dt = (string)dt;
                var _dt = DateTime.TryParseExact(string_dt, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);
                if (_dt)
                {
                    return result.ToString("dd MMMM");
                }
            }
            return "";
        }
        #endregion

        #region Rest
        private class EmployeesVacation
        {
            public string FIO { get; set; }
            public DateTime? first { get; set; }
            public DateTime? twice { get; set; }
        }
        private void GetVacations()
        {
            RestLabel.Text = "";
            SoonRestLabel.Text = "";
            List<EmployeesVacation> SoonVacation = new List<EmployeesVacation>();
            List<EmployeesVacation> Vacation = new List<EmployeesVacation>();
            foreach (var rest in ListEmployees)
            {
                var text = rest.Rest;
                if (text != "&nbsp;")
                {
                    if (text != null)
                        if (text.Trim() != "")
                        {
                            var splitdate = text.Split('|');
                            if (splitdate.Count() > 0)
                                foreach (var s in splitdate)
                                {
                                    if (s.Trim() != "")
                                    {
                                        var g = s.Split('-');
                                        if (g.Count() == 2)
                                        {
                                            var fbool = DateTime.TryParse(g[0], out DateTime first);
                                            var tbool = DateTime.TryParse(g[1], out DateTime twice);
                                            if (fbool && tbool)
                                            {
                                                if (DateTime.Now.Date >= first && DateTime.Now.Date <= twice)
                                                {
                                                    Vacation.Add(new EmployeesVacation
                                                    {
                                                        FIO = rest.FIO,
                                                        first = first,
                                                        twice = twice,
                                                    });
                                                }
                                                if (DateTime.Now < first)
                                                    SoonVacation.Add(new EmployeesVacation
                                                    {
                                                        FIO = rest.FIO,
                                                        first = first,
                                                        twice = twice,
                                                    });
                                            }
                                        }
                                    }
                                }
                        }
                }
            }
            SoonVacation.Sort((x, y) => DateTime.Compare(x.first.Value, y.first.Value));
            foreach (var date in SoonVacation.Take(5))
                SoonRestLabel.Text += date.FIO + " c " + date.first.Value.Date.ToString("dd/MM/yyyy") +
                    " по " + date.twice.Value.Date.ToString("dd/MM/yyyy") + "<br/>";

            Vacation.Sort((x, y) => DateTime.Compare(x.first.Value, y.first.Value));
            foreach (var date in Vacation)
                RestLabel.Text += date.FIO + " c " + date.first.Value.Date.ToString("dd/MM/yyyy") +
                    " по " + date.twice.Value.Date.ToString("dd/MM/yyyy") + "<br/>";
            if (SoonVacation.Count == 0) employees_soon_vacation.Visible = false;
            else employees_soon_vacation.Visible = true;
            if (Vacation.Count == 0) employees_vacation.Visible = false;
            else employees_vacation.Visible = true;
        }
        protected void EmployeesGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (Level >= 3)
                {
                    var deletebtn = e.Row.FindControl("deletebtn") as Button;
                    deletebtn.Enabled = true;
                    deletebtn.Visible = true;
                }
                bool isVacation = false;
                bool isBirthDay = false;
                var text = e.Row.Cells[8].Text;
                if (text != "&nbsp;")
                {
                    if (text.Trim() != "")
                    {
                        var splitdate = text.Trim().Split('|');
                        if (splitdate.Count() > 0)
                            foreach (var s in splitdate)
                            {
                                if (s.Trim() != "")
                                {
                                    var g = s.Split('-');
                                    if (g.Count() == 2)
                                    {
                                        var fbool = DateTime.TryParse(g[0], out DateTime first);
                                        var tbool = DateTime.TryParse(g[1], out DateTime twice);
                                        if (fbool && tbool)
                                            if (DateTime.Now.Date >= first && DateTime.Now.Date <= twice)
                                                isVacation = true;
                                    }
                                }
                            }
                    }
                }
                var birthday = e.Row.Cells[7].Text;
                if (birthday != "&nbsp;")
                    if (birthday.Trim() != "")
                    {
                        var date = DateTime.TryParse(birthday, out DateTime result);
                        if (date)
                            if (result.Day == DateTime.Now.Day && result.Month == DateTime.Now.Month)
                                isBirthDay = true;
                    }
                var FIO = e.Row.Cells[2].Text;
                if (isVacation && !isBirthDay)
                    e.Row.Cells[2].Text = "<div class='CustomToolTip' style='display:inline-block;'><span>В отпуске!</span><img runat='server' src='/images/user-rest.png'/></div> " + FIO;
                if (!isVacation && isBirthDay)
                    e.Row.Cells[2].Text = "<div class='CustomToolTip' style='display:inline-block;'><span>День рождение!</span><img runat='server' src='/images/birthday.png'/></div> " + FIO;
                if (isVacation && isBirthDay)
                    e.Row.Cells[2].Text = "<div style='display:inline-block;'><div class='CustomToolTip' style='display:inline-block;'><span>День рождение!</span><img runat='server' src='/images/birthday.png'/></div>" +
                        "<div class='CustomToolTip' style='display:inline-block;'><span>В отпуске!</span><img runat='server' src='/images/user-rest.png'/></div></div> " + FIO;
                #region Hidden Years of Employees
                //var isDate = DateTime.TryParse(birthday,out DateTime FinalDate);
                //if(isDate)
                //e.Row.Cells[7].Text = FinalDate.ToString("dd MMMM");
                #endregion
            }
        }
        public string ConvertVacationsToReadable(object obj)
        {
            if (obj == null || obj==DBNull.Value) return "";
            string vacations = (string)obj;
            string result = "";
            if (!String.IsNullOrEmpty(vacations))
            {
                int count = 0;
                foreach(var s in vacations.Split('|'))
                {
                    if (s.Contains("-"))
                    {
                        var prev = s.Split('-');
                        if (prev.Length == 2)
                        {
                            count++;
                            bool islast = false;
                            if (count == vacations.Count())
                                islast = true;
                            if(!islast)
                            result += String.Format("{0}: с {1} по {2}<br/>", count.ToRoman(), prev[0],prev[1]);
                            else
                            result += String.Format("{0}: с {1} по {2}", count.ToRoman(), prev[0],prev[1]);
                        }
                    }
                }
            }
            return result;
        }
        #endregion

        protected void EmployeesGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch(e.CommandName)
            {
                case ("DeleteRow"):
                    {
                        GridViewRow _row = (GridViewRow)(((Button)e.CommandSource).NamingContainer);
                        int row_index = _row.RowIndex;
                        GridViewRow row = (GridViewRow)EmployeesGrid.Rows[row_index];
                        var id = EmployeesGrid.Rows[row_index].DataItemIndex;
                        string Email = "";
                        using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                        using (SqlCommand sqlCommand = new SqlCommand("SELECT * from Users Where ID=@ID", sqlConnection))
                        {
                            sqlCommand.Parameters.AddWithValue("@ID", Convert.ToInt32(EmployeesGrid.DataKeys[row_index].Value.ToString()));
                            sqlConnection.Open();
                            using (SqlDataReader reader = sqlCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Email = (string)reader["Email"];
                                }
                            }
                            sqlConnection.Close();
                        }
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("delete FROM users where ID='" + Convert.ToInt32(EmployeesGrid.DataKeys[row_index].Value.ToString()) + "'", conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        GetData_();
                        StatusLabel.Text = Email + " - удален";
                        infoaboutserver.AddHistory("Удален сотрудник - " + Email);
                        break;
                    }
                case ("Cancel"):
                    {
                        var grid = (GridView)sender;
                        grid.EditIndex = -1;
                        grid.DataBind();
                        break;
                    }
            }
        }
    }
    public static class Extenstions
    {
        public static string ToRoman(this int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }
    }
}