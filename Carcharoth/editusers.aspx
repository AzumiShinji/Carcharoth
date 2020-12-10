<%@ Page Title="Веб-сервис: Редактирование пользователей" Language="C#" AutoEventWireup="true" MasterPageFile="~/index.Master"  CodeBehind="editusers.aspx.cs" Inherits="Carcharoth.editusers" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <div class="main" style="height:100%">
            <asp:ScriptManager runat="server" ID="edu"></asp:ScriptManager>
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
                <div class="grid1">
                    <asp:Label runat="server" ID="StatusText" />
                    <asp:GridView ID="UsersGrid" runat="server" RowStyle-CssClass="border-dark" HeaderStyle-BorderColor="Gray" AutoGenerateColumns="false" CssClass="GridListUsers gridview-selected-row-style" BorderColor="Transparent" HeaderStyle-HorizontalAlign="Center" OnRowCommand="UsersGrid_RowCommand">
                        <Columns>
                             <asp:BoundField DataField="Id" ItemStyle-HorizontalAlign="Center" HeaderText="ID" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                            <asp:TemplateField HeaderText="ФИО" SortExpression="Role" HeaderStyle-Width="30%" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:TextBox CssClass="form-control form-control-sm" ReadOnly="false" Width="100%" ID="FIO" Text='<%#Eval("FIO")%>' runat="server"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="Логин" SortExpression="Role" HeaderStyle-Width="30%" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:TextBox CssClass="form-control form-control-sm" ReadOnly="false" Width="100%" ID="Login" Text='<%#Eval("Login")%>' runat="server"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Последний вход" SortExpression="Role" HeaderStyle-Width="30%" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:Label CssClass="form-control form-control-sm" Width="100%" ID="LastTimeEnter" style="text-align:center" Text='<%#Eval("LastTimeEnter")%>' runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Уровень" SortExpression="Role" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:TextBox ReadOnly="false" CssClass="GridRowListUsers form-control form-control-sm" Width="100%" ID="Level" Text='<%#Eval("Level")%>' runat="server"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Все" SortExpression="Role" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ProjectAll" runat="server" CssClass="GridRowListUsers form-control form-control-sm" Checked='<%#(int)Eval("ProjectAll") == 1 ? true:false %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ДСФК" SortExpression="Role" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ProjectDSFK" runat="server" style="width:100%" CssClass="GridRowListUsers form-control form-control-sm" Checked='<%#(int)Eval("ProjectDSFK") == 1 ? true:false %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ЭБ" SortExpression="Role" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ProjectEB" runat="server" CssClass="GridRowListUsers form-control form-control-sm" Checked='<%#(int)Eval("ProjectEB") == 1 ? true:false %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ГАСУ" SortExpression="Role" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ProjectGASU" runat="server" CssClass="GridRowListUsers form-control form-control-sm" Checked='<%#(int)Eval("ProjectGASU") == 1 ? true:false %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ГМП" SortExpression="Role" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ProjectGISGMP" runat="server" CssClass="GridRowListUsers form-control form-control-sm" Checked='<%#(int)Eval("ProjectGISGMP") == 1 ? true:false %>' />
                                 </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ГМУ" SortExpression="Role" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ProjectGISGMU" runat="server" CssClass="GridRowListUsers form-control form-control-sm" Checked='<%#(int)Eval("ProjectGISGMU") == 1 ? true:false %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="СУФД" SortExpression="Role" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ProjectSUFD" runat="server" CssClass="GridRowListUsers form-control form-control-sm" Checked='<%#(int)Eval("ProjectSUFD") == 1 ? true:false %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="1С" SortExpression="Role" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ProjectOneC" runat="server" CssClass="GridRowListUsers form-control form-control-sm" Checked='<%#(int)Eval("ProjectOneC") == 1 ? true:false %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="КС" SortExpression="Role" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ProjectKS" runat="server" CssClass="GridRowListUsers form-control form-control-sm" Checked='<%#(int)Eval("ProjectKS") == 1 ? true:false %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="МИ" SortExpression="Role" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ProjectMI" runat="server" CssClass="GridRowListUsers form-control form-control-sm" Checked='<%#(int)Eval("ProjectMI") == 1 ? true:false %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="УЦ" SortExpression="Role" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ProjectUC" runat="server" CssClass="GridRowListUsers form-control form-control-sm" Checked='<%#(int)Eval("ProjectUC") == 1 ? true:false %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="УД" SortExpression="Role" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ProjectUD" runat="server" CssClass="GridRowListUsers form-control form-control-sm" Checked='<%#(int)Eval("ProjectUD") == 1 ? true:false %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Взносы" SortExpression="Role" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ProjectDebtors" runat="server" CssClass="GridRowListUsers form-control form-control-sm" Checked='<%#(int)Eval("ProjectDebtors") == 1 ? true:false %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" ShowHeader="false" SortExpression="Role">
                                <ItemTemplate>
                                    <asp:LinkButton ID="BtnResetPassword" CssClass="btn btn-outline-secondary" ReadOnly="false" runat="server" CausesValidation="false" CommandName="ResetPassword" CommandArgument='<%# Eval("Login") %>' Text="Сбросить"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" ShowHeader="false" SortExpression="Role">
                                <ItemTemplate>
                                    <asp:LinkButton ID="DeleteUser" CssClass="btn btn-outline-danger" ReadOnly="false" runat="server" CausesValidation="false" CommandName="DeleteUser" CommandArgument='<%# Eval("Login") %>' Text="Удалить"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <div style="text-align:right;">
                    <asp:Button runat="server" style="width:182px;margin-right:2px" CssClass="btn btn-outline-success" CausesValidation="false" Text="Сохранить изменения" ReadOnly="false" ID="SubmitChange" OnClick="SubmitChange_Click" />
                        </div>
                </div>
                <div style="text-align:center;margin:auto;max-width:800px">
                    <div style="text-align:center;border-bottom: 1px solid gray"> 
                        <div style="text-align:left">
                            0 - Заблокированый пользователь;<br />
                            1 - Обычный пользователь;<br />
                            2 - Право редактировать и создавать новых сотрудников;<br />
                            3 - Право удалять сотрудников; <br />
                            10 - Доступ к управлению пользователями веб-сервиса + информация о сервере;
                        </div>
                    </div>
                    <br />
                    <asp:Label ID="LabelAboutNewUser" runat="server" Text="Новый пользователь веб-сервиса:"></asp:Label> <br />
                    <div class="row">
                        <div class="col">
                          <asp:TextBox CssClass="form-control form-control" runat="server" ID="tbLogin" Placeholder="example@fsfk.local"></asp:TextBox>
                        </div>
                        <div class="col">
                          <asp:TextBox CssClass="form-control form-control" ID="tbFIO" runat="server" Placeholder="ФИО"></asp:TextBox>
                        </div>
                        <div class="col">
                            <asp:Button ID="BtnAddUser" CssClass="btn btn-warning" runat="server" Text="Добавить пользователя" OnClick="BtnAddUser_Click" />
                        </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
</asp:Content>
