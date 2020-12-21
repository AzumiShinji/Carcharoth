<%@ Page Title="Веб-сервис: Поиск организаций" Async="true" Language="C#" MasterPageFile="~/index.Master" AutoEventWireup="true" CodeBehind="org.aspx.cs" Inherits="Carcharoth.org" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager runat="server" ID="ors"></asp:ScriptManager>
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
    <asp:Panel ID="PanelOrgs" runat="server" defaultbutton="SearchOrgsBtn">
                    <table style="width:100%;text-align:center">
                    <tr>
                        <td>
                            <asp:Panel runat="server" style="height:100%">
                                <div style="padding:20px;">
                                    <input type="hidden" value="" name="clientScreenWidth" id="clientScreenWidth" />
                                    <h4>Поиск организаций</h4>
                                        <div style="display: table;width:100%;font-size:12px">
                                                <div style="display: table-row;">
                                                    <div style="padding:10px;display:table-cell;text-align:center;">
                                                        <div style="display:inline-block;text-align:left;vertical-align:top">
                                                            <div style="text-align:center;">Поиск по коду
                                                                <div style="margin:0 20% 0 20%; border-bottom:1px solid gray;"></div>
                                                            </div>
                                                           • ИНН, СВР, ОГРН, КПП, ОКПО, ПГМУ
                                                        </div>
                                                        <div style="display:inline-block;text-align:left;vertical-align:top;padding-left:50px;">
                                                        <div style="text-align:center;">Поиск по наименованию
                                                            <div style="margin:0 20% 0 20%; border-bottom:1px solid gray;"></div>
                                                        </div>
                                                            • Максимум - 100 результатов, для уточнения результатов - уточняйте запрос <br />
                                                            • Пробуйте использовать ключевые слова из наименования: город, муниципальное, государственное и.т.д...<br />
                                                        </div>
                                                        <div style="display:inline-block;text-align:left;vertical-align:top;padding-left:50px;">
                                                        <div style="text-align:center;">Статус
                                                            <div style="margin:0 20% 0 20%; border-bottom:1px solid gray;"></div>
                                                        </div>
                                                            • Статус: <asp:Label ID="StatusOrg" runat="server" Text="offline"></asp:Label><br />
                                                            • Количество записей: <asp:Label ID="CountOrg" runat="server"></asp:Label><br />
                                                            • Дата обновления: <asp:Label ID="DateOrg" runat="server"></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                        </div>
                                    <div style="width:50%;float:none;margin: 0 auto;text-align:center;">
                                        <input style="display: none;" type="password" name="pwdplaceholder"/>
                                        <asp:TextBox runat="server" PlaceHolder="Код / Наименование" CssClass="form-control" ID="SearchOrgsTextBox" style="text-align:center;margin-bottom:5px"></asp:TextBox>
                                        <div style="text-align:center">
                                                <asp:LinkButton runat="server" ID="SearchOrgsBtn" CssClass="btn btn-sm btn-outline-light" style="float:left;width:48%;border-radius:0" OnClick="SearchOrgsBtn_Click">Искать по коду</asp:LinkButton>
                                                <asp:LinkButton runat="server" ID="SearchOrgByNameBtn" CssClass="btn btn-sm btn-outline-light" style="float:right;width:48%;border-radius:0" OnClick="SearchOrgByNameBtn_Click">Искать по наименованию</asp:LinkButton>
                                        </div>
                                    </div>
                                    <br /><br />
                                    <b>Найдено организаций: <%= DataOrg.Items.Count %></b>
                                    <div style="border-bottom:1px solid white;margin:0 20% 0 20%"></div>
                                </div>
                                <div style="display:inline-block;text-align:left">
                                    <asp:DataList ID="DataOrg" runat="server" DataSourceID="DataOrgsSqlDataSource" CellSpacing="4" RepeatLayout="Table">  
                                    <ItemTemplate>  
                                        <table class="table-org">
                                            <tr>  
                                                <th style="position:absolute;margin:5px">
                                                    <a href="javascript:void(0);" onclick="fallbackCopyTextToClipboard('<%#
                                                    "_________________________"+ "%048%"+
                                                    "Информация о пользователе: "+"%048%"+
                                                    "ИНН: " + Eval("inn")+"%048%"+
                                                    "СВР: " + Eval("code")+"%048%"+
                                                    "ОГРН: " + Eval("ogrn")+"%048%"+
                                                    "КПП: " + Eval("kpp")+"%048%"+
                                                    "ОКПО: " + Eval("okpoCode")+"%048%"+
                                                    "ПГМУ: " + Eval("pgmu")+"%048%"+
                                                    "Наименование: " + Eval("fullName").ToString().Replace("\"","%049%")+"%048%"+
                                                    "Сокр. наименование: " + Eval("shortName").ToString().Replace("\"","%049%")+"%048%"+
                                                    "ФИО руководителя: " + Eval("fio")+"%048%"+
                                                    "Номер реестровой записи: " + Eval("recordNum")+"%048%"+
                                                    "Город: " + Eval("cityName")+"%048%"+
                                                    "Наименование УФК: " + Eval("orfkName")+"%048%"+
                                                    "Номер УФК: " + Eval("orfkCode")+"%048%"+
                                                    "Номер телефона: " + Eval("phone")+"%048%"+
                                                    "Почта: " + Eval("mail")
                                                    %>')">
                                                        <img class="copy-btn" src='/images/copy-min.png' />
                                                    </a>
                                                </th>
                                                <%# (string)Eval("statusName")=="действующая" ? ""+
                                                         "<th colspan='1'  style='color:green;text-align:center'>"+
                                                  Eval("statusName")+
                                                "</th>  ":""%>
                                                <%# (string)Eval("statusName")=="недействующая" ? ""+
                                                         "<th colspan='1'  style='color:red;text-align:center'>"+
                                                  Eval("statusName")+
                                                "</th>  ":""%>
                                                <%# (string)Eval("statusName")=="Специальные указания" ? ""+
                                                         "<th colspan='1'  style='color:blue;text-align:center'>"+
                                                  Eval("statusName")+
                                                "</th>  ":""%>
                                            </tr>  
                                            <tr>  
                                                <td colspan="2" style="padding:10px"> 
                                                    <b>ИНН: </b><%# Eval("inn") %></br>
                                                    <b>СВР: </b><%# Eval("code") %></br>
                                                    <b>ОГРН: </b><%# Eval("ogrn") %></br>
                                                    <b>КПП: </b><%# Eval("kpp") %></br>
                                                    <b>ОКПО: </b><%# Eval("okpoCode") %></br>
                                                    <b>ПГМУ: </b><%# Eval("pgmu") %></br>
                                                    <b>Наименование: </b><%# Eval("fullName").ToString().Replace('\"','\'') %></br>
                                                    <b>Сокр. наименование: </b><%# Eval("shortName").ToString().Replace('\"','\'') %></br>
                                                    <b>ФИО руководителя: </b><%# Eval("fio") %></br>
                                                    <b>Номер реестровой записи: </b><%# Eval("recordNum") %></br>
                                                    <b>Город: </b><%# Eval("cityName") %></br>
                                                    <b>Наименование УФК: </b><%# Eval("orfkName") %></br>
                                                    <b>Номер УФК: </b><%# Eval("orfkCode") %></br>
                                                    <b>Номер телефона: </b><%# Eval("phone") %></br>
                                                    <b>Почта: </b><%# Eval("mail") %>
                                                </td>  
                                            </tr>  
                                        </table>  
                                        </div>
                                    </ItemTemplate>  
                                    <FooterTemplate>
                                        <asp:Label Visible='<%#bool.Parse((DataOrg.Items.Count==0).ToString())%>' 
               runat="server" ID="lblNoRecord">
                                            Совпадений не найдено!<br />
                                        </asp:Label>
                                    </FooterTemplate>
                                </asp:DataList>  
                                </div>
                                <asp:SqlDataSource ID="DataOrgsSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ToOrgsDB %>"></asp:SqlDataSource>  
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
                </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
