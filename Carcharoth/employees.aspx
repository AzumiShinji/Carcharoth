<%@ Page Title="Веб-сервис: Сотрудники" Language="C#" AutoEventWireup="true" MasterPageFile="~/index.Master"  CodeBehind="employees.aspx.cs" Inherits="Carcharoth.employess" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <div class="main" style="height:100%;width:100%;text-align:center">
            <asp:ScriptManager runat="server" ID="emp"></asp:ScriptManager>
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
                     <script type="text/javascript">
                        function Confim() {
                            var result = window.confirm('Are you sure?');
                            if (result == true) {
                                document.getElementById('ConfirmField').value = true;
                                return true;
                            }
                            else
                                return false;
                        }
                    </script>
                    <asp:HiddenField runat="server" ID="ConfirmField" ClientIDMode="Static" Value="false" />
                <div>
                    <asp:Panel ID="EmployeesGridAdd" CssClass="sticky form-inline" autocomplete="nope" style="background-color:rgba(33, 34, 38, 0.89);padding:10px" runat="server">
                        <div class="col-12" style="text-align:center">
                            <asp:TextBox CssClass="form-control form-control-sm" HeaderText="Код" Width="5%" ID="InsertCode" PlaceHolder="Код" runat="server"></asp:TextBox>
                            <asp:TextBox CssClass="form-control form-control-sm" HeaderText="ФИО" ID="InsertFIO" PlaceHolder="ФИО"  runat="server"></asp:TextBox>
                            <input style="display: none;" type="password" name="pwdplaceholder"/>
                            <input style="display: none;" type="email" name="email">
                            <asp:TextBox CssClass="form-control form-control-sm" HeaderText="Email" type="text"
                                AutoCompleteType="Disabled" autocomplete="stopdoingthat" ID="InsertEmail" PlaceHolder="Email"  runat="server"></asp:TextBox>
                            <asp:ListBox runat="server" SelectionMode="Multiple" ID="InsertDirection"  CssClass="form-control form-control-sm" HeaderText="Направление">
                                <asp:ListItem Enabled="true" Selected="True" Text="Не определено" Value="Не определено" />
                                <asp:ListItem Text="ДС ФК" Value="ДС ФК" />
                                <asp:ListItem Text="ГАСУ" Value="ГАСУ" />
                                <asp:ListItem Text="ГИС ГМП" Value="ГИС ГМП" />
                                <asp:ListItem Text="ГИС ГМУ" Value="ГИС ГМУ" />
                                <asp:ListItem Text="ЭБ" Value="ЭБ" />
                                <asp:ListItem Text="СУФД" Value="СУФД" />
                                <asp:ListItem Text="1С" Value="1С" />
                                <asp:ListItem Text="КС" Value="КС" />
                                <asp:ListItem Text="УЦ" Value="УЦ" />
                                <asp:ListItem Text="Управление делами" Value="Управление делами" />
                                <asp:ListItem Text="ПОИБ СОБИ" Value="ПОИБ СОБИ" />
                                <asp:ListItem Text="Сменщики" Value="Сменщики" />
                                <asp:ListItem Text="Универсалы" Value="Универсалы" />
                            </asp:ListBox>
                          <%--  <asp:DropDownList CssClass="form-control form-control-sm" HeaderText="Направление" ID="InsertDirection2" PlaceHolder="Направление"  runat="server">
                                <asp:ListItem Enabled="true" Selected="True" Text="Не определено" Value="Не определено" />
                                <asp:ListItem Text="ДС ФК" Value="ДС ФК" />
                                <asp:ListItem Text="ГАСУ ГМП ГМУ" Value="ГАСУ ГМП ГМУ" />
                                <asp:ListItem Text="ЭБ" Value="ЭБ" />
                                <asp:ListItem Text="Сменщики" Value="Сменщики" />
                                <asp:ListItem Text="СУФД" Value="СУФД" />
                            </asp:DropDownList>--%>
                            <asp:TextBox CssClass="form-control form-control-sm" HeaderText="Должность" ID="InsertPosition" PlaceHolder="Должность"  runat="server"></asp:TextBox>
                            <asp:TextBox CssClass="form-control form-control-sm" HeaderText="Телефон" ID="InsertPhone" PlaceHolder="Телефон"  runat="server"></asp:TextBox>
                            <asp:TextBox CssClass="form-control form-control-sm" HeaderText="Дата рождения" ID="InsertBirthDate" PlaceHolder="Дата рожд. (10.10.1990)"  runat="server"></asp:TextBox>
                            <asp:LinkButton CssClass="btn btn-success btn-sm" runat="server" ID="AddNewEmployees" Text="Добавить" OnClick="AddNewEmployees_Click"></asp:LinkButton>
                                <br /><asp:Label ID="StatusLabel" ForeColor="OrangeRed" runat="server" Font-Size="14" Font-Bold="true"/>
                        </div>
                    </asp:Panel>
                    <asp:Panel runat="server">
                        <div style="margin-top:10px;font-size:12px">
                            <asp:Panel runat="server" ID="employees_birthdate_today_panel" class="employees-birthdate-today employees-birthdate">
                                <img runat="server" src="/images/birthday.png" style="padding-right:5px"/>Сегодня день рождение у:<br />
                                <div style="margin:0 20% 0 20%; border-bottom:1px solid gray;"></div>
                                <asp:Label runat="server" ID="BirthDayTodayLabel"></asp:Label>
                            </asp:Panel>
                            <div class="employees-birthdate-next employees-birthdate">
                                Ближайшие дни рождения:<br />
                                <div style="margin:0 20% 0 20%; border-bottom:1px solid gray;"></div>
                                <asp:Label runat="server" ID="BirthDayNextLabel"></asp:Label>
                            </div>
                            <asp:Panel runat="server" ID="employees_soon_vacation" class="employees-birthdate-next employees-birthdate">
                                Ближайшие отпуска у:<br />
                                <div style="margin:0 20% 0 20%; border-bottom:1px solid gray;"></div>
                                <asp:Label runat="server" ID="SoonRestLabel"></asp:Label>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="employees_vacation" class="employees-birthdate-next employees-birthdate">
                                <img runat='server' src='/images/user-rest.png'/>Сейчас отпуск у:<br />
                                <div style="margin:0 20% 0 20%; border-bottom:1px solid gray;"></div>
                                <asp:Label runat="server" ID="RestLabel"></asp:Label>
                            </asp:Panel>
                        </div>
                    </asp:Panel>
                    <%--OnRowDeleting="EmployeesGrid_RowDeleting" --%>
                    <asp:GridView ID="EmployeesGrid" Width="100%" RowStyle-CssClass="border-dark" HeaderStyle-CssClass="table-bordered" RowStyle-HorizontalAlign="Center"
                        runat="server" AllowSorting="true" DataKeyNames="ID" AutoGenerateColumns="false" CssClass="GridListEmployees gridview-selected-row-style"
                        BorderColor="Transparent" HeaderStyle-HorizontalAlign="Center" OnSorting="GridView_Sorting" EnableSortingAndPagingCallbacks="true"
                        OnRowEditing="EmployeesGrid_RowEditing" OnRowUpdating="EmployeesGrid_RowUpdating"
                        OnRowCommand="EmployeesGrid_RowCommand" EnableViewState="true"
                        OnPageIndexChanging="EmployeesGrid_PageIndexChanging" OnRowCancelingEdit="EmployeesGrid_RowCancelingEdit" OnRowDataBound="EmployeesGrid_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="ID" ItemStyle-HorizontalAlign="Center" HeaderText="ID" InsertVisible="False" ReadOnly="True" SortExpression="Id"  />
                            <asp:BoundField ControlStyle-CssClass="form-control form-control-sm" ControlStyle-Width="60px" HeaderText="Код" DataField="Code" runat="server" SortExpression="Code"></asp:BoundField>
                            <asp:BoundField ControlStyle-CssClass="form-control form-control-sm" HeaderText="ФИО" DataField="FIO" runat="server" SortExpression="FIO"></asp:BoundField>
                            <asp:BoundField ControlStyle-CssClass="form-control form-control-sm" HeaderText="Email" DataField="Email" runat="server" SortExpression="Email"></asp:BoundField>
                            <asp:TemplateField HeaderText="Направление" SortExpression="Direction">
                                <EditItemTemplate>
                                    <asp:Label runat="server">Зажмите Ctrl для выбора</asp:Label>
                                <asp:ListBox runat="server" SelectionMode="Multiple" ID="Direction" Height="250"  CssClass="form-control form-control-sm" HeaderText="Направление">
                                    <asp:ListItem Text="Не определено" Value="Не определено" />
                                    <asp:ListItem Text="ДС ФК" Value="ДС ФК" />
                                    <asp:ListItem Text="ГАСУ" Value="ГАСУ" />
                                    <asp:ListItem Text="ГИС ГМП" Value="ГИС ГМП" />
                                    <asp:ListItem Text="ГИС ГМУ" Value="ГИС ГМУ" />
                                    <asp:ListItem Text="ЭБ" Value="ЭБ" />
                                    <asp:ListItem Text="СУФД" Value="СУФД" />
                                    <asp:ListItem Text="1С" Value="1С" />
                                    <asp:ListItem Text="КС" Value="КС" />
                                    <asp:ListItem Text="УЦ" Value="УЦ" />
                                    <asp:ListItem Text="Управление делами" Value="Управление делами" />
                                    <asp:ListItem Text="ПОИБ СОБИ" Value="ПОИБ СОБИ" />
                                    <asp:ListItem Text="Сменщики" Value="Сменщики" />
                                    <asp:ListItem Text="Универсалы" Value="Универсалы" />
                                </asp:ListBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                     <div style="display: table;width:100%;font-size:12px;">
                                        <div style="display: table-row;">
                                            <div style="padding:0;display:table-cell;text-align:center;">
                                                <div style="display:inline-block;text-align:left;vertical-align:middle">
                                    	           &#9830; <asp:Label ID="Label_Direction" Style="text-align:left" runat="server" Text='<%# String.Join("<br/>&#9830; ",System.Text.RegularExpressions.Regex.Split(Eval("Direction").ToString(),"/")) %>'></asp:Label>  
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField ControlStyle-CssClass="form-control form-control-sm" HeaderText="Должность" DataField="Position" runat="server" SortExpression="Position"></asp:BoundField>
                            <asp:BoundField ControlStyle-CssClass="form-control form-control-sm" HeaderText="Телефон" DataField="Phone" runat="server" SortExpression="Phone"></asp:BoundField>
                            <%--<asp:BoundField ControlStyle-CssClass="form-control form-control-sm" HeaderText="Дата рождения" DataField="BirthDate" runat="server" />--%>
                             <asp:TemplateField HeaderText="Дата рождения" SortExpression="BirthDate">
                                <EditItemTemplate>
                                    <asp:Label runat="server" Font-Size="8" ForeColor="LightGray">
                                        <p>
                                            Формат даты: ДД.ММ.ГГГГ<br />
                                            Например: 01.01.1990
                                        </p>
                                    </asp:Label>
                                    <asp:TextBox ID="BirthDateTextBox" Text='<%# Eval("BirthDate") %>' runat="server" CssClass="form-control form-control-sm"/>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="BirthDateLabel"
                                        Text='<%# ConvertDateTimeToShort(Eval("BirthDate")) %>' runat="server"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--<asp:BoundField ControlStyle-CssClass="form-control form-control-sm" HeaderText="Отпуска" DataField="Rest" runat="server"></asp:BoundField>--%>
                            <asp:TemplateField HeaderText="Отпуска" SortExpression="Rest">
                                <EditItemTemplate>
                                    <asp:Label runat="server" Font-Size="8" ForeColor="LightGray">
                                        <p>
                                            Разделитель между периодами - | </br>
                                            Например: 06.04.2020-19.04.2020|06.07.2020-13.07.2020|17.08.2020-30.08.2020
                                        </p>
                                    </asp:Label>
                                    <asp:TextBox ID="VacationsTextBox" Text='<%# Eval("Rest") %>' runat="server" CssClass="form-control form-control-sm"/>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="VacationsLabel"
                                        Text='<%# ConvertVacationsToReadable(Eval("Rest")) %>' runat="server"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ShowEditButton="true" EditText="Править" UpdateText="Обновить" ShowCancelButton="false"
                                CancelText="Отмена" ControlStyle-CssClass="btn btn-dark btn-sm"/>
                            <%--<asp:CommandField ShowDeleteButton="true" DeleteText="Удалить" CausesValidation="true"/>--%> 
                            <asp:TemplateField>
                                <EditItemTemplate>
                                     <asp:Button ID="deletebtn" runat="server" Enabled="True"
                                            ControlStyle-CssClass="btn btn-light btn-sm" CommandName="Cancel"
                                         Text="Отмена" />
                                </EditItemTemplate>
                                  <ItemTemplate>
                                        <asp:Button ID="deletebtn" runat="server" CommandName="DeleteRow"
                                            ControlStyle-CssClass="btn btn-danger btn-sm" Visible="false" Enabled="false"
                                         Text="Удалить" OnClientClick="return confirm('Вы уверены, что хотите удалить сотрудника?');" />
                                  </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                   </div>
                    </ContentTemplate>
            </asp:UpdatePanel>
        </div>
</asp:Content>
