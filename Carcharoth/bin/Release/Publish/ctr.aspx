<%@ Page Title="Веб-сервис: Каталог клиентских IT-сервисов / Телефонный справочник ЦА,ТОФК" Language="C#" ValidateRequest="false" MasterPageFile="~/index.Master" AutoEventWireup="true" CodeBehind="ctr.aspx.cs" Inherits="Carcharoth.ctr" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <div class="main" style="height:100%">
            <asp:ScriptManager runat="server" ID="ctrw"></asp:ScriptManager>
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
                    <asp:Panel runat="server" DefaultButton="SearchtelBtn">
                    <div>
                        <div style="width:100%;text-align:center;padding-top:10px;">
                            <div style="text-align:center;width:100%;margin-bottom:10px">
                                    <h4>Каталог клиентских IT-сервисов / Телефонный справочник ЦА,ТОФК</h4>
                                <div style="display: table;width:100%;font-size:12px">
                                        <div style="display: table-row;">
                                            <div style="padding:10px;display:table-cell;text-align:center;">
                                                <div style="display:inline-block;text-align:left;vertical-align:top;padding-left:50px;">
                                                <div style="text-align:center;">Информация
                                                    <div style="margin:0 20% 0 20%; border-bottom:1px solid gray;"></div>
                                                </div>
                                                    🔥 Что бы отобразить весь список групп поддержки - оставьте поле пустым и нажмите поиск по каталогу<br />
                                                    🔥 Для поиска в телефонном справочнике - достаточно ввести номер телефона, либо ФИО или адрес эл.почты
                                                </div>
                                                <div style="display:inline-block;text-align:left;vertical-align:top;padding-left:50px;">
                                            </div>
                                        </div>
                                    </div>
                                    <div style="text-align:left;display:inline-flex;padding-bottom:10px;font-size:12px;">
                                        <asp:TextBox CssClass="form-control" style="width:500px;text-align:center" runat="server" ID="TextBoxSearchCCACCITSSQLS" type="search" placeholder="Поиск" aria-label="Поиск"></asp:TextBox>
                                    </div>
                                    <div>
                                        <asp:LinkButton CssClass="btn btn-outline-light" style="border-radius:0;" runat="server" ID="SearchtelBtn" OnClick="SearchtelBtn_Click">Поиск по телефонному справочнику</asp:LinkButton>
                                        <asp:LinkButton CssClass="btn btn-outline-light" style="border-radius:0;margin-right:3px;" runat="server" ID="SearchCCACCITSSQLSBtn" OnClick="SearchCCACCITSSQLSBtn_Click">Поиск по каталогу</asp:LinkButton>
                                        <br />
                                        <asp:Label runat="server" ID="LabelSearchCCACCITSSQLS"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <asp:Panel ID="PanelCCACCITSSQLSUpdate" runat="server" Enabled="false" Visible="false" style="border:1px solid gray;padding:15px">
                            Обновление каталога клиентских ИТ-сервисов, телефоный справочник ЦА, УФК (Каталог(**.**.****)_v***, tel_central_apparat, tel_ufk) в формате *xls, *xlsm <br />
                            <asp:FileUpload id="CatalogUpload" AllowMultiple="true"  CssClass="btn btn-dark btn-sm" runat="server" accept="application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,application/vnd.ms-excel.sheet.macroEnabled.12">
                            </asp:FileUpload>    
                            <asp:Button runat="server" CssClass="btn btn-outline-light" Text="Обновить каталог" ID="UploadCatalogBtn" OnClick="UploadCatalogBtn_Click" />
                            <br />
                            <asp:Label runat="server" ID="InfoAboutEXE"></asp:Label>
                        </asp:Panel>
                         <asp:GridView Enabled="false" Visible="false" Width="100%" ID="DataCCACCITSSQLS" 
                             CssClass="ctr-gridview gridview-selected-row-style" RowStyle-CssClass="border-dark" 
                             Font-Size="10px" runat="server" DataSourceID="DataCCACCITSSQLSource" 
                             AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="Совпадений в 'Каталоге клиентских ИТ-сервисов' - не найдено">  
                                <Columns>
                                    <asp:BoundField DataField="ID" HeaderText="ID"/>
                                    <asp:BoundField DataField="IDs" HeaderText="№ Ком."/>
                                    <asp:TemplateField HeaderText="Полное название Команды">
                                        <ItemTemplate>
                                            <a href="javascript:void(0);" onclick="fallbackCopyTextToClipboard('<%# Eval("Groups") %>')">
                                                        <img class="copy-btn" src='/images/copy-min.png' />
                                            </a>
                                            <%# Eval("Groups") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Сервисы (ИТ-системы)">
                                        <ItemTemplate>
                                            <a href="javascript:void(0);" onclick="fallbackCopyTextToClipboard('<%# Eval("Services") %>')">
                                                        <img class="copy-btn" src='/images/copy-min.png' />
                                            </a>
                                            <%# Eval("Services") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Функция">
                                        <ItemTemplate>
                                            <p>
                                              <a class="btn btn-outline-light btn-ctr-btn" data-toggle="collapse" href='#Collapse_Function_<%# Eval("ID") %>' role="button" aria-expanded="false" aria-controls='Collapse_Function_<%# Eval("ID") %>'>
                                                Показать
                                              </a>
                                            </p>
                                            <div class="collapse" id='Collapse_Function_<%# Eval("ID") %>'>
                                              <%# ((string)Eval("Function")) %>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="LeaderGroups" HeaderText="Руководитель службы поддержки"/>
                                    <asp:BoundField DataField="LeaderTeam" HeaderText="ФИО лидера Команды, № тел. Лидера"/>
                                    <asp:BoundField DataField="MITeam" HeaderText="ФИО МИ Команды, № тел. МИ"/>
                                    <asp:BoundField DataField="DistributionGroup" HeaderText="Группа рассылки"/>
                                    <asp:TemplateField HeaderText="Состав Команды (Список специалистов)">
                                        <ItemTemplate>
                                            <p>
                                              <a class="btn btn-outline-light btn-ctr-btn" data-toggle="collapse" href='#Collapse_EmployeesTeam_<%# Eval("ID") %>' role="button" aria-expanded="false" aria-controls='Collapse_EmployeesTeam_<%# Eval("ID") %>'>
                                                Показать
                                              </a>
                                            </p>
                                            <div class="collapse" id='Collapse_EmployeesTeam_<%# Eval("ID") %>'>
                                              <%# Eval("EmployeesTeam") %>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="E-mail интернет=УЗ">
                                        <ItemTemplate>
                                            <p>
                                              <a class="btn btn-outline-light btn-ctr-btn" data-toggle="collapse" href='#Collapse_EmailsTeam_<%# Eval("ID") %>' role="button" aria-expanded="false" aria-controls='Collapse_EmailsTeam_<%# Eval("ID") %>'>
                                                Показать
                                              </a>
                                            </p>
                                            <div class="collapse" id='Collapse_EmailsTeam_<%# Eval("ID") %>'>
                                              <%# Eval("EmailsTeam") %>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="EmailsTeam" HeaderText="" Visible="false"/>
                                </Columns>    
                        </asp:GridView>  
                        <asp:GridView Enabled="false" Visible="false" Width="100%" ID="Datatel"
                            CssClass="ctr-gridview gridview-selected-row-style" RowStyle-CssClass="border-dark"
                            Font-Size="10px" runat="server" DataSourceID="DataCCACCITSSQLSource"
                            AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="Совпадений в 'Телефонном справочнике ЦА, УФК' - не найдено">
                            <Columns>
                                <asp:BoundField DataField="Id" HeaderText="ID" />
                                <asp:BoundField DataField="Target" HeaderText="Орган" />
                                <asp:BoundField DataField="Position" HeaderText="Должность" />
                                <asp:TemplateField HeaderText="ФИО">
                                    <ItemTemplate>
                                        <a href="javascript:void(0);" onclick="fallbackCopyTextToClipboard('<%# Eval("FIO") %>')">
                                            <img class="copy-btn" src='/images/copy-min.png' />
                                        </a>
                                        <%# Eval("FIO") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Phone_City" HeaderText="Телефон" />
                                <asp:BoundField DataField="Phone_Inside" HeaderText="ВТС" />
                                <asp:BoundField DataField="Address" HeaderText="Адрес" />
                                <asp:TemplateField HeaderText="Управление">
                                    <ItemTemplate>
                                        <div class="div-division">
                                            <asp:LinkButton CssClass="div-division" runat="server" OnClick="SearchtelBtn_Click" ID="DivisionSearch" Text=<%# Eval("Division") %>>
                                                <%# Eval("Division") %>
                                            </asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Отдел">
                                    <ItemTemplate>
                                        <div class="div-division">
                                            <asp:LinkButton CssClass="div-division" runat="server" OnClick="SearchtelBtn_Click" ID="DepartmentSearch" Text=<%# Eval("Department") %>>
                                                <%# Eval("Department") %>
                                            </asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Почта">
                                    <ItemTemplate>
                                        <a href="javascript:void(0);" onclick="fallbackCopyTextToClipboard('<%# Eval("Email") %>')">
                                            <img class="copy-btn" src='/images/copy-min.png' />
                                        </a>
                                        <%# Eval("Email") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="DataCCACCITSSQLSource" runat="server" ConnectionString="<%$ ConnectionStrings:ToCatalogDB %>"></asp:SqlDataSource>  
                    </div>
                    </asp:Panel>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID = "UploadCatalogBtn" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
</asp:Content>
