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
    public partial class games : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //gameslinkspnl
            foreach (var s in Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "games", "*.*"))
            {
                var name = s.Split('\\').LastOrDefault();
                var btn = new LinkButton();
                var image = new Image();
                var tb = new Label();
                tb.Text = name;
                image.ImageUrl = "images/game-btn.png";
                image.Attributes.Add("style", "padding:5px;");
                btn.Text = name;
                btn.Attributes.Add("href", "/games/" + name + "/index.html");
                btn.Attributes.Add("target", "_blank");
                btn.Attributes.Add("class", "btn btn-sm btn-outline-info game-button");
                btn.Controls.Add(image);
                btn.Controls.Add(tb);
                gameslinkspnl.Controls.Add(btn);
            }
        }
    }
}