using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Carcharoth
{
    public partial class infoaboutserver : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated && (int)Session["Level"] >= 10)
            {
                if (!IsPostBack)
                {
                   // GetInfoServer();
                }
            }
            else
            {
                Server.Transfer("403.html", true);
            }
        }

        private void GetInfoServer()
        {
            lblServerIP.Text = Request.ServerVariables["LOCAL_ADDR"];
            lblMachineName.Text = Environment.MachineName;
            try
            {
                ConnectionOptions co = new ConnectionOptions();
                co.Username = null;
                ManagementScope ms = new ManagementScope("\\\\" + "WebService" + "\\root\\CIMV2", co);
                ObjectQuery q = new ObjectQuery("select * from Win32_OperatingSystem");
                ManagementObjectSearcher os = new ManagementObjectSearcher(ms, q);
                ManagementObjectCollection moc = os.Get();
                UInt64 totalphisicalMemorySize = 0;
                UInt64 freephisicalMemorySize = 0;
                foreach (ManagementObject o in moc)
                {
                    totalphisicalMemorySize += Convert.ToUInt64(o["TotalVisibleMemorySize"], CultureInfo.InvariantCulture);
                    freephisicalMemorySize += Convert.ToUInt64(o["FreePhysicalMemory"], CultureInfo.InvariantCulture);
                }
                lblRAM.Text = ((double)((int)totalphisicalMemorySize - (int)freephisicalMemorySize) * 0.001) + "MB / " + ((double)((int)totalphisicalMemorySize * 0.001)) +
                    "MB, Usage: " + Math.Round(((double)((int)totalphisicalMemorySize - (int)freephisicalMemorySize) / (int)totalphisicalMemorySize), 2) * 100 + "%";
            }
            catch { }
            FillActiveUsersListView();
        }

        protected void RefreshInfo_Click(object sender, EventArgs e)
        {
            GetInfoServer();
        }

        private SqlConnection conn = new SqlConnection(index.CM);
        private void FillActiveUsersListView()
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("Select * from HistoryActivesUsers ORDER BY Id DESC", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            conn.Close();
            if (ds.Tables[0].Rows.Count > 0)
            {
                ActiveUsersListView.DataSource = ds;
                ActiveUsersListView.DataBind();
            }
        }

        protected void ActiveUsersListView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            FillActiveUsersListView();
            ActiveUsersListView.PageIndex = e.NewPageIndex;
            ActiveUsersListView.DataBind();
        }

        public static void AddHistory(string txt)
        {
            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
            using (SqlCommand sqlCommand2 = new SqlCommand("INSERT INTO [HistoryActivesUsers] (Login,FIO,History,DateTime,IP) " +
                    "VALUES (@Login,@FIO,@History,@DateTime,@IP)", sqlConnection))
            {
                sqlConnection.Open();
                sqlCommand2.Parameters.AddWithValue("@Login", (string)HttpContext.Current.Session["Login"] ?? "Guest");
                sqlCommand2.Parameters.AddWithValue("@FIO", (string)HttpContext.Current.Session["FIO"] ?? "-");
                sqlCommand2.Parameters.AddWithValue("@History", txt + "<br/>" + HttpContext.Current.Request.Url.AbsoluteUri ?? "NaN");
                sqlCommand2.Parameters.AddWithValue("@DateTime", DateTime.Now.ToString() ?? "NaN");
                sqlCommand2.Parameters.AddWithValue("@IP", HttpContext.Current.Request.UserHostAddress ?? "NaN");
                sqlCommand2.ExecuteNonQuery();
                sqlConnection.Close();
            }
        }
    }
}