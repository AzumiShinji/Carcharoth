<%@ Page Title="Веб-сервис: Игры" Language="C#" ValidateRequest="false" MasterPageFile="~/index.Master" AutoEventWireup="true" CodeBehind="games.aspx.cs" Inherits="Carcharoth.games" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <div class="main" style="height:100%">
            <asp:ScriptManager runat="server" ID="gamessc"></asp:ScriptManager>
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
                    <link rel="stylesheet" runat="server" media="screen" href="css/knt9.css" />
                   <%-- <script src="js/knt9.js"></script>
                    <div id="knt9">
                        <div class='a' id="source-pidor" onclick="addNew(this);"></div>
                    </div>--%>
                    <div style="width:100%;margin-top:200px">
                        <div style="text-align:center;width:30%;margin:0 auto">
                            <asp:Panel ID="gameslinkspnl" runat="server" style="text-align:center;">

                            </asp:Panel>
                        </div>
                     </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
</asp:Content>
