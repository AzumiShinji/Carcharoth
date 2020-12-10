using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Carcharoth
{
    public partial class editusers : System.Web.UI.Page
    {
        protected List<UsersFields> ListUsers = new List<UsersFields>();
        protected class UsersFields
        {
            public int ID { get; set; }
            public string FIO { get; set; }
            public string Login { get; set; }
            public string LastTimeEnter { get; set; }
            public int Level { get; set; }
            public int ProjectAll { get; set; }
            public int ProjectDSFK { get; set; }
            public int ProjectEB { get; set; }
            public int ProjectGASU { get; set; }
            public int ProjectGISGMP { get; set; }
            public int ProjectGISGMU { get; set; }
            public int ProjectSUFD { get; set; }
            public int ProjectOneC { get; set; }
            public int ProjectKS { get; set; }
            public int ProjectMI { get; set; }
            public int ProjectUC { get; set; }
            public int ProjectUD { get; set; }
            public int ProjectDebtors { get; set; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                if (Session["Level"] == null)
                    Response.Redirect("/catalog.aspx");
                else
                {
                    if ((int)Session["Level"] >= 10)
                        if (!IsPostBack)
                        {
                            GetData();
                        }
                }
            }
            else
            {
                Server.Transfer("403.html", true);
            }
        }

        protected void GetData()
        {
            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
            using (SqlCommand sqlCommand = new SqlCommand("SELECT * from UsersCarcharoth", sqlConnection))
            {
                sqlConnection.Open();
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ListUsers.Add(new UsersFields
                        {
                            ID = (int)reader["Id"],
                            FIO = (string)reader["FIO"],
                            Login = (string)reader["Login"],
                            Level = (object)reader["Level"] == DBNull.Value ? 0 : (int)reader["Level"],
                            ProjectAll = (object)reader["ProjectAll"] == DBNull.Value ? 0 : (int)reader["ProjectAll"],
                            ProjectDSFK = (object)reader["ProjectDSFK"] == DBNull.Value ? 0 : (int)reader["ProjectDSFK"],
                            ProjectEB = (object)reader["ProjectEB"] == DBNull.Value ? 0 : (int)reader["ProjectEB"],
                            ProjectGASU = (object)reader["ProjectGASU"] == DBNull.Value ? 0 : (int)reader["ProjectGASU"],
                            ProjectGISGMP = (object)reader["ProjectGISGMP"] == DBNull.Value ? 0 : (int)reader["ProjectGISGMP"],
                            ProjectGISGMU = (object)reader["ProjectGISGMU"] == DBNull.Value ? 0 : (int)reader["ProjectGISGMU"],
                            ProjectSUFD = (object)reader["ProjectSUFD"] == DBNull.Value ? 0 : (int)reader["ProjectSUFD"],
                            ProjectOneC = (object)reader["ProjectOneC"] == DBNull.Value ? 0 : (int)reader["ProjectOneC"],
                            ProjectKS = (object)reader["ProjectKS"] == DBNull.Value ? 0 : (int)reader["ProjectKS"],
                            ProjectMI = (object)reader["ProjectMI"] == DBNull.Value ? 0 : (int)reader["ProjectMI"],
                            ProjectUC = (object)reader["ProjectUC"] == DBNull.Value ? 0 : (int)reader["ProjectUC"],
                            ProjectUD = (object)reader["ProjectUD"] == DBNull.Value ? 0 : (int)reader["ProjectUD"],
                            ProjectDebtors = (object)reader["ProjectDebtors"] == DBNull.Value ? 0 : (int)reader["ProjectDebtors"],
                            LastTimeEnter = (object)reader["LastTimeEnter"] == DBNull.Value ? "Никогда" : (string)reader["LastTimeEnter"],
                        });
                    }
                }
                sqlConnection.Close();
            }
            UsersGrid.DataSource = ListUsers;
            UsersGrid.DataBind();
        }

        protected void UsersGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "ResetPassword")
                {
                    string _Login = e.CommandArgument.ToString();
                    if (_Login == "PyatkoBV@fsfk.local") { StatusText.Text= "Действие невозможно: нет доступа"; return; }
                    using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                    using (SqlCommand sqlCommand = new SqlCommand("UPDATE UsersCarcharoth SET Password=@ST_pass WHERE Login like @_Login", sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@ST_pass", "123");
                        sqlCommand.Parameters.AddWithValue("@_Login", _Login);
                        sqlConnection.Open();
                        sqlCommand.ExecuteScalar();
                        sqlConnection.Close();
                        StatusText.Text = "Пароль пользователя: " + _Login + " сброшен до стандартного - 123";
                    }
                    infoaboutserver.AddHistory("Таблица пользователей: " + StatusText.Text);
                }
                if (e.CommandName == "DeleteUser")
                {
                    string _Login = e.CommandArgument.ToString();
                    if (_Login == "PyatkoBV@fsfk.local") { StatusText.Text="Действие невозможно: нет доступа"; return; }
                    using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                    using (SqlCommand sqlCommand = new SqlCommand("DELETE FROM UsersCarcharoth WHERE Login like @_Login", sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@_Login", _Login);
                        sqlConnection.Open();
                        sqlCommand.ExecuteScalar();
                        sqlConnection.Close();
                        StatusText.Text = "Пользователь: " + _Login + " - удален";
                        ListUsers.Clear();
                        GetData();
                        infoaboutserver.AddHistory("Таблица пользователей: "+ StatusText.Text);
                    }
                }
            }
            catch (Exception es) { Response.Write(es.Message); }
        }

        protected void SubmitChange_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                {
                    foreach (GridViewRow row in UsersGrid.Rows)
                    {
                        
                        string Login = String.Empty, FIO = String.Empty;
                        int Level = 0, ProjectAll = 0,
                        ProjectDSFK = 0, ProjectEB = 0, ProjectGASU = 0, ProjectGISGMP = 0
                        , ProjectGISGMU = 0, ProjectSUFD = 0, ProjectOneC = 0, ProjectKS = 0, 
                        ProjectMI = 0, ProjectUC = 0, ProjectUD = 0, ProjectDebtors=0;

                        int Id = Convert.ToInt32(UsersGrid.Rows[row.RowIndex].Cells[0].Text);

                        TextBox _Login = (TextBox)row.FindControl("Login");
                        if (_Login.Text.Contains("@fsfk.local"))
                            Login = _Login.Text;
                        else { StatusText.Text = "Логин пользователя должен соответствовать правилам: example@fsfk.local"; return; }

                        TextBox _FIO = (TextBox)row.FindControl("FIO");
                        if (_FIO.Text != String.Empty)
                            FIO = _FIO.Text;
                        else { StatusText.Text = "Поле: ФИО - не может быть пустым"; return; }

                        TextBox _Level = (TextBox)row.FindControl("Level");
                        if (_Level.Text != String.Empty)
                            Level = Convert.ToInt32(_Level.Text);
                        else { StatusText.Text = "Поле: Уровень - не может быть пустым"; return; }

                        CheckBox _ProjectAll = (CheckBox)row.FindControl("ProjectAll");
                        if (_ProjectAll.Checked) ProjectAll = 1;
                        else ProjectAll = 0;

                        CheckBox _ProjectDSFK = (CheckBox)row.FindControl("ProjectDSFK");
                        if (_ProjectDSFK.Checked) ProjectDSFK = 1;
                        else ProjectDSFK = 0;

                        CheckBox _ProjectEB = (CheckBox)row.FindControl("ProjectEB");
                        if (_ProjectEB.Checked) ProjectEB = 1;
                        else ProjectEB = 0;

                        CheckBox _ProjectGASU = (CheckBox)row.FindControl("ProjectGASU");
                        if (_ProjectGASU.Checked) ProjectGASU = 1;
                        else ProjectGASU = 0;

                        CheckBox _ProjectGISGMP = (CheckBox)row.FindControl("ProjectGISGMP");
                        if (_ProjectGISGMP.Checked) ProjectGISGMP = 1;
                        else ProjectGISGMP = 0;

                        CheckBox _ProjectGISGMU = (CheckBox)row.FindControl("ProjectGISGMU");
                        if (_ProjectGISGMU.Checked) ProjectGISGMU = 1;
                        else ProjectGISGMU = 0;

                        CheckBox _ProjectSUFD = (CheckBox)row.FindControl("ProjectSUFD");
                        if (_ProjectSUFD.Checked) ProjectSUFD = 1;
                        else ProjectSUFD = 0;

                        CheckBox _ProjectOneC = (CheckBox)row.FindControl("ProjectOneC");
                        if (_ProjectOneC.Checked) ProjectOneC = 1;
                        else ProjectOneC = 0;

                        CheckBox _ProjectKS = (CheckBox)row.FindControl("ProjectKS");
                        if (_ProjectKS.Checked) ProjectKS = 1;
                        else ProjectKS = 0;

                        CheckBox _ProjectMI = (CheckBox)row.FindControl("ProjectMI");
                        if (_ProjectMI.Checked) ProjectMI = 1;
                        else ProjectMI = 0;

                        CheckBox _ProjectUC = (CheckBox)row.FindControl("ProjectUC");
                        if (_ProjectUC.Checked) ProjectUC = 1;
                        else ProjectUC = 0;

                        CheckBox _ProjectUD = (CheckBox)row.FindControl("ProjectUD");
                        if (_ProjectUD.Checked) ProjectUD = 1;
                        else ProjectUD = 0;

                        CheckBox _ProjectDebtors = (CheckBox)row.FindControl("ProjectDebtors");
                        if (_ProjectDebtors.Checked) ProjectDebtors = 1;
                        else ProjectDebtors = 0;

                        if (Id != 1)
                        using (SqlCommand sqlCommand = new SqlCommand("UPDATE UsersCarcharoth SET FIO=@FIO, Login=@Login, Level=@Level, ProjectAll=@ProjectAll, ProjectDSFK=@ProjectDSFK, ProjectEB=@ProjectEB, ProjectGASU=@ProjectGASU, ProjectGISGMP=@ProjectGISGMP, ProjectGISGMU=@ProjectGISGMU, ProjectSUFD=@ProjectSUFD, ProjectOneC=@ProjectOneC, ProjectKS=@ProjectKS, ProjectMI=@ProjectMI, ProjectUC=@ProjectUC, ProjectUD=@ProjectUD, ProjectDebtors=@ProjectDebtors WHERE Id=@Id", sqlConnection))
                        {
                            sqlConnection.Open();
                            sqlCommand.Parameters.AddWithValue("@FIO", FIO);
                            sqlCommand.Parameters.AddWithValue("@Login", Login);
                            sqlCommand.Parameters.AddWithValue("@Level", Level);
                            sqlCommand.Parameters.AddWithValue("@ProjectAll", ProjectAll);
                            sqlCommand.Parameters.AddWithValue("@ProjectDSFK", ProjectDSFK);
                            sqlCommand.Parameters.AddWithValue("@ProjectEB", ProjectEB);
                            sqlCommand.Parameters.AddWithValue("@ProjectGASU", ProjectGASU);
                            sqlCommand.Parameters.AddWithValue("@ProjectGISGMP", ProjectGISGMP);
                            sqlCommand.Parameters.AddWithValue("@ProjectGISGMU", ProjectGISGMU);
                            sqlCommand.Parameters.AddWithValue("@ProjectSUFD", ProjectSUFD);
                            sqlCommand.Parameters.AddWithValue("@ProjectOneC", ProjectOneC);
                            sqlCommand.Parameters.AddWithValue("@ProjectKS", ProjectKS);
                            sqlCommand.Parameters.AddWithValue("@ProjectMI", ProjectMI);
                            sqlCommand.Parameters.AddWithValue("@ProjectUC", ProjectUC);
                            sqlCommand.Parameters.AddWithValue("@ProjectUD", ProjectUD);
                            sqlCommand.Parameters.AddWithValue("@ProjectDebtors", ProjectDebtors);
                            sqlCommand.Parameters.AddWithValue("@Id", Id);
                            sqlCommand.ExecuteNonQuery();
                            sqlConnection.Close();
                        }
                    }
                    ListUsers.Clear();
                    GetData();
                    StatusText.Text = "Таблица успешно обновлена";
                    infoaboutserver.AddHistory("Таблица пользователей обновлена");
                }
            }
            catch (Exception ex) { Response.Write(ex.Message); StatusText.Text = ex.Message; }
        }

        protected void BtnAddUser_Click(object sender, EventArgs e)
        {
            string Login = String.Empty, FIO = String.Empty;

            if (tbFIO.Text != String.Empty)
                FIO = tbFIO.Text;
            else { StatusText.Text = "Поле: ФИО - не может быть пустым"; return; }

            if (tbLogin.Text.Contains("@fsfk.local"))
                Login = tbLogin.Text;
            else { StatusText.Text = "Логин пользователя должен соответствовать правилам: example@fsfk.local"; return; }

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                using (SqlCommand sqlCommand = new SqlCommand("Select Count(Login) from UsersCarcharoth where Login like @Login", sqlConnection))
                {
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@Login", Login);
                    if (Convert.ToInt32(sqlCommand.ExecuteScalar().ToString()) != 0)
                    {
                        StatusText.Text = "Пользователь с таким логином (" + Login + ") уже существует!";
                        return;
                    }
                    sqlConnection.Close();

                    using (SqlCommand sqlCommand2 = new SqlCommand("INSERT INTO UsersCarcharoth ( FIO, Login, Password ) VALUES(@FIO,@Login, @Password) ", sqlConnection))
                    {
                        sqlCommand2.Parameters.AddWithValue("@Fio", FIO);
                        sqlCommand2.Parameters.AddWithValue("@Login", Login);
                        sqlCommand2.Parameters.AddWithValue("@Password", "123");
                        sqlConnection.Open();
                        sqlCommand2.ExecuteNonQuery();
                        sqlConnection.Close();
                        StatusText.Text = "Пользователь (" + Login + ") успешно добавлен, стандартный пароль для входа - 123";
                        ListUsers.Clear();
                        GetData();
                        infoaboutserver.AddHistory("Таблица пользователей: " + StatusText.Text);
                    }
                }
            }
            catch (Exception ex) { Response.Write(ex.Message); }
        }
    }
}