using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Carcharoth
{
    public partial class stat : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region Stat
        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
        protected void ShowStats_Click(object sender, EventArgs e)
        {
            InfoTablo.Visible = false;
            ProjectsList.Clear();
            StatsUsersList.Clear();
            EmployeesList.Clear();
            AllStatsUsersList.Clear();
            int AllCreatedContacts = 0,
                AllTResolved = 0,
                AllOrgCreated = 0;

            int ProjectDSFK = 0,
                ProjectGASU = 0,
                ProjectGISGMP = 0,
                ProjectGISGMU = 0,
                ProjectEB = 0,
                ProjectSUFD = 0,
                Project1C = 0,
                ProjectKS = 0,
                ProjectUC = 0,
                ProjectUD=0,
                ProjectShift = 0,
                ProjectUnivers = 0,
                ProjectUnchange = 0;

        var dateStart = DatePickerStatsStart.SelectedDate;
            if (dateStart.ToString() == "01.01.0001 0:00:00")
            {
                InfoTablo.Visible = true;
                InfoTablo.Text = "Внимание! Не выбрана дата!";
                return;
            }
            var dateEnd = DatePickerStatsEnd.SelectedDate;
            if (dateEnd.ToString() == "01.01.0001 0:00:00")
                dateEnd = DateTime.Today;
            ShowCalendarEnd.Text = "По это число(включительно): " + dateEnd.AddHours(23).AddMinutes(59).AddSeconds(59);
            if(dateStart>dateEnd)
            {
                InfoTablo.Visible = true;
                InfoTablo.Text = "Внимание! Неправильно выбран диапазон дат! Первое не может быть больше второго.";
                return;
            }

            #region StatsUsers
            GetEmployees();
            int
                SummaIMAll = 0,
                SummaSDResolved = 0;
            foreach (var user in EmployeesList)
            {
                int SDPhone = 0,
                    SDOther = 0,
                    SDAll = 0,
                    SDResolved = 0,
                    IMAll = 0,
                    IMWrong = 0,
                    IMResolved = 0,
                    CreatedContacts = 0,
                    TResolved = 0,
                    OrgCreated = 0;

                foreach (DateTime day in EachDay(dateStart, dateEnd))
                    using (SqlConnection sqlConnection = new SqlConnection(index.CM))
                    using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM ExecutedUsers WHERE PointDate like '" + day.Date.ToString() + "' AND Email like '" + user.Email + "' ORDER BY ID", sqlConnection))
                    {
                        sqlConnection.Open();
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SDPhone += (int)reader["SDPhone"];
                                SDOther += (int)reader["SDOther"];
                                SDAll += (int)reader["SDAll"];
                                SDResolved += (int)reader["SDResolved"];
                                IMAll += (int)reader["IMAll"];
                                IMWrong += (int)reader["IMWrong"];
                                IMResolved += (int)reader["IMResolved"];
                                CreatedContacts += (int)reader["CreatedContacts"];
                                TResolved += (int)reader["TResolved"];
                                OrgCreated += (int)reader["OrgCreated"];
                            }
                        }
                        sqlConnection.Close();
                    }

                AllCreatedContacts += CreatedContacts;
                AllTResolved += TResolved;
                AllOrgCreated += OrgCreated;
                SummaIMAll += IMAll;
                SummaSDResolved += SDResolved;

                StatsUsersList.Add(new StatsUsersFields
                {
                    Direction=user.Direction,
                    Email = user.Email,
                    FIO = user.FIO,
                    SDPhone = SDPhone,
                    SDOther = SDOther,
                    SDAll = SDAll,
                    SDResolved = SDResolved,
                    IMAll = IMAll,
                    IMWrong = IMWrong,
                    IMResolved = IMResolved,
                    CreatedContacts = CreatedContacts,
                    TResolved = TResolved,
                    OrgCreated = OrgCreated,
                });
                var count = user.Direction.Split('/').Count();
                //UsersProjects
                if (user.Direction.Contains("ДС ФК"))
                    ProjectDSFK+=(SDResolved + IMAll + TResolved) / count;
                if (user.Direction.Contains("ГАСУ"))
                    ProjectGASU += (SDResolved + IMAll + TResolved) / count;
                if (user.Direction.Contains("ГИС ГМП"))
                    ProjectGISGMP += (SDResolved + IMAll + TResolved) / count;
                if (user.Direction.Contains("ГИС ГМУ"))
                    ProjectGISGMU += (SDResolved + IMAll + TResolved) / count;
                if (user.Direction.Contains("ЭБ"))
                    ProjectEB += (SDResolved + IMAll + TResolved) / count;
                if (user.Direction.Contains("СУФД"))
                    ProjectSUFD += (SDResolved + IMAll + TResolved) / count;
                if (user.Direction.Contains("1С"))
                    Project1C += (SDResolved + IMAll + TResolved) / count;
                if (user.Direction.Contains("КС"))
                    ProjectKS += (SDResolved + IMAll + TResolved) / count;
                if (user.Direction.Contains("УЦ"))
                    ProjectUC += (SDResolved + IMAll + TResolved) / count;
                if (user.Direction.Contains("Управление делами"))
                    ProjectUD += (SDResolved + IMAll + TResolved) / count;
                if (user.Direction.Contains("Сменщики"))
                    ProjectShift += (SDResolved + IMAll + TResolved) / count;
                if (user.Direction.Contains("Универсалы"))
                    ProjectUnivers += (SDResolved + IMAll + TResolved) / count;
                if (user.Direction.Contains("Не определено"))
                    ProjectUnchange += (SDResolved + IMAll + TResolved) / count;
                //
                //top5
                Top5List.Add(new Top5
                {
                    Email = user.Email,
                    FIO = user.FIO,
                    Value = SDResolved + IMAll + TResolved,
                });
            }
            StatUsers.DataSource = StatsUsersList.OrderByDescending(x=>x.Direction!= "Не определено").ThenByDescending(x=>x.Direction).ThenBy(x=>x.Id);
            StatUsers.DataBind();

            AllStatsUsersList.Add(new AllStatsUsersFields
            {
                AllIM = SummaIMAll,
                AllResolved= SummaSDResolved,
                AllCreatedContacts = AllCreatedContacts,
                AllTResolved = AllTResolved,
                AllOrgCreated = AllOrgCreated,
                ProjectDSFK=ProjectDSFK,
                ProjectGASU= ProjectGASU,
                ProjectGISGMP= ProjectGISGMP,
                ProjectGISGMU= ProjectGISGMU,
                ProjectEB = ProjectEB,
                Project1C = Project1C,
                ProjectKS= ProjectKS,
                ProjectUC= ProjectUC,
                ProjectSUFD=ProjectSUFD,
                ProjectUD = ProjectUD,
                ProjectShift =ProjectShift,
                ProjectUnivers = ProjectUnivers,
                ProjectUnchange =ProjectUnchange,
            });
            AllStatsUsersListView.DataSource = AllStatsUsersList;
            AllStatsUsersListView.DataBind();
            #endregion
            #region top5
            var top5res = Top5List.OrderByDescending(x => x.Value).Take(10);
            Top5View.DataSource = top5res;
            Top5View.DataBind();
            var top5resdes = Top5List.Where(x => x.Value > 5).ToList().OrderBy(x => x.Value).Take(5);
            Top5ViewDes.DataSource = top5resdes;
            Top5ViewDes.DataBind();
            #endregion
            infoaboutserver.AddHistory("Загрузил статистику с: "+ dateStart+" По: "+ dateEnd);
        }
        protected void ShowCalendarStart_Click(object sender, EventArgs e)
        {
            DatePickerStatsEnd_Panel.Visible = false;
            DatePickerStatsStart_Panel.Visible = true;
        }
        protected void ShowCalendarEnd_Click(object sender, EventArgs e)
        {
            DatePickerStatsStart_Panel.Visible = false;
            DatePickerStatsEnd_Panel.Visible = true;
        }
        protected void DatePickerStatsStart_SelectionChanged(object sender, EventArgs e)
        {
            DatePickerStatsEnd_Panel.Visible = false;
            DatePickerStatsStart_Panel.Visible = false;
            ShowCalendarStart.Text = "C этого числа(включительно): " + DatePickerStatsStart.SelectedDate;
        }
        protected void DatePickerStatsEnd_SelectionChanged(object sender, EventArgs e)
        {
            DatePickerStatsEnd_Panel.Visible = false;
            DatePickerStatsStart_Panel.Visible = false;
            ShowCalendarEnd.Text = "По это число(включительно): " + DatePickerStatsEnd.SelectedDate.AddHours(23).AddMinutes(59).AddSeconds(59);
        }
        protected void btn_hide_DatePickerStatsStart_Click(object sender, EventArgs e)
        {
            DatePickerStatsStart_Panel.Visible = false;
        }

        protected void btn_hide_DatePickerStatsEnd_Click(object sender, EventArgs e)
        {
            DatePickerStatsEnd_Panel.Visible = false;
        }
        private void GetEmployees()
        {
            using (SqlConnection sqlConnection = new SqlConnection(index.CM))
            using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Users", sqlConnection))
            {
                sqlConnection.Open();
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        EmployeesList.Add(new EmployeesFields
                        {
                            Email = (string)reader["Email"],
                            FIO = (string)reader["FIO"],
                            Direction=(string)reader["Direction"],
                        });
                    }
                }
                sqlConnection.Close();
            }
        }
        public void ExportCSV()
        {
            try
            {
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=Carcharoth_export.xls");
                Response.ContentType = "text/csv";
                Response.AddHeader("Pragma", "public");
                Response.ContentEncoding = Encoding.UTF8;
                Response.BinaryWrite(Encoding.UTF8.GetPreamble());
                Response.Charset = "";
                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        AllStatsUsersListView.RenderControl(hw);
                    }
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        StatUsers.RenderControl(hw);
                    }
                    Response.Write(sw.ToString());
                }
                Response.End();
            }
            catch (Exception ex)
            {
                //catching exception
            }
        }
        protected void ExportData_Click(object sender, EventArgs e)
        {
            ExportCSV();
        }
        #endregion

        #region StatFieldsAndLists
        public class ProjectsFields
        {
            public int Id { get; set; }
            public int AllMailDSFK { get; set; }
            public int AllMailGASU { get; set; }
            public int AllMailGISGMP { get; set; }
            public int AllMailGISGMU { get; set; }
            public int AllMailEB { get; set; }
            public int AllMailKS { get; set; }
            public int AllMailOneC { get; set; }
            public int AllMailSUFD { get; set; }
            public int AllMailBUFOIV { get; set; }
            public int AllPhoneDSFK { get; set; }
            public int AllPhoneGASU { get; set; }
            public int AllPhoneGISGMP { get; set; }
            public int AllPhoneGISGMU { get; set; }
            public int AllPhoneEB { get; set; }
            public int AllPhoneKS { get; set; }
            public int AllPhoneOneC { get; set; }
            public int AllPhoneSUFD { get; set; }
            public int AllPhoneBUFOIV { get; set; }
            public int AllResolvedDSFK { get; set; }
            public int AllResolvedGASU { get; set; }
            public int AllResolvedGISGMP { get; set; }
            public int AllResolvedGISGMU { get; set; }
            public int AllResolvedEB { get; set; }
            public int AllResolvedKS { get; set; }
            public int AllResolvedOneC { get; set; }
            public int AllResolvedSUFD { get; set; }
            public int AllResolvedBUFOIV { get; set; }
            public int AllIMDSFK { get; set; }
            public int AllIMGASU { get; set; }
            public int AllIMGISGMP { get; set; }
            public int AllIMGISGMU { get; set; }
            public int AllIMEB { get; set; }
            public int AllIMKS { get; set; }
            public int AllIMOneC { get; set; }
            public int AllIMSUFD { get; set; }
            public int AllIMBUFOIV { get; set; }
            public int AllCompletedDSFK { get; set; }
            public int AllCompletedGASU { get; set; }
            public int AllCompletedGISGMP { get; set; }
            public int AllCompletedGISGMU { get; set; }
            public int AllCompletedEB { get; set; }
            public int AllCompletedKS { get; set; }
            public int AllCompletedOneC { get; set; }
            public int AllCompletedSUFD { get; set; }
            public int AllCompletedBUFOIV { get; set; }
            public int AllIMAll { get; set; }
            public int AllResolvedAll { get; set; }
        }
        public class StatsUsersFields
        {
            public int Id { get; set; }
            public string Direction { get; set; }
            public string Email { get; set; }
            public string FIO { get; set; }
            public int SDPhone { get; set; }
            public int SDOther { get; set; }
            public int SDAll { get; set; }
            public int SDResolved { get; set; }
            public int IMAll { get; set; }
            public int IMWrong { get; set; }
            public int IMResolved { get; set; }
            public int CreatedContacts { get; set; }
            public int TResolved { get; set; }
            public int OrgCreated { get; set; }
        }
        public class EmployeesFields
        {
            public string FIO { get; set; }
            public string Email { get; set; }
            public string Direction { get; set; }
        }
        public class AllStatsUsersFields
        {
            public int AllIM { get; set; }
            public int AllResolved { get; set; }
            public int AllCreatedContacts { get; set; }
            public int AllTResolved { get; set; }
            public int AllOrgCreated { get; set; }
            public int ProjectDSFK { get; set; }
            public int ProjectGASU { get; set; }
            public int ProjectGISGMP { get; set; }
            public int ProjectGISGMU { get; set; }
            public int ProjectEB { get; set; }
            public int ProjectSUFD { get; set; }
            public int Project1C { get; set; }
            public int ProjectKS { get; set; }
            public int ProjectUC { get; set; }
            public int ProjectUD { get; set; }
            public int ProjectShift { get; set; }
            public int ProjectUnivers { get; set; }
            public int ProjectUnchange { get; set; }
        }
        public class Top5
        {
            public string Email { get; set; }
            public string FIO { get; set; }
            public int Value { get; set; }
        }

        public List<ProjectsFields> ProjectsList = new List<ProjectsFields>();
        public List<StatsUsersFields> StatsUsersList = new List<StatsUsersFields>();
        public List<EmployeesFields> EmployeesList = new List<EmployeesFields>();
        public List<AllStatsUsersFields> AllStatsUsersList = new List<AllStatsUsersFields>();
        public List<Top5> Top5List = new List<Top5>();
        #endregion
    }
}