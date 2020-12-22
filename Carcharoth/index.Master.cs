using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace Carcharoth
{
    public partial class index : System.Web.UI.MasterPage
    {
        public class InfoAboutUsers
        {
            public List<ProjectField> Projects = new List<ProjectField>();
        }
        public class ProjectField
        {
            public int Status { get; set; }
            public string Name { get; set; }
        }
        public static string CM = ConfigurationManager.ConnectionStrings["ToCatalogDB"].ConnectionString;
        public static string ProjectsListForAllow = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                //Settings.ExpireAllCookies();
                btnLogin.Enabled = true;
                btnLogin.Visible = true;
                PanelDropDown.Enabled = false;
                PanelDropDown.Visible = false;
                Session["CurrentProject"] = "null";
            }
            else
            {
                btnLogin.Enabled = false;
                btnLogin.Visible = false;
                PanelDropDown.Enabled = true;
                PanelDropDown.Visible = true;
                GetInfoAboutUsers();
                UserLabel.Text = (string)Session["Login"];
                FIOLabel.Text = (string)Session["FIO"];
                ProjectLabel.Text = (string)Session["CurrentProject"];
                if ((int)Session["Level"] >= 10)
                {
                    linkbtnEditUsers.Enabled = true;
                    linkbtnEditUsers.Visible = true;
                    linkbtnServerInfo.Enabled = true;
                    linkbtnServerInfo.Visible = true;
                    //linkbtnEditAlerts.Visible = true;
                    //linkbtnEditAlerts.Enabled = true;
                }
            }
        }
        #region auth
        protected void Login_Click(object sender, EventArgs e)
        {
            Session["Login"] = LoginTB.Text;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                string hash = HASH.GetHash(sha256Hash, PasswordTB.Text);
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(CM))
                    using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) from UsersCarcharoth where Login like @username AND Password like @password", sqlConnection))
                    {
                        sqlConnection.Open();
                        sqlCommand.Parameters.AddWithValue("@username", Session["Login"].ToString());
                        if (PasswordTB.Text != "123")
                            sqlCommand.Parameters.AddWithValue("@password", hash);
                        else sqlCommand.Parameters.AddWithValue("@password", PasswordTB.Text);

                        int userCount = (int)sqlCommand.ExecuteScalar();
                        if (userCount > 0)
                        {
                            if (Check_Level(sqlConnection))
                            {
                                if (Check_Password(sqlConnection))
                                {
                                    SetLastTimeEnterUser();
                                    Auth(Session["Login"].ToString(), 1440);
                                }
                                else
                                {
                                    ChangePassword(true);
                                }
                            }
                            else
                            {
                                StatusLogin.Text = "Учетная запись заблокирована.";
                                ChangePassword(false);
                            }
                        }
                        else
                        {
                            StatusLogin.Text = "Неверный логин или пароль.";
                            ChangePassword(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("Ошибка: " + ex.ToString());
                }
            }
        }

        private void ChangePassword(bool over)
        {
            if (over)
            {
                ModalHeadLabel.Text = "Изменение пароля";
                modalreg.DefaultButton = btnChangePasswordinDB.ClientID;
            }
            else
            {
                ModalHeadLabel.Text = "Вход";
                modalreg.DefaultButton = Login.ClientID;
            }
            Login.Enabled = !over;
            Login.Visible = !over;
            LoginTB.Enabled = !over;
            PasswordTB.Enabled = !over;
            LoginTB.Visible = !over;
            PasswordTB.Visible = !over;
            userloginimage.Visible = !over;
            userpasswordimage.Visible = !over;

            Password1.Enabled = over;
            Password1.Visible = over;
            Password2.Enabled = over;
            Password2.Visible = over;
            btnChangePasswordinDB.Enabled = over;
            btnChangePasswordinDB.Visible = over;

            ScriptShowPopup();
        }

        private void ScriptShowPopup()
        {
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "clentscript", "ShowPopup();", true);
        }

        protected void btnChangePasswordinDB_Click(object sender, EventArgs e)
        {
            try
            {
                if (Password1.Text != "123")
                {
                    if (Password1.Text.Trim() == Password2.Text.Trim())
                    {
                        using (SHA256 sha256Hash = SHA256.Create())
                        {
                            using (SqlConnection sqlConnection = new SqlConnection(CM))
                            using (SqlCommand sqlCommand = new SqlCommand("UPDATE UsersCarcharoth SET Password=@new_password WHERE Login LIKE @username", sqlConnection))
                            {
                                sqlConnection.Open();
                                sqlCommand.Parameters.AddWithValue("@username", LoginTB.Text);
                                sqlCommand.Parameters.AddWithValue("@new_password", HASH.GetHash(sha256Hash, Password1.Text.Trim()));
                                sqlCommand.ExecuteScalar();
                                Settings.ExpireAllCookies();
                            }
                        }
                    }
                    else { StatusLogin.Text = "Пароли не совпадают."; ChangePassword(true); return; };
                }
                else { StatusLogin.Text = "Новый пароль не может быть стандартным."; ChangePassword(true); return; };
                Settings.ExpireAllCookies();
                ChangePassword(false);
            }
            catch (Exception es) { Response.Write(es.Message); }
        }

        private void Auth(string LoginTB, int min)
        {
            Settings.ExpireAllCookies();
            Response.Cookies[LoginTB].Value = LoginTB;
            Response.Cookies[LoginTB].Expires = DateTime.Now.AddMinutes(min);
            FormsAuthentication.Initialize();
            FormsAuthentication.SetAuthCookie(LoginTB, true);
            Response.Redirect(Request.RawUrl);
        }

        private bool Check_Level(SqlConnection sqlConnection)
        {
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand("SELECT Level from UsersCarcharoth where Login like @username", sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@username", LoginTB.Text);
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                            if (reader != null && (int)reader["Level"] != 0)
                                return true;
                            else
                                return false;
                        return false;
                    }
                }
            }
            catch (Exception e) { Response.Write(e.Message); return false; }
        }

        private bool Check_Password(SqlConnection sqlConnection)
        {
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand("SELECT Password, Login from UsersCarcharoth where Login like @username", sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@username", LoginTB.Text);
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LoginTB.Text = (string)reader["Login"];
                            if (reader != null && (string)reader["Password"] != "123")
                                return true;
                            else
                                return false;
                        }
                        return false;
                    }
                }
            }
            catch (Exception e) { Response.Write(e.Message); return false; }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Settings.ExpireAllCookies();
            //Page.Response.Redirect(Page.Request.Url.ToString(), true);
            Session["Level"] = -1;
            Session["CurrentProject"] = "";
            //InfoAboutUsers.Projects.Clear();
            Response.Redirect(Request.RawUrl);
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            ChangePassword(true);
        }

        protected void SetLastTimeEnterUser()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(CM))
                using (SqlCommand sqlCommand = new SqlCommand("UPDATE UsersCarcharoth SET LastTimeEnter=@LastTimeEnter WHERE Login like @username", sqlConnection))
                {
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@username", LoginTB.Text);
                    sqlCommand.Parameters.AddWithValue("@LastTimeEnter", DateTime.Now.ToString());
                    sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();
                }
            }
            catch (Exception e) { Response.Write(e.Message); }
        }
        #endregion
        protected void linkbtnEditUsers_Click(object sender, EventArgs e)
        {
            Server.Transfer("editusers.aspx", true);
        }
        public void GetInfoAboutUsers()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(CM))
                using (SqlCommand sqlCommand = new SqlCommand("SELECT * from UsersCarcharoth where Login like @username", sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@username", HttpContext.Current.User.Identity.Name);
                    sqlConnection.Open();
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                            if (reader != null && (int)reader["Level"] != 0)
                            {
                                try { Session["FIO"] = (string)reader["FIO"]; } catch { Session["FIO"] = "Неизвестно"; }
                                Session["Login"] = (string)reader["Login"];
                                Session["Level"] = (int)reader["Level"];
                                //InfoAboutUsers.Projects.Clear();
                                var g = new InfoAboutUsers().Projects;
                                g.Add(new ProjectField
                                {
                                    Status = (int)reader["ProjectAll"],
                                    Name = "Все проекты",
                                });
                                g.Add(new ProjectField
                                {
                                    Status = (int)reader["ProjectDSFK"],
                                    Name = "ДС ФК"
                                });
                                g.Add(new ProjectField
                                {
                                    Status = (int)reader["ProjectEB"],
                                    Name = "ЭБ"
                                });
                                g.Add(new ProjectField
                                {
                                    Status = (int)reader["ProjectGASU"],
                                    Name = "ГАСУ"
                                });
                                g.Add(new ProjectField
                                {
                                    Status = (int)reader["ProjectGISGMP"],
                                    Name = "ГИС ГМП"
                                });
                                g.Add(new ProjectField
                                {
                                    Status = (int)reader["ProjectGISGMU"],
                                    Name = "ГИС ГМУ"
                                });
                                g.Add(new ProjectField
                                {
                                    Status = (int)reader["ProjectKS"],
                                    Name = "Казначейское сопровождение"
                                });
                                g.Add(new ProjectField
                                {
                                    Status = (int)reader["ProjectOneC"],
                                    Name = "1С"
                                });
                                g.Add(new ProjectField
                                {
                                    Status = (int)reader["ProjectSUFD"],
                                    Name = "СУФД"
                                });
                                g.Add(new ProjectField
                                {
                                    Status = (int)reader["ProjectMI"],
                                    Name = "Менеджер инцидентов"
                                });
                                g.Add(new ProjectField
                                {
                                    Status = (int)reader["ProjectUC"],
                                    Name = "Удостоверяющий центр"
                                });
                                g.Add(new ProjectField
                                {
                                    Status = (int)reader["ProjectUD"],
                                    Name = "Управление делами"
                                });
                                g.Add(new ProjectField
                                {
                                    Status = reader["ProjectDebtors"]==DBNull.Value?0: (int)reader["ProjectDebtors"],
                                    Name = "Взносы"
                                });
                                Session["Projects"] = g;
                            }
                    }
                }
                string cp = "";
                foreach (var s in Session["Projects"] as List<ProjectField>)
                {
                    if (s.Status != 0)
                    { cp += s.Name + ","; }
                }
                if (cp != "")
                {
                    cp = cp.Remove(cp.Length - 1);
                }
                Session["CurrentProject"] = cp;
                if (Session["CurrentProject"].ToString().Trim() == "")
                    Session["CurrentProject"] = "Нет проектов";
            }
            catch (Exception e) { Response.Write(e.ToString()); }
        }
        private void MessageBoxShow(string message)
        {
            ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "clentscript", "alert('" + message + "');", true);
        }

        #region EditAddDeletePost
        protected void DeletePost_Click(object sender, EventArgs e)
        {
            try
            {
                int _id = Convert.ToInt32(id_edit.Value);
                using (SqlConnection sqlConnection = new SqlConnection(CM))
                using (SqlCommand sqlCommand = new SqlCommand("DELETE FROM Data where Id = @Id", sqlConnection))
                {
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@Id", _id);
                    sqlCommand.ExecuteNonQuery();
                }
                infoaboutserver.AddHistory("База знаний: Пост удален #" + _id);
                if (Session["NameIT"] != null)
                    Session["PrevSelectedNode"] = Session["NameIT"];
                Response.Redirect(Request.RawUrl);
            }
            catch (Exception ex) { Response.Write(ex.ToString()); }
        }

        protected void AddEditPosts_Click(object sender, EventArgs e)
        {
            try
            {
                UploadFile();
                foreach (var s in ListFileUploaded)
                    if (s.PathFile != "")
                    {
                        text_edit.Text += "<p><img src='/images/attachments.png' width='20' />Вложение: <a target='_blank' href='" + s.PathFile + "'>" + s.FileName + "</a></p>";
                        text_edit.Text = text_edit.Text.Replace('\'', '\"');
                    }

                if (Session["NameIT"].ToString() == "Главная страница") { MessageBoxShow("Нельзя статью поместить на главную страницу"); return; }
                if (Session["NameIT"].ToString().Trim() == "") { MessageBoxShow("Не указан IT-сервис"); return; }
                if ((Session["NameIT"].ToString().Trim() == "Общая информация" || 
                    Session["NameIT"].ToString().Trim() == "Другие проекты") && isfirstline_edit.Checked==true) { MessageBoxShow("Нельзя статью пометить как Решение для 1-ой линии"); return; }
                int _id = 0;
                try
                {
                    _id = Convert.ToInt32(id_edit.Value);
                }
                catch { }
                using (SqlConnection sqlConnection = new SqlConnection(CM))
                using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Data where Id = @Id", sqlConnection))
                {
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@Id", _id);
                    int count = (int)sqlCommand.ExecuteScalar();
                    if (count == 0)
                    {
                        using (SqlCommand sqlCommand2 = new SqlCommand("INSERT INTO [Data] (IT,isImportant,isMainPage,isFirstLine,Head,Text,KeyWords,SD,WhoCreated,WhenCreated) " +
                                "VALUES (@IT,@isImportant,@isMainPage,@isFirstLine,@Head,@Text,@KeyWords,@SD,@WhoCreated,@WhenCreated)", sqlConnection))
                        {
                            sqlCommand2.Parameters.AddWithValue("@IT", Session["NameIT"] ?? "");
                            sqlCommand2.Parameters.AddWithValue("@isImportant", isimportant_edit.Checked == true ? "1" : "0");
                            sqlCommand2.Parameters.AddWithValue("@isMainPage", ismainpage_edit.Checked == true ? "1" : "0");
                            sqlCommand2.Parameters.AddWithValue("@isFirstLine", isfirstline_edit.Checked == true ? "1" : "0");
                            sqlCommand2.Parameters.AddWithValue("@Head", head_edit.Text ?? "");
                            sqlCommand2.Parameters.AddWithValue("@Text", text_edit.Text ?? "");
                            sqlCommand2.Parameters.AddWithValue("@KeyWords", keywords_edit.Text ?? "");
                            sqlCommand2.Parameters.AddWithValue("@SD", sd_edit.Text ?? "");
                            sqlCommand2.Parameters.AddWithValue("@WhoCreated", Session["FIO"] ?? "");
                            sqlCommand2.Parameters.AddWithValue("@WhenCreated", DateTime.Now.ToString() ?? "");
                            sqlCommand2.ExecuteNonQuery();
                        }
                        infoaboutserver.AddHistory("База знаний: Пост добавлен в " + Session["NameIT"]);
                    }
                    else
                    {
                        using (SqlCommand sqlCommand2 = new SqlCommand("UPDATE [Data] SET IT=@IT,isImportant=@isImportant,isMainPage=@isMainPage,isFirstLine=@isFirstLine,Head=@Head,Text=@Text,KeyWords=@KeyWords,SD=@SD,Who=@Who,DateTime=@DateTime WHERE Id=@_Id", sqlConnection))
                        {
                            sqlCommand2.Parameters.AddWithValue("@_Id", _id);
                            sqlCommand2.Parameters.AddWithValue("@IT", it_edit.Value ?? "");
                            sqlCommand2.Parameters.AddWithValue("@isImportant", (bool)isimportant_edit.Checked == true ? "1" : "0");
                            sqlCommand2.Parameters.AddWithValue("@isMainPage", ismainpage_edit.Checked == true ? "1" : "0");
                            sqlCommand2.Parameters.AddWithValue("@isFirstLine", isfirstline_edit.Checked == true ? "1" : "0");
                            sqlCommand2.Parameters.AddWithValue("@Head", head_edit.Text ?? "");
                            sqlCommand2.Parameters.AddWithValue("@Text", text_edit.Text ?? "");
                            sqlCommand2.Parameters.AddWithValue("@KeyWords", keywords_edit.Text ?? "");
                            sqlCommand2.Parameters.AddWithValue("@SD", sd_edit.Text ?? "");
                            sqlCommand2.Parameters.AddWithValue("@Who", Session["FIO"] ?? "");
                            sqlCommand2.Parameters.AddWithValue("@DateTime", DateTime.Now.ToString() ?? "");
                            sqlCommand2.ExecuteNonQuery();
                        }
                        infoaboutserver.AddHistory("База знаний: Пост изменен #" + _id + "/" + Session["NameIT"]);
                    }
                    sqlConnection.Close();
                }
                if (Session["NameIT"] != null)
                {
                    Session["PrevSelectedNode"] = Session["NameIT"];
                    Session["PrevPathNode"] = Session["PathNode"];
                }

                Response.Redirect(Request.RawUrl);
            }
            catch (Exception ex) { Response.Write(ex.ToString()); }
        }

        public List<FilesUploaded> ListFileUploaded = new List<FilesUploaded>();
        public class FilesUploaded
        {
            public string FileName { get; set; }
            public string PathFile { get; set; }
        }
        protected void UploadFile()
        {
            string forhistory = "";
            HttpFileCollection uploadedFiles = Request.Files;
            string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            if (FileUpload.HasFiles)
            {
                for (int i = 0; i < uploadedFiles.Count; i++)
                {
                    HttpPostedFile userPostedFile = uploadedFiles[i];
                    try
                    {
                        if (userPostedFile.ContentLength > 0)
                        {
                            String fileName = userPostedFile.FileName;
                            var FileName = fileName + " (" + Math.Round(userPostedFile.ContentLength * 0.000001, 2) + " MB)";
                            var file = new RandomGenerator().RandomPassword() + "_" + fileName.Replace(" ", "_");
                            var path = HttpRuntime.AppDomainAppPath + @"/Uploads/" + file;
                            var href = baseUrl + "/Uploads/" + file;
                            userPostedFile.SaveAs(path);
                            ListFileUploaded.Add(new FilesUploaded
                            {
                                FileName = FileName,
                                PathFile = href,
                            });
                            forhistory += FileName + "<br/>";
                        }
                    }
                    catch (Exception Ex)
                    {
                        Response.Write(Ex.ToString());
                    }
                }
                infoaboutserver.AddHistory("База знаний: файл загружен: " + forhistory);
            }
        }
        #endregion

        protected void linkbtnServerInfo_Click(object sender, EventArgs e)
        {
            Server.Transfer("infoaboutserver.aspx", true);
        }

        public string GetVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        protected void linkbtnEditAlerts_Click(object sender, EventArgs e)
        {
            Server.Transfer("alerts.aspx", true);
        }

        public string GetBirthDayToAlerts()
        {
            if (!IsPostBack)
            {
                List<employess.EmployeesClass> g = new List<employess.EmployeesClass>();
                using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                using (SqlCommand sqlCommand = new SqlCommand("SELECT * from Users", sqlConnection))
                {
                    sqlConnection.Open();
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            g.Add(new employess.EmployeesClass
                            {
                                FIO = (string)reader["FIO"],
                                BirthDate = (object)reader["BirthDate"]==DBNull.Value?"":(string)reader["BirthDate"]
                            });
                        }
                    }
                    sqlConnection.Close();
                }
                string bd = String.Empty;
                foreach (var s in g)
                {
                    var date = DateTime.TryParse(s.BirthDate, out DateTime result);
                    if (date)
                    {
                        if (result.Day == DateTime.Now.Day && result.Month == DateTime.Now.Month)
                        {
                            bd += "<img src=\"/images/birthday-min.gif\" height=\"15\">" + s.FIO + "<img src=\"/images/birthday-min.gif\" height=\"15\"><br/>";
                        }
                    }
                }
                if (bd != "")
                {
                    Page.ClientScript.RegisterStartupScript(
    GetType(),
    "MyKey",
    "document.getElementById('"+bdPanel.ClientID+"').hidden=false;",
    true);
                    bdPanel.Enabled = true;
                    bdPanel.Visible = true;
                    bd = bd.Remove(bd.Length - 5, 5);
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(
    GetType(),
    "MyKey",
    "document.getElementById('" + bdPanel.ClientID + "').hidden=true;",
    true);
                    bdPanel.Enabled = false;
                    bdPanel.Visible = false;
                }
                return bd;
            }
            return String.Empty;
        }

        public string ПолучитьСписокДолжников()
        {
            string списокдолжников = String.Empty;
            int i = 0;
            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
            using (SqlCommand sqlCommand = new SqlCommand("SELECT ФИО,Долг FROM Debtors", sqlConnection))
            {
                sqlConnection.Open();
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        double долг = (object)reader["Долг"] == DBNull.Value ? 0 : (double)reader["Долг"];
                        if (долг == 0 || долг==-1) continue;
                        string фио = (object)reader["ФИО"] == DBNull.Value ? "" : (string)reader["ФИО"];
                        i++;
                        списокдолжников += i+". "+фио + " " + долг + " р.</br>";
                    }
                }
                sqlConnection.Close();
            }
            var IsFirstRun = Session["IsFirstRun"];
            if (i > 0 && IsFirstRun!=null && (bool)IsFirstRun ==true && !IsPostBack)
            {
                Page.ClientScript.RegisterStartupScript(
      GetType(),
      "DebtorsFieldsStatus",
      "document.getElementById('" + ПанельСписокДолжников.ClientID + "').setAttribute('class','panel-debtors-notification');",
      true);
                Session["IsFirstRun"] = false;
            }
            return списокдолжников;
        }
    }

    public class RandomGenerator
    {
        private int RandomNumber(int min, int max)
        {
            return appRandom.Value.Next(min, max);
        }
        private static readonly ThreadLocal<Random> appRandom
     = new ThreadLocal<Random>(() => new Random());

        private string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * appRandom.Value.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
  
        public string RandomPassword()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(15, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(18, false));
            return builder.ToString();
        }
    }
}