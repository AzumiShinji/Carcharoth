using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Carcharoth
{
    public partial class catalog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SelectedTree(0);
                if (Session["PrevSelectedNode"] != null)
                {
                    SelectNodeByValue(CatalogTree.Nodes, Session["PrevSelectedNode"].ToString());
                    SelectedTree(0);
                    Session["PrevSelectedNode"] = null;
                }
            }
            GetKuratorProject();
        }
        protected void SelectNodeByValue(TreeNodeCollection Nodes, string ValueToSelect)
        {
            if (Session["PrevPathNode"]!=null && Session["PrevPathNode"].ToString().Contains("Решения на 1-ой линии"))
                foreach (TreeNode n in Nodes)
                {
                    if (n.ValuePath.Contains("Решения на 1-ой линии"))
                    {
                        if (n.Text == ValueToSelect)
                            n.Select();
                        else if (n.ChildNodes.Count > 0)
                            SelectNodeByValue(n.ChildNodes, ValueToSelect);
                        n.Expand();
                    }
                }
            else
                foreach (TreeNode n in Nodes)
                {
                    if (n.Text == ValueToSelect)
                        n.Select();
                    else if (n.ChildNodes.Count > 0)
                        SelectNodeByValue(n.ChildNodes, ValueToSelect);
                    if (!n.ValuePath.Contains("Решения на 1-ой линии"))
                    {
                        n.Expand();
                    }
                }
        }

        private void SetPageVariableValue(bool NextTurnOn, bool PrevTurnOn, int offset, int pagenum)
        {
            _NextTurnOn.Value = NextTurnOn.ToString();
            _PrevTurnOn.Value = PrevTurnOn.ToString();
            _offset.Value = offset.ToString();
            _pagenum.Value = pagenum.ToString();
        }

        private void SetSQLCommand(TreeNode tn, int offset, int fetch)
        {
            if (tn.Text == "Главная страница")
                DataTreeSQLSource.SelectCommand =
                "SELECT * FROM Data WHERE IsMainPage LIKE 1 ORDER BY isImportant DESC,Id DESC OFFSET " + offset + " ROWS FETCH NEXT " + fetch + " ROWS ONLY";
            else if (tn.ValuePath.Contains("Решения на 1-ой линии"))
                    DataTreeSQLSource.SelectCommand =
                    "SELECT * FROM Data WHERE isFirstLine like 1 AND IT LIKE N'" + tn.Text + "' ORDER BY isImportant DESC,Id DESC OFFSET " + offset + " ROWS FETCH NEXT " + fetch + " ROWS ONLY";
            else
                DataTreeSQLSource.SelectCommand =
                    "SELECT * FROM Data WHERE IT LIKE N'" + tn.Text + "' AND NOT isFirstLine like 1 ORDER BY isImportant DESC,Id DESC OFFSET " + offset + " ROWS FETCH NEXT " + fetch + " ROWS ONLY";
        }

        public void SelectedTree(int stat)
        {
            SearchLabelText.Visible = false;

            if (CatalogTree.SelectedNode.ValuePath.Contains("Решения на 1-ой линии"))
            {
                CurrentProjectText.Text = "Каталог: Решения на 1-ой линии";
                PanelCurrentProjectText.Visible = true;
                ImageCurrentProjectText.ImageUrl = "/images/catalog-tree-firstline_.png";
            }
            else if (CatalogTree.SelectedNode.ValuePath.Contains("Справочная информация"))
            {
                CurrentProjectText.Text = "Каталог: Справочная информация";
                PanelCurrentProjectText.Visible = true;
                ImageCurrentProjectText.ImageUrl = "/images/catalog-tree-reference.png";
            }
            else PanelCurrentProjectText.Visible = false;

            bool NextTurnOn = Convert.ToBoolean(_NextTurnOn.Value);
            bool PrevTurnOn = Convert.ToBoolean(_PrevTurnOn.Value);
            int offset = Convert.ToInt32(_offset.Value);
            int fetch = Convert.ToInt32(_fetch.Value);
            int pagenum = Convert.ToInt32(_pagenum.Value);

            var tn = CatalogTree.SelectedNode;
            var count = DataPostsCount(tn);
            var countpage = count / fetch;
            if (count % fetch > 0)
                countpage++;

            Task.Factory.StartNew(new Action(() =>
            {
                AccessITPosts(tn);
            }));

            Session["NameIT"] = tn.Text;
            Session["PathNode"] = tn.ValuePath;

            if (pagenum <= 1) { PrevPageBtn.Enabled = false; PrevTurnOn = false; PrevPageBtnTop.Enabled = PrevPageBtn.Enabled; }
            else { PrevPageBtn.Enabled = true; PrevTurnOn = true; PrevPageBtnTop.Enabled = PrevPageBtn.Enabled; }
            if (pagenum == countpage) { NextPageBtn.Enabled = false; NextTurnOn = false; NextPageBtnTop.Enabled = NextPageBtn.Enabled; }
            else { NextPageBtn.Enabled = true; NextTurnOn = true; NextPageBtnTop.Enabled = NextPageBtn.Enabled; }

            if (stat == 0)
            {
                SetPageVariableValue(false, false, 0, 0);
                NextTurnOn = Convert.ToBoolean(_NextTurnOn.Value);
                PrevTurnOn = Convert.ToBoolean(_PrevTurnOn.Value);
                offset = Convert.ToInt32(_offset.Value);
                fetch = Convert.ToInt32(_fetch.Value);
                pagenum = Convert.ToInt32(_pagenum.Value);
                PageNumLabel.Text = pagenum + "/" + countpage;
                PageNumLabelTop.Text = PageNumLabel.Text;
                if (NextPageBtn.Enabled && NextTurnOn)
                    offset = offset + fetch;
                SetSQLCommand(tn, offset, fetch);
                if (pagenum != countpage)
                    pagenum++;
                PageNumLabel.Text = pagenum + "/" + countpage;
                PageNumLabelTop.Text = PageNumLabel.Text;
                if (pagenum <= 1) { PrevPageBtn.Enabled = false; PrevTurnOn = false; PrevPageBtnTop.Enabled = PrevPageBtn.Enabled; }
                else { PrevPageBtn.Enabled = true; PrevTurnOn = true; PrevPageBtnTop.Enabled = PrevPageBtn.Enabled; }
                if (pagenum == countpage) { NextPageBtn.Enabled = false; NextTurnOn = false; NextPageBtnTop.Enabled = NextPageBtn.Enabled; }
                else { NextPageBtn.Enabled = true; NextTurnOn = true; NextPageBtnTop.Enabled = NextPageBtn.Enabled; }

            }
            else if (stat == 1)
            {
                if (NextPageBtn.Enabled && NextTurnOn)
                    offset = offset + fetch;
                SetSQLCommand(tn, offset, fetch);
                if (pagenum != countpage)
                    pagenum++;
                PageNumLabel.Text = pagenum + "/" + countpage;
                PageNumLabelTop.Text = PageNumLabel.Text;
                if (pagenum <= 1) { PrevPageBtn.Enabled = false; PrevTurnOn = false; PrevPageBtnTop.Enabled = PrevPageBtn.Enabled; }
                else { PrevPageBtn.Enabled = true; PrevTurnOn = true; PrevPageBtnTop.Enabled = PrevPageBtn.Enabled; }
                if (pagenum == countpage) { NextPageBtn.Enabled = false; NextTurnOn = false; NextPageBtnTop.Enabled = NextPageBtn.Enabled; }
                else { NextPageBtn.Enabled = true; NextTurnOn = true; NextPageBtnTop.Enabled = NextPageBtn.Enabled; }
            }
            else if (stat == -1)
            {
                if (PrevPageBtn.Enabled && PrevTurnOn)
                    offset = offset - fetch;
                SetSQLCommand(tn, offset, fetch);
                if (pagenum > 1)
                    pagenum--;
                PageNumLabel.Text = pagenum + "/" + countpage;
                PageNumLabelTop.Text = PageNumLabel.Text;
                if (pagenum <= 1) { PrevPageBtn.Enabled = false; PrevTurnOn = false; PrevPageBtnTop.Enabled = PrevPageBtn.Enabled; }
                else { PrevPageBtn.Enabled = true; PrevTurnOn = true; PrevPageBtnTop.Enabled = PrevPageBtn.Enabled; }
                if (pagenum == countpage) { NextPageBtn.Enabled = false; NextTurnOn = false; NextPageBtnTop.Enabled = NextPageBtn.Enabled; }
                else { NextPageBtn.Enabled = true; NextTurnOn = true; NextPageBtnTop.Enabled = NextPageBtn.Enabled; }
            }
            SetPageVariableValue(PrevTurnOn, NextTurnOn, offset, pagenum);
        }
        protected void CatalogTree_SelectedNodeChanged(object sender, EventArgs e)
        {
            SelectedTree(0);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (textboxSearch.Text.Trim() != "")
            {
                var text = textboxSearch.Text.Split();
                for (int i = 0; i < text.Count(); i++)
                {
                    if (text.Count() == 1)
                        text[i] =
                            "WHERE (IT LIKE (N'%" + text[i] + "%') " +
                            "OR Head LIKE (N'%" + text[i] + "%') " +
                            "OR Text LIKE (N'%" + text[i] + "%') " +
                            "OR KeyWords LIKE (N'%" + text[i] + "%') " +
                            "OR SD LIKE (N'%" + text[i] + "%') " +
                            "OR Who LIKE (N'%" + text[i] + "%') " +
                            "OR DateTime LIKE (N'%" + text[i] + "%'))";
                    else
                    {
                        if (i == 0)
                            text[i] =
                                "WHERE (IT LIKE (N'%" + text[i] + "%') " +
                                "OR Head LIKE (N'%" + text[i] + "%') " +
                                "OR Text LIKE (N'%" + text[i] + "%') " +
                                "OR KeyWords LIKE (N'%" + text[i] + "%') " +
                                "OR SD LIKE (N'%" + text[i] + "%') " +
                                "OR Who LIKE (N'%" + text[i] + "%') " +
                                "OR DateTime LIKE (N'%" + text[i] + "%')) AND ";
                        else
                        {
                            if (i == text.Count() - 1)
                                text[i] =
                                    "(IT LIKE (N'%" + text[i] + "%') " +
                                    "OR Head LIKE (N'%" + text[i] + "%') " +
                                    "OR Text LIKE (N'%" + text[i] + "%') " +
                                    "OR KeyWords LIKE (N'%" + text[i] + "%') " +
                                    "OR SD LIKE (N'%" + text[i] + "%') " +
                                    "OR Who LIKE (N'%" + text[i] + "%') " +
                                    "OR DateTime LIKE (N'%" + text[i] + "%'))";
                            else
                                text[i] =
                                    "(IT LIKE (N'%" + text[i] + "%') " +
                                    "OR Head LIKE (N'%" + text[i] + "%') " +
                                    "OR Text LIKE (N'%" + text[i] + "%') " +
                                    "OR KeyWords LIKE (N'%" + text[i] + "%') " +
                                    "OR SD LIKE (N'%" + text[i] + "%') " +
                                    "OR Who LIKE (N'%" + text[i] + "%') " +
                                    "OR DateTime LIKE (N'%" + text[i] + "%')) AND ";
                        }
                    }
                }
                string restext = string.Join(" ", text);
                DataTreeSQLSource.SelectCommand =
                        "SELECT TOP 100 * FROM Data " + restext + " ORDER BY Id DESC";
                textboxSearch.Focus();
                textboxSearch.Text = textboxSearch.Text;
                SearchLabelText.Visible = true;
                SearchLabelText.Text = "Поиск по запросу: " + textboxSearch.Text;
            }
            else SearchLabelText.Text = "Поиск по запросу: Нет данных, запрос не может быть пустым!";
        }

        protected void AddPost_Click(object sender, EventArgs e)
        {
            if (CatalogTree.SelectedNode.ValuePath.Contains("Общая информация") || CatalogTree.SelectedNode.ValuePath.Contains("Другие проекты"))
                ScriptManager.RegisterStartupScript(this, this.GetType()
                                                      , "clentscript2", "Sys.Application.add_load(function() { document.getElementById('it_edit').value = '" + CatalogTree.SelectedNode.Text + "'; document.getElementById('isfirstline_edit').disabled=true; });", true);
            else ScriptManager.RegisterStartupScript(this, this.GetType()
                                                      , "clentscript2", "Sys.Application.add_load(function() { document.getElementById('it_edit').value = '" + CatalogTree.SelectedNode.Text + "'; document.getElementById('isfirstline_edit').disabled=false; });", true);
            if (CatalogTree.SelectedNode.ValuePath.Contains("Решения на 1-ой линии"))
                ScriptManager.RegisterStartupScript(this, this.GetType()
                                                      , "clentscript2", "Sys.Application.add_load(function() { document.getElementById('it_edit').value = '" + CatalogTree.SelectedNode.Text + "'; document.getElementById('isfirstline_edit').checked=true; });", true);
            else ScriptManager.RegisterStartupScript(this, this.GetType()
                                                      , "clentscript2", "Sys.Application.add_load(function() { document.getElementById('it_edit').value = '" + CatalogTree.SelectedNode.Text + "'; document.getElementById('isfirstline_edit').checked=false;});", true);
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "clentscript2", "ShowPopupEA();", true);
        }
        public string ConvertFIO(string FIO)
        {
            try
            {
                var g = FIO.Split();
                string fam = g[0];
                string ab1 = g[1].Substring(0, 1) + ".";
                string ab2 = g[2].Substring(0, 1) + ".";

                string res = fam + " " + ab1 + ab2;
                return res;
            }
            catch { return FIO; }
        }

        protected void NextPageBtn_Click(object sender, EventArgs e)
        {
            SelectedTree(1);
        }

        protected void PrevPageBtn_Click(object sender, EventArgs e)
        {
            SelectedTree(-1);
        }

        public int DataPostsCount(TreeNode IT)
        {
            try
            {
                if (IT.Text == "Главная страница")
                    using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                    using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Data WHERE isMainPage LIKE 1", sqlConnection))
                    {
                        sqlConnection.Open();
                        int count = (int)sqlCommand.ExecuteScalar();
                        sqlConnection.Close();
                        return count;
                    }
                else if (IT.ValuePath.Contains("Решения на 1-ой линии"))
                    using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                    using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Data WHERE isFirstLine LIKE 1 AND IT LIKE @IT", sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@IT", IT.Text);
                        sqlConnection.Open();
                        int count = (int)sqlCommand.ExecuteScalar();
                        sqlConnection.Close();
                        return count;
                    }
                else
                    using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                    using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Data WHERE IT LIKE @IT AND NOT isFirstLine like 1", sqlConnection))
                    {
                        sqlConnection.Open();
                        sqlCommand.Parameters.AddWithValue("@IT", IT.Text);
                        int count = (int)sqlCommand.ExecuteScalar();
                        sqlConnection.Close();
                        return count;
                    }
            }
            catch (Exception ex) { Response.Write(ex.ToString()); return 0; }
        }

        public bool AccessITPosts(TreeNode tn)
        {
            bool ResultAccess = false;
            if (Request.IsAuthenticated)
            {
                if (Session["Projects"] == null)
                    Response.Redirect("/catalog.aspx");
                foreach (var s in Session["Projects"] as List<index.ProjectField>)
                    if (((tn.Text.StartsWith(s.Name) && s.Status == 1) ||
                        (s.Name == "Все проекты" && s.Status == 1) ||
                        (s.Status == 1 && (tn.Text == "Другие проекты" || tn.Text == "Общая информация")))
                        && CatalogTree.SelectedNode.Value != "Главная страница" && s.Status == 1)
                    { ResultAccess = true; }
                    else if (tn.Parent != null)
                    {
                        if ((tn.Parent.Text.StartsWith(s.Name) && s.Status == 1) || (tn.Parent.Text == "Другие проекты" && s.Status == 1))
                            ResultAccess = true;
                    }
                       
                if (ResultAccess)
                {
                    AddPost.Enabled = true;
                    AddPost.Visible = true;
                    SelectedNodeReplacePosts.Enabled = false;
                    SelectedNodeReplacePosts.Visible = false;
                }
                else
                {
                    AddPost.Enabled = false;
                    AddPost.Visible = false;
                    SelectedNodeReplacePosts.Enabled = true;
                    SelectedNodeReplacePosts.Visible = true;
                }
            }
            else
            {
                AddPost.Enabled = false;
                AddPost.Visible = false;
                SelectedNodeReplacePosts.Enabled = true;
                SelectedNodeReplacePosts.Visible = true;
            }
            return ResultAccess;
        }

        public void GetKuratorProject()
        {
            KuratorsProject.Text = String.Empty;
            if (CatalogTree.SelectedNode.Text != "Главная страница"
                && CatalogTree.SelectedNode.Text != "Общая информация"
                && CatalogTree.SelectedNode.Text != "Другие проекты")
            {
                PanelKuratorsProject.Visible = true;
                using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                using (SqlCommand sqlCommand = new SqlCommand("SELECT * from UsersCarcharoth", sqlConnection))
                {
                    sqlConnection.Open();
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                            if (reader != null && (int)reader["Level"] != 0)
                            {
                                if ((int)reader["ProjectAll"] == 1)
                                    ListKuratorProjectClass.Add(new KuratorProjectClass
                                    {
                                        FIO = (string)reader["FIO"],
                                        Project = "Все проекты",
                                    });
                                if ((int)reader["ProjectDSFK"] == 1)
                                    ListKuratorProjectClass.Add(new KuratorProjectClass
                                    {
                                        FIO = (string)reader["FIO"],
                                        Project = "ДС ФК",
                                    });
                                if ((int)reader["ProjectEB"] == 1)
                                    ListKuratorProjectClass.Add(new KuratorProjectClass
                                    {
                                        FIO = (string)reader["FIO"],
                                        Project = "ЭБ",
                                    });
                                if ((int)reader["ProjectGASU"] == 1)
                                    ListKuratorProjectClass.Add(new KuratorProjectClass
                                    {
                                        FIO = (string)reader["FIO"],
                                        Project = "ГАСУ",
                                    });
                                if ((int)reader["ProjectGISGMP"] == 1)
                                    ListKuratorProjectClass.Add(new KuratorProjectClass
                                    {
                                        FIO = (string)reader["FIO"],
                                        Project = "ГИС ГМП",
                                    });
                                if ((int)reader["ProjectGISGMU"] == 1)
                                    ListKuratorProjectClass.Add(new KuratorProjectClass
                                    {
                                        FIO = (string)reader["FIO"],
                                        Project = "ГИС ГМУ",
                                    });
                                if ((int)reader["ProjectSUFD"] == 1)
                                    ListKuratorProjectClass.Add(new KuratorProjectClass
                                    {
                                        FIO = (string)reader["FIO"],
                                        Project = "СУФД",
                                    });
                                if ((int)reader["ProjectOneC"] == 1)
                                    ListKuratorProjectClass.Add(new KuratorProjectClass
                                    {
                                        FIO = (string)reader["FIO"],
                                        Project = "1С",
                                    });
                                if ((int)reader["ProjectKS"] == 1)
                                    ListKuratorProjectClass.Add(new KuratorProjectClass
                                    {
                                        FIO = (string)reader["FIO"],
                                        Project = "Казначейское сопровождение",
                                    });
                                if ((int)reader["ProjectMI"] == 1)
                                    ListKuratorProjectClass.Add(new KuratorProjectClass
                                    {
                                        FIO = (string)reader["FIO"],
                                        Project = "Менеджер инцидентов",
                                    });
                                if ((int)reader["ProjectUC"] == 1)
                                    ListKuratorProjectClass.Add(new KuratorProjectClass
                                    {
                                        FIO = (string)reader["FIO"],
                                        Project = "Удостоверяющий центр",
                                    });
                                if ((int)reader["ProjectUD"] == 1)
                                    ListKuratorProjectClass.Add(new KuratorProjectClass
                                    {
                                        FIO = (string)reader["FIO"],
                                        Project = "Управление делами",
                                    });
                            }
                    }
                    string kurators = "";
                    foreach (var s in ListKuratorProjectClass)
                    {
                        if (CatalogTree.SelectedNode.Text.StartsWith(s.Project))
                        {
                            kurators += s.FIO + ", ";
                        }
                        else if (CatalogTree.SelectedNode.Parent != null && CatalogTree.SelectedNode.Parent.Text.StartsWith(s.Project))
                        {
                            kurators += s.FIO + ", ";
                        }
                    }
                    if (kurators == "") kurators = "Отсутствуют ,";
                    KuratorsProject.Text = kurators.Substring(0, kurators.Length - 2);
                }
            } else PanelKuratorsProject.Visible = false;
        }

        public class KuratorProjectClass
        {
            public string FIO { get; set; }
            public string Project { get; set; }
        }
        public List<KuratorProjectClass> ListKuratorProjectClass = new List<KuratorProjectClass>();
    }
}