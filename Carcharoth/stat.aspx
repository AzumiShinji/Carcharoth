<%@ Page Title="Веб-сервис: Статистика" Language="C#" MasterPageFile="~/index.Master" AutoEventWireup="true" CodeBehind="stat.aspx.cs" Inherits="Carcharoth.stat" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <asp:ScriptManager runat="server" ID="pst"></asp:ScriptManager>
    <asp:UpdateProgress runat="server" DisplayAfter="0">
        <ProgressTemplate>
            <div class="preloader" id="cover">
               <div class="preloader-padding">
                    <div class="lds-default"><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div></div>
                    <br />
                    Пожалуйста, подождите...
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
    <asp:Panel ID="PanelStat" runat="server">
                    <div style="width:100%;text-align:center;padding-top:10px;">
                        <div style="text-align:center;width:100%">
                            <h4>Статистика по сотрудникам</h4>
                            <div style="text-align:left;display:inline-block;padding-bottom:10px;font-size:12px;">
                                <div style="text-align:center;">Информация
                                    <div style="margin:0 20% 0 20%; border-bottom:1px solid gray;"></div>
                                </div>
                                🔔Если не выбрана конечная дата, то автоматически выставляется сегодняшнее число<br />
                                🔔Отныне созданные организации и контакты не суммируются, это все включает в себя "Закрыто на 1-ой линии"<br />
                                🔔Возвраты по 1С не учитываются c 1 августа!<br />
                            </div>
                        </div>
                        <asp:LinkButton ID="ShowCalendarStart" runat="server" CssClass="btn btn-outline-light btn-sm" style="border-radius:0" OnClick="ShowCalendarStart_Click">
                            C этого числа(включительно): 
                            <img  src="/images/calendar.png"/>
                        </asp:LinkButton>
                         - 
                        <asp:LinkButton ID="ShowCalendarEnd" runat="server" CssClass="btn btn-outline-light btn-sm" style="border-radius:0" OnClick="ShowCalendarEnd_Click">
                            По это число(включительно): 
                            <img  src="/images/calendar.png"/>
                        </asp:LinkButton>
                        <br />
                        <div>
                            <asp:Label runat="server" Visible="false" ID="InfoTablo" ForeColor="Red"></asp:Label>
                        </div>
                        <asp:LinkButton ID="ShowStats" runat="server" style="margin-top:5px;width:250px;border-radius:0" Text="Показать" CssClass="btn btn-outline-light btn-sm" OnClick="ShowStats_Click"></asp:LinkButton>
                        <asp:LinkButton runat="server" ID="ExportData" Enabled="false" Visible="false" ClientIDMode="Static" style="margin-top:5px" CssClass="btn btn-outline-secondary btn-sm" OnClick="ExportData_Click">Экспорт *.xls</asp:LinkButton>
                        <br />
                        <asp:Panel runat="server" ID="DatePickerStatsStart_Panel" Visible="false" style="display: inline-block;position:absolute;margin-left:-180px;margin-top:-35px">
                            <asp:Calendar runat="server" ID="DatePickerStatsStart" BackColor="White" ForeColor="Black" BorderStyle="None" OnSelectionChanged="DatePickerStatsStart_SelectionChanged" TodayDayStyle-BackColor="Gray"/>
                            <asp:LinkButton runat="server" ID="btn_hide_DatePickerStatsStart" OnClick="btn_hide_DatePickerStatsStart_Click" CssClass="btn btn-danger btn-sm" style="width:100%;border-radius: 0 0 5px 5px">Закрыть</asp:LinkButton>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="DatePickerStatsEnd_Panel" Visible="false" style="display: inline-block;position:absolute;margin-left:-5px;margin-top:-35px">
                            <asp:Calendar runat="server" ID="DatePickerStatsEnd" BackColor="White" ForeColor="Black" BorderStyle="None" OnSelectionChanged="DatePickerStatsEnd_SelectionChanged" TodayDayStyle-BackColor="Gray"/>
                            <asp:LinkButton runat="server" ID="btn_hide_DatePickerStatsEnd" OnClick="btn_hide_DatePickerStatsEnd_Click" CssClass="btn btn-danger btn-sm" style="width:100%;border-radius: 0 0 5px 5px">Закрыть</asp:LinkButton>
                        </asp:Panel>
                    </div>
                    <table style="width:100%;table-layout: fixed;">
                            <tr>
                            <td>
                                <div style="border-bottom:1px solid white;margin:20px 20% 20px 20%;"></div>
                                <div style="font-size:14px">
                                <asp:ListView ID="AllStatsUsersListView" runat="server" AutoGenerateColumns="false">
                                        <ItemTemplate>
                                            <div style="display: table;width:100%;font-size:12px;">
                                                <div style="display: table-row;">
                                                    <div style="padding:10px;display:table-cell;text-align:center;">
                                                        <div style="display:inline-block;text-align:left;vertical-align:middle">
                                                            <div style="text-align:center">
                                                                Всего обработано
                                                                <div style="margin:0 20% 0 20%; border-bottom:1px solid gray;"></div>
                                                            </div>
                                                            Инцидентов: <div style="float:right;"><%# Eval("AllIM") %></div>
                                                            <br />
                                                            Закрыто на 1-ой линии: <div style="float:right;"><%# Eval("AllResolved") %></div>
                                                            <div style="font-size:10px;padding-left:10px">
                                                                 + Создано контактов
                                                                <br />
                                                                 + Создано организаций
                                                            </div>
                                                            Рабочие задания: <div style="float:right;"><%# Eval("AllTResolved") %></div>
                                                            <div style="border-bottom:1px solid gray;text-align:center;background-color:rgba(95, 207, 107, 0.1)"></div>
                                                            <div style="float:right;"><%# (int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved") %></div>
                                                        </div>
                                                        <div style="display:inline-block;text-align:left;vertical-align:middle;padding-left:50px;">
                                                            <div style="text-align:center;">
                                                                Всего обработано %
                                                                <div style="margin:0 20% 0 20%; border-bottom:1px solid gray;"></div>
                                                            </div>
                                                            Инцидентов: <div style="float:right;"><%# Math.Round((double)(int)Eval("AllIM")/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,2) %>%</div>
                                                            <br />
                                                            Закрыто на 1-ой линии: <div style="float:right;"><%#  Math.Round((double)(int)Eval("AllResolved")/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,2) %>%</div>
                                                            <div style="font-size:10px;padding-left:10px">
                                                                 + Создано контактов
                                                                <br />
                                                                 + Создано организаций
                                                            </div>
                                                            Рабочие задания:<div style="float:right;"><%# Math.Round((double)(int)Eval("AllTResolved")/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,2) %>%</div>
                                                            <div style="border-bottom:1px solid gray;text-align:center;background-color:rgba(95, 207, 107, 0.1)"></div>
                                                            <div style="float:right;"><%# Math.Round((double)((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,0) %>%</div>
                                                        </div>
                                                         <div style="display:inline-block;text-align:left;vertical-align:middle;padding-left:50px;">
                                                            <div style="text-align:center;">Обработано по каждому проекту
                                                                <div style="margin:0 20% 0 20%; border-bottom:1px solid gray;"></div>
                                                            </div>
                                                            ДС ФК: <div style="float:right;"><%# Eval("ProjectDSFK") %></div>
                                                            <br />
                                                            ГАСУ: <div style="float:right;"><%#  Eval("ProjectGASU") %></div>
                                                             <br />
                                                            ГИС ГМП: <div style="float:right;"><%#  Eval("ProjectGISGMP") %></div>
                                                             <br />
                                                            ГИС ГМУ: <div style="float:right;"><%#  Eval("ProjectGISGMU") %></div>
                                                            <br />
                                                            ЭБ: <div style="float:right;"><%#Eval("ProjectEB") %></div>
                                                            <br />
                                                            СУФД: <div style="float:right;"><%# Eval("ProjectSUFD") %></div>
                                                             <br />
                                                            1C: <div style="float:right;"><%# Eval("Project1C") %></div>
                                                             <br />
                                                            КС: <div style="float:right;"><%# Eval("ProjectKS") %></div>
                                                            <br />
                                                            УЦ: <div style="float:right;"><%# Eval("ProjectUC") %></div>
                                                             <br />
                                                            Управление делами: <div style="float:right;"><%# Eval("ProjectUD") %></div>
                                                             <br />
                                                            ПОИБ СОБИ: <div style"float:right;"><%# Eval("ProjectPOIB") %></div>
                                                             <br />
                                                            Сменщики: <div style="float:right;"><%#(int)Eval("ProjectShift") %></div>
                                                            <br />
                                                            Универсалы: <div style="float:right;"><%#(int)Eval("ProjectUnivers") %></div>
                                                            <br />
                                                            <%# (int)Eval("ProjectUnchange")!=0 ? 
                                                                    "Не определено: <div style='float:right;'>"+Eval("ProjectUnchange")+"</div>"
                                                                    :
                                                                    "" %>
                                                            <div style="border-bottom:1px solid gray;text-align:center;background-color:rgba(95, 207, 107, 0.1)"></div>
                                                            <div style="float:right;"><%# (int)Eval("ProjectDSFK")+
                                                                                              (int)Eval("ProjectGASU")+
                                                                                              (int)Eval("ProjectGISGMP")+
                                                                                              (int)Eval("ProjectGISGMU")+
                                                                                              (int)Eval("ProjectEB")+
                                                                                              (int)Eval("ProjectSUFD")+
                                                                                              (int)Eval("Project1C")+
                                                                                              (int)Eval("ProjectKS")+
                                                                                              (int)Eval("ProjectUC")+
                                                                                              (int)Eval("ProjectUD")+
                                                                                              (int)Eval("ProjectPOIB")+
                                                                                              (int)Eval("ProjectShift")+
                                                                                              (int)Eval("ProjectUnivers")+
                                                                                              (int)Eval("ProjectUnchange") 
                                                                                        %></div>
                                                        </div>
                                                        <div style="display:inline-block;text-align:left;vertical-align:middle;padding-left:50px;">
                                                            <div style="text-align:center;">Обработано по каждому проекту %
                                                                <div style="margin:0 20% 0 20%; border-bottom:1px solid gray;"></div>
                                                            </div>
                                                            ДС ФК: <div style="float:right;"><%# Math.Round((double)(int)Eval("ProjectDSFK")/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,2) %>%</div>
                                                            <br />
                                                            ГАСУ: <div style="float:right;"><%#  Math.Round((double)(int)Eval("ProjectGASU")/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,2) %>%</div>
                                                            <br />
                                                            ГИС ГМП: <div style="float:right;"><%#  Math.Round((double)(int)Eval("ProjectGISGMP")/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,2) %>%</div>
                                                            <br />
                                                            ГИС ГМУ: <div style="float:right;"><%#  Math.Round((double)(int)Eval("ProjectGISGMU")/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,2) %>%</div>
                                                            <br />
                                                            ЭБ: <div style="float:right;"><%# Math.Round((double)(int)Eval("ProjectEB")/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,2) %>%</div>
                                                            <br />
                                                            СУФД: <div style="float:right;"><%# Math.Round((double)(int)Eval("ProjectSUFD")/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,2) %>%</div>
                                                            <br />
                                                            1C: <div style="float:right;"><%# Math.Round((double)(int)Eval("Project1C")/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,2) %>%</div>
                                                            <br />
                                                            КС: <div style="float:right;"><%# Math.Round((double)(int)Eval("ProjectKS")/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,2) %>%</div>
                                                            <br />
                                                            УЦ: <div style="float:right;"><%# Math.Round((double)(int)Eval("ProjectUC")/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,2) %>%</div>
                                                            <br />
                                                            Управление делами: <div style="float:right;"><%# Math.Round((double)(int)Eval("ProjectUD")/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,2) %>%</div>
                                                            <br />
                                                            ПОИБ СОБИ: <div style="float:right;"><%# Math.Round((double)(int)Eval("ProjectPOIB")/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,2) %>%</div>
                                                            <br />
                                                            Сменщики: <div style="float:right;"><%# Math.Round((double)(int)Eval("ProjectShift")/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,2) %>%</div>
                                                            <br />
                                                            Универсалы: <div style="float:right;"><%# Math.Round((double)(int)Eval("ProjectUnivers")/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,2) %>%</div>
                                                            <br />
                                                            <%# (int)Eval("ProjectUnchange")!=0 ? 
                                                                    "Не определено: <div style='float:right;'>"+Math.Round((double)(int)Eval("ProjectUnchange")/((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,2)+"%</div>"
                                                                    :
                                                                    "" %>
                                                            <div style="border-bottom:1px solid gray;text-align:center;background-color:rgba(95, 207, 107, 0.1)"></div>
                                                            <div style="float:right;"><%# Math.Round((double)(
                                                                                              (int)Eval("ProjectDSFK")+
                                                                                              (int)Eval("ProjectGASU")+
                                                                                              (int)Eval("ProjectGISGMP")+
                                                                                              (int)Eval("ProjectGISGMU")+
                                                                                              (int)Eval("ProjectEB")+
                                                                                              (int)Eval("ProjectSUFD")+
                                                                                              (int)Eval("Project1C")+
                                                                                              (int)Eval("ProjectKS")+
                                                                                              (int)Eval("ProjectUC")+
                                                                                              (int)Eval("ProjectUD")+
                                                                                              (int)Eval("ProjectPOIB")+
                                                                                              (int)Eval("ProjectShift")+
                                                                                              (int)Eval("ProjectUnivers")+
                                                                                              (int)Eval("ProjectUnchange") 
                                                                                        )
                                                                                        /
                                                                                        ((int)Eval("AllIM")+(int)Eval("AllResolved")+(int)Eval("AllTResolved"))*100,0) %>%</div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:ListView>
                                </div>
                                <div style="width:100%;text-align:center;display:table;margin-bottom:30px;font-size:12px;">
                                    <div style="display: table-row;">
                                        <div style="padding:10px;display:table-cell;text-align:center;">
                                            <div style="display:inline-block;text-align:left;vertical-align:middle;padding-left:50px;">
                                                
                                                <div style="width:100%;text-align:center;">Топ-10
                                                    <div style="margin:0 20% 0 20%; border-bottom:1px solid gray;"></div>
                                                </div>
                                                <asp:ListView ID="Top5View" runat="server" AutoGenerateColumns="false">
                                                    <ItemTemplate>
                                                        <div style="text-align:left;">
                                                                <%# Eval("FIO") %> / <%# Eval("Email") %> <div style="float:right;margin-left:30px;"> <%# Eval("Value") %></div>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:ListView>
                                            </div>
                                            <div style="display:none;text-align:left;vertical-align:middle;padding-left:50px;">
                                                <div style="width:100%;text-align:center;">Топ-5 с наименьшим показателем
                                                    <div style="margin:0 20% 0 20%; border-bottom:1px solid gray;"></div>
                                                </div>
                                                <asp:ListView ID="Top5ViewDes" runat="server" AutoGenerateColumns="false">
                                                    <ItemTemplate>
                                                        <div style="text-align:left;">
                                                                <%# Eval("FIO") %> ( <%# Eval("Email") %> ) <div style="float:right;"> <%# Eval("Value") %></div>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:ListView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                 <asp:GridView ID="StatUsers" runat="server" AutoGenerateColumns="false" Width="100%" Font-Size="12px" HeaderStyle-HorizontalAlign="Center" CssClass="ctr-gridview gridview-selected-row-style">  
                                     <Columns>
                                         <asp:BoundField DataField="Direction" HeaderText="Направление" ItemStyle-Width="100px" ItemStyle-CssClass="colExUsers colExUsers-head-rows" ItemStyle-HorizontalAlign="Center"/>
                                         <asp:TemplateField HeaderText="ФИО / Почта" ItemStyle-CssClass="colExUsers colExUsers-head-rows" ItemStyle-Width="300px" ItemStyle-HorizontalAlign="Center">
                                             <ItemTemplate>
                                                 <div>
                                                 <%# Eval("FIO")%>
                                                </div>
                                                 <div>
                                                 <%# Eval("Email")%>
                                                </div>
                                             </ItemTemplate>
                                         </asp:TemplateField>
                                         <asp:BoundField DataField="SDPhone" HeaderText="Создано SD По телефону" ItemStyle-Width="100px" ItemStyle-CssClass="colExUsers" ItemStyle-HorizontalAlign="Center"/>
                                         <asp:BoundField DataField="SDOther" HeaderText="Создано SD Остальными способами" ItemStyle-Width="100px" ItemStyle-CssClass="colExUsers" ItemStyle-HorizontalAlign="Center"/>
                                         <asp:BoundField DataField="SDAll" HeaderText="Создано SD Всего" ItemStyle-Width="100px" ItemStyle-CssClass="colExUsers" ItemStyle-HorizontalAlign="Center"/>
                                         <asp:TemplateField HeaderText="Закрыто SD на 1-ой линии*" ItemStyle-Width="100px" ItemStyle-CssClass="colExUsers colExUsers-union" HeaderStyle-CssClass="colExUsers-union" ItemStyle-HorizontalAlign="Center">
                                             <ItemTemplate>
                                                 <%# Eval("SDResolved") %> / (<%# !Double.IsNaN(Math.Round((double)(int)Eval("SDResolved")/((int)Eval("IMAll")+(int)Eval("SDResolved")+(int)Eval("TResolved"))*100)) ? Math.Round((double)(int)Eval("SDResolved")/((int)Eval("IMAll")+(int)Eval("SDResolved")+(int)Eval("TResolved"))*100) : 0 %>%)
                                             </ItemTemplate>
                                         </asp:TemplateField>
                                         <asp:BoundField DataField="IMAll" HeaderText="Инциденты*" ItemStyle-Width="100px" ItemStyle-CssClass="colExUsers colExUsers-union" HeaderStyle-CssClass="colExUsers-union" ItemStyle-HorizontalAlign="Center"/>
                                         <asp:TemplateField HeaderText="Неверное назначение (Минус 1С)" ItemStyle-Width="100px" ItemStyle-CssClass="colExUsers-wrong" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="colExUsers-wrong">
                                             <ItemTemplate>
                                                 <%# Eval("IMWrong") %> / (<%# !Double.IsNaN(Math.Round((double)(int)Eval("IMWrong")/((int)Eval("IMAll")+(int)Eval("SDResolved")+(int)Eval("TResolved"))*100)) ? Math.Round((double)(int)Eval("IMWrong")/((int)Eval("IMAll")+(int)Eval("SDResolved")+(int)Eval("TResolved"))*100) : 0 %>%)
                                             </ItemTemplate>
                                         </asp:TemplateField>
                                         <asp:BoundField DataField="IMResolved" HeaderText="Закрыто IM на 1-ой линии" ItemStyle-Width="100px" ItemStyle-CssClass="colExUsers" ItemStyle-HorizontalAlign="Center"/>
                                         <asp:BoundField DataField="CreatedContacts" HeaderText="Создано контактов" ItemStyle-Width="100px" ItemStyle-CssClass="colExUsers" ItemStyle-HorizontalAlign="Center"/>
                                         <asp:BoundField DataField="TResolved" HeaderText="Рабочие задания*" ItemStyle-Width="100px" ItemStyle-CssClass="colExUsers colExUsers-union" HeaderStyle-CssClass="colExUsers-union" ItemStyle-HorizontalAlign="Center"/>
                                         <asp:BoundField DataField="OrgCreated" HeaderText="Создано организаций" ItemStyle-Width="100px" ItemStyle-CssClass="colExUsers" ItemStyle-HorizontalAlign="Center"/>
                                         <asp:TemplateField HeaderText="Всего обработано (Сумма *)" ItemStyle-Width="100px" ItemStyle-CssClass="colExUsers colExUsers-result" ItemStyle-HorizontalAlign="Center">
                                             <ItemTemplate>
                                                 <%# (int)Eval("IMAll")+(int)Eval("SDResolved")+(int)Eval("TResolved") %>
                                             </ItemTemplate>
                                         </asp:TemplateField>
                                     </Columns>
                                </asp:GridView>  
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
        </ContentTemplate>
        <Triggers>
     <asp:PostBackTrigger ControlID="ExportData"/> 
    </Triggers>
    </asp:UpdatePanel>
</asp:Content>
