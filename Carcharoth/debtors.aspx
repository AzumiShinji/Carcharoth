<%@ Page Title="Веб-сервис: Взносы" Language="C#" ValidateRequest="false" MasterPageFile="~/index.Master" AutoEventWireup="true" CodeBehind="debtors.aspx.cs" Inherits="Carcharoth.debtors" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <div class="main" style="height:100%">
            <asp:ScriptManager runat="server" ID="debt"></asp:ScriptManager>
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
            <asp:UpdatePanel UpdateMode="Always" runat="server">
                <ContentTemplate>
                    <div style="width:100%;text-align:center;">
                        <div style="width:100%;text-align:center;padding-top:10px;">
                            <div style="text-align:center;width:100%;margin-bottom:10px">
                                    <h4>Взносы</h4>
                                <div style="display: table;width:100%;font-size:12px">
                                        <asp:Label ID="ResponsibilityUsersLabel" runat="server" Text="Ответственное лицо: нет" Font-Size="10px" />
                                </div>
                                <asp:Label runat="server" Text="Подсказка: При наведении на ячейку со взносом, можно узнать, кто добавил." Font-Size="11"/>
                                <br />
                                <asp:LinkButton runat="server" ID="AcceptGVDataSortBtn" OnClick="AcceptGVDataSortBtn_Click" Text="Сортировка только по сотрудникам" />
                            </div>
                        </div>
                        <asp:Panel ID="PanelAddDebts" runat="server" Enabled="false" Visible="false" style="border:1px solid gray;padding:15px">
                            <div style="width:300px;display:inline-block">
                                <asp:TextBox CssClass="form-control" runat="server" Style="margin-bottom:5px;" ID="DescriptionDebt" PlaceHolder="Описание взноса" />
                                <asp:TextBox CssClass="form-control" runat="server" type="number" Style="margin-bottom:5px;" ID="SumDebtTB" PlaceHolder="Сумма взноса (руб.)" />
                                <asp:CheckBox ID="isHiddenDebt" Checked="false" runat="server"  Text="Скрыть взнос (перманентно)"/>

                                <asp:LinkButton CssClass="btn btn-sm btn-outline-light" runat="server" ID="ДобавитьВзнос" OnClick="ДобавитьВзнос_Click" Text="Добавить"/>
                                </br>
                                <asp:Label runat="server" ID="StatusDebtInfo"  Font-Size="14" ForeColor="OrangeRed" Font-Bold="true"/>
                            </div>
                            <div style="display:inline-block;vertical-align:top;border:1px solid white;padding:10px;margin:5px;max-width:350px;">
                                <div>
                                    <asp:Label  Text="Удалить столбцы" runat="server"/>
                                    <asp:Panel runat="server" ID="ПанельУдаленияСтолбцов" CssClass="panel-button-controls-debtors">
                                    
                                    </asp:Panel>
                                </div>
                            </div>
                            <p style="margin-left:500px;margin-right:500px;text-align:center;font-size:11px">После создания взноса он будет закреплен за Вами (удалять и управлять можете только Вы), если взнос - скрытый, то он будет видимым только для Вас и общий долг учитываться не будет по этому столбцу.</p>
                            </br>
                        </asp:Panel>
                            <div style="position:absolute;margin-top:100px;margin-left:20px;border: 1px solid white;padding:10px;text-align:left;font-size:12px">
                                <div>
                                    Всего сотрудников: <asp:Label style="float:right" runat="server" ID="Статистика_Сотрудники_Всего"/>
                                </div>
                                <div>
                                    Учитываются: <asp:Label style="float:right" runat="server" ID="Статистика_Сотрудники_Учитываются"/>
                                </div>
                                <div>
                                    Без долгов: <asp:Label style="float:right" runat="server" ID="Статистика_Сотрудники_БезДолгов"/>
                                </div>
                                <div>
                                    Должников: <asp:Label style="float:right" runat="server" ID="Статистика_Сотрудники_Должников"/>
                                </div>
                                <div>
                                    Взнос:
                                    <asp:Panel runat="server" ID="Панель_взносов">

                                    </asp:Panel>
                                    <asp:DataList runat="server" ID="Взнос_DataList" HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <table style="text-align:center">
                                                <div style="color:lightgray;text-align:center"><%#(string)Eval("column_name") %> руб.</div>
                                                <div style="border-bottom:1px solid white"></div>
                                                <tr class="debt_stat_th">
                                                     <th></th>
                                                    <th>Нал</th>
                                                    <th>Безнал</th>
                                                </tr>
                                                <tr class="debt_stat_tr">
                                                    <td>Кол-во</td>
                                                    <td><%#(int)Eval("count_nal") %></td>
                                                    <td><%#(int)Eval("count_beznal") %></td>
                                                </tr>
                                                <tr class="debt_stat_tr">
                                                    <td>Сум.</td>
                                                    <td><%#(double)Eval("sum_nal") %> руб.</td>
                                                    <td><%#(double)Eval("sum_beznal") %> руб.</td>
                                                </tr>
                                                <tr class="debt_stat_tr">
                                                    <td>Итог</td>
                                                    <td colspan="2"><%#(double)Eval("sum_nal") +(double)Eval("sum_beznal")%> руб.</td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                    </asp:DataList>
                                </div>
                            </div>
                        <div style="text-align:center;display:inline-block">
                         <asp:GridView Enabled="true" Visible="true" ID="GVDataDebtorsSQL" 
                             CssClass="DebtorsGridView gridview-selected-row-style" RowStyle-CssClass="border-dark" 
                             Font-Size="12px" runat="server" DataSourceID="DataDebtorsSQL" 
                             AutoGenerateColumns="true" ShowHeaderWhenEmpty="true" EmptyDataText="Нет данных"
                             OnLoad="GVDataDebtorsSQL_Load"
                             OnRowDataBound="GVDataDebtorsSQL_RowDataBound">
                             <Columns>
                                <asp:TemplateField HeaderText="Производить учет" SortExpression="Role" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:CheckBox ID="УчетВзносовСотрудника" runat="server" CssClass="GridRowListUsers form-control form-control-sm"
                                        ClientIDMode="AutoID" 
                                        OnCheckedChanged="УчетВзносовСотрудника_CheckedChanged"
                                        AutoPostBack="true"
                                        Checked='<%#(object)Eval("Учет")==DBNull.Value?false:(bool)Eval("Учет") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                             </Columns>
                         </asp:GridView>
                        </div>
                        <asp:SqlDataSource ID="DataDebtorsSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ToCatalogDB %>" SelectCommand="SELECT * FROM [Debtors] ORDER BY [Учет] DESC, CASE [Направление] WHEN 'Не определено' THEN 5 ELSE 0 END, [Направление] DESC, ФИО ASC"></asp:SqlDataSource>  
                    </div>
                </ContentTemplate>
          <%--      <Triggers>
                    <asp:PostBackTrigger ControlID = "GVDataDebtorsSQL" />
                </Triggers>--%>
            </asp:UpdatePanel>
        </div>
</asp:Content>
