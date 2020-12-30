using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Carcharoth
{
    public partial class ctr : System.Web.UI.Page
    {
        private string NameExeFile = "CatalogNSI";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (File.Exists(Server.MapPath("~/ctr/catalog.xlsm")) || File.Exists(Server.MapPath("~/ctr/tel_central_apparat.xls")))
            {
                InfoAboutEXE.Text = "Процесс уже запущен, ожидайте...";
                CatalogUpload.Enabled = false;
                UploadCatalogBtn.Enabled = false;
            }
            else
            {
                var prc = Process.GetProcessesByName(NameExeFile);
                if (prc.Count() > 0)
                {
                    InfoAboutEXE.Text = "Готово";
                    CatalogUpload.Enabled = true;
                    UploadCatalogBtn.Enabled = true;
                }
                else
                {
                    InfoAboutEXE.Text = "Требуемый процесс не запущен, обратитесь к администратору.";
                    CatalogUpload.Enabled = false;
                    UploadCatalogBtn.Enabled = false;
                }
            }

            if (Request.IsAuthenticated)
            {
                if (Session["Level"] == null)
                    Response.Redirect("/catalog.aspx");
                else
                {
                    if ((int)Session["Level"] >= 1 && 
                        (((string)Session["CurrentProject"]).Contains("ДС ФК") 
                        || ((string)Session["CurrentProject"]).Contains("Все проекты")))
                    {
                        PanelCCACCITSSQLSUpdate.Visible = true;
                        PanelCCACCITSSQLSUpdate.Enabled = true;
                    }
                }
            }
            else
            {
                PanelCCACCITSSQLSUpdate.Visible = false;
                PanelCCACCITSSQLSUpdate.Enabled = false;
            }
        }

        protected void UploadCatalogBtn_Click(object sender, EventArgs e)
        {
            HttpFileCollection uploadedFiles = Request.Files;
            if (CatalogUpload.HasFiles)
            {
                for (int i = 0; i < uploadedFiles.Count; i++)
                {
                    HttpPostedFile userPostedFile = uploadedFiles[i];
                    try
                    {
                        if (userPostedFile.ContentLength > 0)
                        {
                            String fileName = userPostedFile.FileName;
                            string newfilename = "";
                            if (fileName.ToLower().Contains("каталог"))
                            {
                                newfilename = "catalog.xlsm";
                            }
                            else if(fileName.ToLower().Contains("tel_central_apparat"))
                            {
                                newfilename = "tel_central_apparat.xls";
                            }
                            else if(fileName.ToLower().Contains("tel_ufk"))
                            {
                                newfilename = "tel_UFK.xls";
                            }
                            else newfilename = "garbadge.gg";
                            var path = HttpRuntime.AppDomainAppPath + @"/ctr/" + newfilename;
                            userPostedFile.SaveAs(path);
                        }
                    }
                    catch (Exception Ex)
                    {
                        Response.Write(Ex.ToString());
                    }
                }
            }
            else
            {
                InfoAboutEXE.Text = "Файлы не выбраны!";
            }
            InfoAboutEXE.Text = "Это займет некоторое время...";
            infoaboutserver.AddHistory("Загружен файл для CTR");
            CatalogUpload.Enabled = false;
            UploadCatalogBtn.Enabled = false;
        }

        protected void SearchCCACCITSSQLSBtn_Click(object sender, EventArgs e)
        {
            DataCCACCITSSQLS.Enabled = true;
            DataCCACCITSSQLS.Visible = true;
            Datatel.Enabled = false;
            Datatel.Visible = false;
            if (TextBoxSearchCCACCITSSQLS.Text.Trim() != "")
            {
                var text = TextBoxSearchCCACCITSSQLS.Text.Split();
                for (int i = 0; i < text.Count(); i++)
                {
                    if (text.Count() == 1)
                        text[i] =
                            "WHERE (Groups LIKE (N'%" + text[i] + "%') " +
                            "OR [Services] LIKE (N'%" + text[i] + "%')) ";
                    else
                    {
                        if (i == 0)
                            text[i] =
                                "WHERE ([Groups] LIKE (N'%" + text[i] + "%') " +
                            "OR [Services] LIKE (N'%" + text[i] + "%')) AND ";
                        else
                        {
                            if (i == text.Count() - 1)
                                text[i] =
                                    "([Groups] LIKE (N'%" + text[i] + "%') " +
                                    "OR [Services] LIKE (N'%" + text[i] + "%'))";
                            else
                                text[i] =
                                    "([Groups] LIKE (N'%" + text[i] + "%') " +
                                    "OR [Services] LIKE (N'%" + text[i] + "%')) AND ";
                        }
                    }
                }
                string restext = string.Join(" ", text);
                DataCCACCITSSQLSource.SelectCommand =
                        "SELECT * FROM ctr " + restext;
                TextBoxSearchCCACCITSSQLS.Focus();
                TextBoxSearchCCACCITSSQLS.Text = TextBoxSearchCCACCITSSQLS.Text;
               //infoaboutserver.AddHistory("CTR поиск: "+ TextBoxSearchCCACCITSSQLS.Text);
            }
            else { DataCCACCITSSQLSource.SelectCommand = "SELECT * FROM ctr"; infoaboutserver.AddHistory("CTR поиск: Весь список каталога "); }
        }

        protected void SearchtelBtn_Click(object sender, EventArgs e)
        {
            DataCCACCITSSQLS.Enabled = false;
            DataCCACCITSSQLS.Visible = false;
            Datatel.Enabled = true;
            Datatel.Visible = true;

            LinkButton linkButton = (LinkButton)sender;
            if (linkButton.ID == "DivisionSearch" || linkButton.ID == "DepartmentSearch")
            {
                TextBoxSearchCCACCITSSQLS.Text = linkButton.Text;
            }

            if (TextBoxSearchCCACCITSSQLS.Text.Trim() != "")
            {
                var text = TextBoxSearchCCACCITSSQLS.Text.Split();
                for (int i = 0; i < text.Count(); i++)
                {
                    if (text.Count() == 1)
                        text[i] =
                            "WHERE (FIO LIKE (N'%" + text[i] + "%') " +
                            "OR Phone_City LIKE (N'%" + text[i] + "%') " +
                            "OR Phone_Inside LIKE (N'%" + text[i] + "%') " +
                            "OR Email LIKE (N'%" + text[i] + "%') " +
                            "OR Division LIKE (N'%" + text[i] + "%') " +
                            "OR Department LIKE (N'%" + text[i] + "%')) ";
                    else
                    {
                        if (i == 0)
                            text[i] =
                                "WHERE (FIO LIKE (N'%" + text[i] + "%') " +
                                "OR Phone_City LIKE (N'%" + text[i] + "%') " +
                                "OR Phone_Inside LIKE (N'%" + text[i] + "%') " +
                                "OR Email LIKE (N'%" + text[i] + "%') " +
                                "OR Division LIKE (N'%" + text[i] + "%') " +
                                "OR Department LIKE (N'%" + text[i] + "%')) AND ";
                        else
                        {
                            if (i == text.Count() - 1)
                                text[i] =
                                    "([FIO] LIKE (N'%" + text[i] + "%') " +
                                    "OR Phone_City LIKE (N'%" + text[i] + "%') " +
                                    "OR Phone_Inside LIKE (N'%" + text[i] + "%') " +
                                    "OR [Email] LIKE (N'%" + text[i] + "%')" +
                                    "OR [Division] LIKE (N'%" + text[i] + "%') " +
                                    "OR [Department] LIKE (N'%" + text[i] + "%')) ";
                            else
                                text[i] =
                                    "([FIO] LIKE (N'%" + text[i] + "%') " +
                                    "OR Phone_City LIKE (N'%" + text[i] + "%') " +
                                    "OR Phone_Inside LIKE (N'%" + text[i] + "%') " +
                                    "OR [Email] LIKE (N'%" + text[i] + "%') " +
                                    "OR [Division] LIKE (N'%" + text[i] + "%') " +
                                    "OR [Department] LIKE (N'%" + text[i] + "%')) AND ";
                        }
                    }
                }
                string restext = string.Join(" ", text);
                DataCCACCITSSQLSource.SelectCommand =
                        "SELECT TOP 100 * FROM ctr_tel " + restext + "ORDER BY ID DESC";
                TextBoxSearchCCACCITSSQLS.Focus();
                TextBoxSearchCCACCITSSQLS.Text = TextBoxSearchCCACCITSSQLS.Text;
               // infoaboutserver.AddHistory("CTR поиск: " + TextBoxSearchCCACCITSSQLS.Text);
            }
        }
    }
}