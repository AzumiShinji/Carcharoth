<%@ Page Title="Веб-сервис: Информация о сервере" Language="C#" MasterPageFile="~/index.Master" AutoEventWireup="true" CodeBehind="infoaboutserver.aspx.cs" Inherits="Carcharoth.infoaboutserver" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
<asp:ScriptManager runat="server" ID="infas"></asp:ScriptManager>
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
 <asp:UpdatePanel ID="UpdInfas" runat="server" UpdateMode="Conditional">
     <ContentTemplate>
         <div style="width:100%;margin-top:15px;">
         <div style="text-align:center;">
              <asp:LinkButton runat="server" ID="RefreshInfo" OnClick="RefreshInfo_Click" style="margin-bottom:15px;width:150px;" CssClass="btn btn-outline-danger btn-sm">Обновить</asp:LinkButton>
                <div style="width:100%;text-align:center;display:inline-block">
                        <div style="display:inline-block">
                            Server IP: 
                            <asp:Label runat="server" ID="lblServerIP"></asp:Label>;
                        </div>
                    
                        <div style="display:inline-block">    
                            Machine Name (Computer Name):
                            <asp:Label runat="server" ID="lblMachineName"></asp:Label>;
                        </div>
                    
                        <div style="display:inline-block">  
                            RAM:
                            <asp:Label runat="server" ID="lblRAM"></asp:Label>;
                      </div>
                </div>
             <asp:GridView ID="ActiveUsersListView" PageSize="50" CssClass="gridview-selected-row-style" runat="server" AllowPaging="true" OnPageIndexChanging="ActiveUsersListView_PageIndexChanging" AutoGenerateColumns="False" style="width:100%">
                 <Columns>
                <asp:TemplateField HeaderText="Логин" SortExpression="Role" HeaderStyle-Width="30%">
                        <ItemTemplate>
                            <asp:Label ReadOnly="false" Width="100%" ID="Login" Text='<%#Eval("Login")%>' runat="server"></asp:Label>
                        </ItemTemplate>
                </asp:TemplateField>
                 <asp:TemplateField HeaderText="ФИО" SortExpression="Role" HeaderStyle-Width="30%">
                                <ItemTemplate>
                                    <asp:Label ReadOnly="false" Width="100%" ID="FIO" Text='<%#Eval("FIO")%>' runat="server"></asp:Label>
                                </ItemTemplate>
                </asp:TemplateField>
                     <asp:TemplateField HeaderText="История" SortExpression="Role" HeaderStyle-Width="30%">
                                <ItemTemplate>
                                    <asp:Label ReadOnly="false" Width="100%" ID="FIO" Text='<%#Eval("History")%>' runat="server"></asp:Label>
                                </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Время" SortExpression="Role" HeaderStyle-Width="30%">
                                <ItemTemplate>
                                    <asp:Label ReadOnly="false" Width="100%" ID="DateTime" Text='<%#Eval("DateTime")%>' runat="server"></asp:Label>
                                </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="IP" SortExpression="Role" HeaderStyle-Width="30%">
                                <ItemTemplate>
                                    <asp:Label ReadOnly="false" Width="100%" ID="DateTime" Text='<%#Eval("IP")%>' runat="server"></asp:Label>
                                </ItemTemplate>
                </asp:TemplateField>
                </Columns>
                 <PagerStyle HorizontalAlign="Center" Font-Size="20px" CssClass="pager-style" />
                <PagerSettings Position="TopAndBottom" Mode="NextPreviousFirstLast" FirstPageText=" &#171; " LastPageText=" &#187; " NextPageText=" &#62; " PreviousPageText=" &#60; " />
            </asp:GridView>
         </div>
     </div>
     </ContentTemplate>
 </asp:UpdatePanel>
</asp:Content>
