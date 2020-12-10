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
    public partial class alerts : System.Web.UI.Page
    {
        protected List<AlertsFields> ListAlerts = new List<AlertsFields>();
        protected class AlertsFields
        {
            public int ID { get; set; }
            public string AlertsText { get; set; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated && (int)Session["Level"] >= 10)
            {
                if (!IsPostBack)
                {
                    GetData();
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
            using (SqlCommand sqlCommand = new SqlCommand("SELECT * from Alerts", sqlConnection))
            {
                sqlConnection.Open();
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ListAlerts.Add(new AlertsFields
                        {
                            ID = (int)reader["Id"],
                            AlertsText = (string)reader["AlertsText"],
                        });
                    }
                }
                sqlConnection.Close();
            }
            AlertsGrid.DataSource = ListAlerts;
            AlertsGrid.DataBind();
        }

        protected void AlertsGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "DeleteAlerts")
                {
                    string _Id = e.CommandArgument.ToString();
                    using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                    using (SqlCommand sqlCommand = new SqlCommand("DELETE FROM Alerts WHERE Id like @_Id", sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@_Id", _Id);
                        sqlConnection.Open();
                        sqlCommand.ExecuteScalar();
                        sqlConnection.Close();
                        StatusText.Text = "Оповещение №: " + _Id + " - удалено";
                        ListAlerts.Clear();
                        GetData();
                        infoaboutserver.AddHistory("Оповещение удалено: "+ StatusText.Text);
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
                    foreach (GridViewRow row in AlertsGrid.Rows)
                    {

                        string AlertsText = String.Empty;

                        int Id = Convert.ToInt32(AlertsGrid.Rows[row.RowIndex].Cells[0].Text);

                        TextBox _AlertsText = (TextBox)row.FindControl("AlertsText");
                        if (_AlertsText.Text != String.Empty)
                            AlertsText = _AlertsText.Text;
                        else { StatusText.Text = "Не может быть пустым"; return; }

                        using (SqlCommand sqlCommand = new SqlCommand("UPDATE Alerts SET AlertsText=@AlertsText WHERE Id=@Id", sqlConnection))
                        {
                            sqlConnection.Open();
                            sqlCommand.Parameters.AddWithValue("@AlertsText", AlertsText);
                            sqlCommand.Parameters.AddWithValue("@Id", Id);
                            sqlCommand.ExecuteNonQuery();
                            sqlConnection.Close();
                        }
                    }
                    ListAlerts.Clear();
                    GetData();
                    StatusText.Text = "Таблица успешно обновлена";
                    infoaboutserver.AddHistory("Таблица оповещений обновлена");
                }
            }
            catch (Exception ex) { Response.Write(ex.Message); StatusText.Text = ex.Message; }
        }

        protected void BtnAddAlerts_Click(object sender, EventArgs e)
        {
            string AlertsText = String.Empty;

            if (tbAlerts.Text != String.Empty)
                AlertsText = tbAlerts.Text;
            else { StatusText.Text = "Поле не может быть пустым"; return; }

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                using (SqlCommand sqlCommand2 = new SqlCommand("INSERT INTO Alerts ( AlertsText ) VALUES(@AlertsText) ", sqlConnection))
                {
                    sqlCommand2.Parameters.AddWithValue("@AlertsText", AlertsText);
                    sqlConnection.Open();
                    sqlCommand2.ExecuteNonQuery();
                    sqlConnection.Close();
                    StatusText.Text = "Оповещение успешно добавлено";
                    ListAlerts.Clear();
                    GetData();
                    infoaboutserver.AddHistory("Добавлено оповещение!");
                }
            }
            catch (Exception ex) { Response.Write(ex.Message); }
        }
    }
}