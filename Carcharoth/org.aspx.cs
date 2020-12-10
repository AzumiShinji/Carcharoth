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
    public partial class org : System.Web.UI.Page
    {
        private static string scriptGetWidth = "$(document).ready(function getwidthwindow() {" +
               "$('#clientScreenWidth').val($(window).width());" +
               "});";

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, typeof(Page), Guid.NewGuid().ToString(), scriptGetWidth, true);
            if (!IsPostBack)
                GetInfoOrgs();
        }

        #region SearchOrgs
        protected void SearchOrgsBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, typeof(Page), Guid.NewGuid().ToString(), scriptGetWidth, true);
                int width = Convert.ToInt32(HttpContext.Current.Request.Params["clientScreenWidth"]);
                DataOrg.RepeatColumns = width / 400;
            }
            catch { }
            string code = SearchOrgsTextBox.Text.Trim();
            if(code!="")
            DataOrgsSqlDataSource.SelectCommand = "SELECT * FROM runbp WHERE inn = N'" + code + "'" +
                " OR code = N'" + code + "'" +
                " OR ogrn = N'" + code + "'" +
                " OR kpp = N'" + code + "'" +
                " OR okpoCode = N'" + code + "'" +
                " OR pgmu = N'" + code + "'"
                ;
            infoaboutserver.AddHistory("Поиск организации по коду: "+code);
        }

        protected void SearchOrgByNameBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, typeof(Page), Guid.NewGuid().ToString(), scriptGetWidth, true);
                int width = Convert.ToInt32(HttpContext.Current.Request.Params["clientScreenWidth"]);
                DataOrg.RepeatColumns = width / 400;
            }
            catch { }

            if (SearchOrgsTextBox.Text.Trim() != "")
            {
                var name = SearchOrgsTextBox.Text.Replace("\'","").Replace("\"","").Split();
                for (int i = 0; i < name.Count(); i++)
                {
                    if (name.Count() == 1)
                        name[i] =
                            "WHERE (fullName LIKE (N'%" + name[i] + "%')) OR (shortName LIKE (N'%" + name[i] + "%'))";
                    else
                    {
                        if (i == 0)
                            name[i] =
                                "WHERE (fullName LIKE (N'%" + name[i] + "%') OR (shortName LIKE (N'%" + name[i] + "%'))) AND ";
                        else
                        {
                            if (i == name.Count() - 1)
                                name[i] =
                                    "(fullName LIKE (N'%" + name[i] + "%') OR (shortName LIKE (N'%" + name[i] + "%')))";
                            else
                                name[i] =
                                    "(fullName LIKE (N'%" + name[i] + "%') OR (shortName LIKE (N'%" + name[i] + "%'))) AND ";
                        }
                    }
                }
                string resname = string.Join(" ", name);
                DataOrgsSqlDataSource.SelectCommand =
                        "SELECT TOP 100 * FROM runbp " + resname;
            }

            infoaboutserver.AddHistory("Поиск организации по имени: " + SearchOrgsTextBox.Text);
        }
        #endregion

        #region OrgsInfo
        public static string CMOrg = ConfigurationManager.ConnectionStrings["ToOrgsDB"].ConnectionString;
        public void GetInfoOrgs()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(CMOrg))
                using (SqlCommand sqlCommand = new SqlCommand("SELECT TOP 1 * FROM runbp ORDER BY Id DESC", sqlConnection))
                {
                    sqlConnection.Open();
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StatusOrg.Text = "online";
                            CountOrg.Text = (object)reader["inn"] == DBNull.Value ? "Нет данных" : (string)reader["inn"];
                            DateOrg.Text = (object)reader["code"] == DBNull.Value ? "Нет данных" : (string)reader["code"];
                        }
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex) {
                StatusOrg.Text = "Error "+ex.HResult;
                CountOrg.Text = "Нет данных";
                DateOrg.Text = "Нет данных";
            }
        }
        #endregion
    }
}