<%@ Page Title="Веб-сервис: Оповещения" Language="C#" AutoEventWireup="true" MasterPageFile="~/index.Master"  CodeBehind="alerts.aspx.cs" Inherits="Carcharoth.alerts" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <div class="main" style="height:100%">
            <asp:ScriptManager runat="server" ID="alsm"></asp:ScriptManager>
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
                    <asp:GridView ID="AlertsGrid" runat="server" RowStyle-CssClass="border-dark" HeaderStyle-BorderColor="Gray" AutoGenerateColumns="false" Width="100%" CssClass="GridListUsers gridview-selected-row-style" BorderColor="Transparent" HeaderStyle-HorizontalAlign="Center" OnRowCommand="AlertsGrid_RowCommand">
                        <Columns>
                             <asp:BoundField DataField="Id" ItemStyle-HorizontalAlign="Center" HeaderText="ID" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                            <asp:TemplateField HeaderText="Оповещения" SortExpression="Role" HeaderStyle-Width="90%" ControlStyle-BackColor="Transparent" ControlStyle-BorderStyle="None">
                                <ItemTemplate>
                                    <asp:TextBox CssClass="form-control form-control-sm" ReadOnly="false" Width="100%" ID="AlertsText" Text='<%#Eval("AlertsText")%>' runat="server"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" ShowHeader="false" SortExpression="Role">
                                <ItemTemplate>
                                    <asp:LinkButton ID="DeleteAlerts" CssClass="btn btn-outline-danger" ReadOnly="false" runat="server" CausesValidation="false" CommandName="DeleteAlerts" CommandArgument='<%# Eval("Id") %>' Text="Удалить"></asp:LinkButton>
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
                         Будут отображаться последние 5.
                        </div>
                    </div>
                    <br />
                    <asp:Label ID="LabelAboutAlerts" runat="server" Text="Добавить оповещение:"></asp:Label> <br />
                    <div class="row">
                        <div class="col">
                          <asp:TextBox CssClass="form-control" ID="tbAlerts" runat="server" Width="500px" Placeholder="Оповещение"></asp:TextBox>
                        </div>
                        <div class="col">
                            <asp:Button ID="BtnAddAlerts" CssClass="btn btn-warning" runat="server" Text="Добавить оповещение" OnClick="BtnAddAlerts_Click" />
                        </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
</asp:Content>
