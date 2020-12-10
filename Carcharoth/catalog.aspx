<%@ Page Title="Веб-сервис: База знаний" ValidateRequest="false" Language="C#" AutoEventWireup="true" MasterPageFile="~/index.Master" CodeBehind="catalog.aspx.cs" Inherits="Carcharoth.catalog" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">   
    <asp:ScriptManager runat="server" Id="ctl"></asp:ScriptManager>
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
    <asp:UpdatePanel runat="server" UpdateMode="Always" ID="updctl">
        <ContentTemplate>
            <asp:Panel runat="server" DefaultButton="btnSearch">
            <div class="search-control-main">
                <div class="custom-control-inline search-control">
                        <asp:TextBox runat="server" ID="textboxSearch" CssClass="form-control" type="search" placeholder="Поиск в базе знаний" aria-label="Поиск в базе знаний" />
                        <asp:LinkButton runat="server" ID="btnSearch" CssClass="" OnClick="btnSearch_Click"></asp:LinkButton>
                </div>  
            </div>
            <div class="main-body">
                <asp:Panel ID="PanelCatalog" runat="server" defaultbutton="btnSearch">
                <table style="width:100%">
                    <tr>
                        <td>
                            <asp:Panel runat="server" style="z-index:1;width:25%; height:100%; position:fixed;padding-bottom:56px;">
                            <asp:TreeView ID="CatalogTree" runat="server" CssClass="catalog-scrollbar"
                                OnSelectedNodeChanged="CatalogTree_SelectedNodeChanged"
                                SelectedNodeStyle-CssClass="catalog-scrollbar-selected"
                                NodeStyle-ImageUrl="/images/catalog-tree-node.png"
                                CollapseImageUrl="/images/catalog-tree-expand-1.png"
                                ExpandImageUrl="/images/catalog-tree-expand-2.png">
	                            <Nodes>
                                    <asp:TreeNode Text="Главная страница" Selected="true" ImageUrl="/images/catalog-tree-home.png"/>
                                    <asp:TreeNode Text="Общая информация" ImageUrl="/images/catalog-tree-info.png"/>
                                    <asp:TreeNode Text="Решения на 1-ой линии" SelectAction="Expand" Expanded="false" ImageUrl="/images/catalog-tree-firstline_.png">
                                        <asp:TreeNode Text="ДС ФК" ImageUrl="/images/catalog-tree-dsfk.png" />
                                        <asp:TreeNode Text="1С" SelectAction="SelectExpand" ImageUrl="/images/catalog-tree-1c.png">
                                            <asp:TreeNode Text="БУ ФОИВ" />
                                        </asp:TreeNode>
	                                    <asp:TreeNode Text="ЭБ" SelectAction="SelectExpand" ImageUrl="/images/catalog-tree-eb.png">
                                            <asp:TreeNode Text="ЭБ - ЕТД Портал" />
                                            <asp:TreeNode Text="ЭБ - Ведение НСИ ФК" />
                                            <asp:TreeNode Text="ЭБ - Единый портал бюджетной системы Российской Федерации" />
                                            <asp:TreeNode Text="ЭБ - НСИ_Базовые (отраслевые) и Ведомственные перечни государственных услуг (работ)" />
                                            <asp:TreeNode Text="ЭБ - НСИ_Сводный реестр" />
                                            <asp:TreeNode Text="ЭБ - Подсистема информационно-аналитического обеспечения" />
                                            <asp:TreeNode Text="ЭБ - Подсистема обеспечения интеграции, ведения реестров и формуляров" />
                                            <asp:TreeNode Text="ЭБ - Подсистема обеспечения информационной безопастности" />
                                            <asp:TreeNode Text="ЭБ - Подсистема обеспечения юридической значимости электронных документов" />
                                            <asp:TreeNode Text="ЭБ - Подсистема управления денежными средствами"/>
                                            <asp:TreeNode Text="ЭБ - Подсистема управления закупками" />
                                            <asp:TreeNode Text="ЭБ - Подсистема управления расходами" />
                                            <asp:TreeNode Text="ЭБ - Подсистема управления расходами в части КС"/>
                                            <asp:TreeNode Text="ЭБ - Подсистема учета и отчетности в части обеспечения централизованного составления, представления, свода и консолидации отчетности об исполнении федерального бюджета и бухгалтерской отчетности федеральных бюджетных и автономных учреждений системы" /> 
                                        </asp:TreeNode>
                                        <asp:TreeNode Text="Казначейское сопровождение" ImageUrl="/images/catalog-tree-ks.png" />
                                        <asp:TreeNode Text="ГАСУ" ImageUrl="/images/catalog-tree-gasu.png" />
                                        <asp:TreeNode Text="ГИС ГМП" ImageUrl="/images/catalog-tree-gmp.png" SelectAction="SelectExpand" />
                                        <asp:TreeNode Text="ГИС ГМУ" ImageUrl="/images/catalog-tree-gmu.png"/>
                                        <asp:TreeNode Text="СУФД" ImageUrl="/images/catalog-tree-sufd.png"/>
                                        <asp:TreeNode Text="Удостоверяющий центр" ImageUrl="/images/catalog-tree-fzs.png"/>
                                        <asp:TreeNode Text="Управление делами" ImageUrl="/images/catalog-tree-ud.png"/>
                                    </asp:TreeNode>
	                                <asp:TreeNode Text="Справочная информация" SelectAction="Expand" Expanded="false" ImageUrl="/images/catalog-tree-reference.png">
	                                    <asp:TreeNode Text="ДС ФК" ImageUrl="/images/catalog-tree-dsfk.png" />
                                        <asp:TreeNode Text="1С" SelectAction="SelectExpand" ImageUrl="/images/catalog-tree-1c.png">
                                            <asp:TreeNode Text="БУ ФОИВ" />
                                        </asp:TreeNode>
	                                    <asp:TreeNode Text="ЭБ" SelectAction="SelectExpand" ImageUrl="/images/catalog-tree-eb.png">
                                            <asp:TreeNode Text="ЭБ - ЕТД Портал" />
                                            <asp:TreeNode Text="ЭБ - Ведение НСИ ФК" />
                                            <asp:TreeNode Text="ЭБ - Единый портал бюджетной системы Российской Федерации" />
                                            <asp:TreeNode Text="ЭБ - НСИ_Базовые (отраслевые) и Ведомственные перечни государственных услуг (работ)" />
                                            <asp:TreeNode Text="ЭБ - НСИ_Сводный реестр" />
                                            <asp:TreeNode Text="ЭБ - Подсистема информационно-аналитического обеспечения" />
                                            <asp:TreeNode Text="ЭБ - Подсистема обеспечения интеграции, ведения реестров и формуляров" />
                                            <asp:TreeNode Text="ЭБ - Подсистема обеспечения информационной безопастности" />
                                            <asp:TreeNode Text="ЭБ - Подсистема обеспечения юридической значимости электронных документов" />
                                            <asp:TreeNode Text="ЭБ - Подсистема управления денежными средствами"/>
                                            <asp:TreeNode Text="ЭБ - Подсистема управления закупками" />
                                            <asp:TreeNode Text="ЭБ - Подсистема управления расходами" />
                                            <asp:TreeNode Text="ЭБ - Подсистема управления расходами в части КС"/>
                                            <asp:TreeNode Text="ЭБ - Подсистема учета и отчетности в части обеспечения централизованного составления, представления, свода и консолидации отчетности об исполнении федерального бюджета и бухгалтерской отчетности федеральных бюджетных и автономных учреждений системы" /> 
                                        </asp:TreeNode>
                                        <asp:TreeNode Text="Казначейское сопровождение" ImageUrl="/images/catalog-tree-ks.png" />
                                        <asp:TreeNode Text="ГАСУ" ImageUrl="/images/catalog-tree-gasu.png" />
                                        <asp:TreeNode Text="ГИС ГМП" ImageUrl="/images/catalog-tree-gmp.png" SelectAction="SelectExpand">
                                            <asp:TreeNode Text="СМЭВ" />
                                        </asp:TreeNode>
                                        <asp:TreeNode Text="ГИС ГМУ" ImageUrl="/images/catalog-tree-gmu.png"/>
                                        <asp:TreeNode Text="СУФД" ImageUrl="/images/catalog-tree-sufd.png"/>
                                        <asp:TreeNode Text="Удостоверяющий центр" ImageUrl="/images/catalog-tree-fzs.png"/>
                                        <asp:TreeNode Text="Управление делами" ImageUrl="/images/catalog-tree-ud.png"/>
                                        <asp:TreeNode Text="Менеджер инцидентов" ImageUrl="/images/catalog-tree-mi.png"/>
	                                </asp:TreeNode>
	                                <asp:TreeNode Text="Другие проекты" ImageUrl="/images/catalog-tree-others.png"/>
	                            </Nodes>
                            </asp:TreeView>
                                <asp:HiddenField runat="server" Value="false" ID="_NextTurnOn" />
                                <asp:HiddenField runat="server" Value="false" ID="_PrevTurnOn" />
                                <asp:HiddenField runat="server" Value="0" ID="_offset" />
                                <asp:HiddenField runat="server" Value="25" ID="_fetch" />
                                <asp:HiddenField runat="server" Value="0" ID="_pagenum" />
                            </asp:Panel>
                        </td>
                        <td>
                            <asp:Panel runat="server" style="z-index:3;height:100%;position:absolute;margin-left:-24.2%;width:74%;margin-top:5px">
                                <asp:Label runat="server" Visible="false" ID="SearchLabelText" Style="font-size:30px;font-weight:bold"></asp:Label>
                                <div style="width:100%">
                                    <div style="text-align:center;">
                                        <div style="display:inline-block">
                                            <asp:Panel ID="PanelCurrentProjectText" runat="server" Visible="false" style="padding-bottom:10px">
                                                <asp:Image runat="server" ID="ImageCurrentProjectText" />
                                                <asp:Label ID="CurrentProjectText" runat="server"></asp:Label>
                                                <div style="margin:0 20% 0 20%; border-bottom:1px solid gray;padding-top:5px;"></div>
                                            </asp:Panel>
                                            <img src="<%= CatalogTree.SelectedNode.ImageUrl != String.Empty ? CatalogTree.SelectedNode.ImageUrl:CatalogTree.SelectedNode.Parent.ImageUrl %>"/>
                                            <asp:LinkButton runat="server" ID="AddPost" Enabled="false" Visible="false" CssClass="btn btn-outline-warning btn-sm" style="margin-bottom:10px;border-radius:0" OnClick="AddPost_Click">
                                                <%= CatalogTree.SelectedNode.Text %>: Добавить статью
                                                <img runat="server" src="/images/post-add.png" style="padding-right:5px;padding-bottom:2px;" />
                                            </asp:LinkButton>
                                            <asp:Label runat="server" ID="SelectedNodeReplacePosts" Enabled="false" Visible="false"><%= CatalogTree.SelectedNode.Text %></asp:Label>
                                        </div>
                                    </div>
                                </div>
                                <asp:Panel style="font-size:10px;" runat="server" id="PanelKuratorsProject" Visible="false">
                                    <b>Кураторы: </b><asp:Label runat="server" ID="KuratorsProject"></asp:Label>
                                </asp:Panel>
                                <div style="width:100%;padding:10px">
                                    <div style="text-align:center">
                                        <asp:LinkButton runat="server" ID="PrevPageBtnTop" CssClass="btn btn-dark btn-sm" OnClick="PrevPageBtn_Click"><</asp:LinkButton>
                                        <asp:Label runat="server" ID="PageNumLabelTop"></asp:Label>
                                        <asp:LinkButton runat="server" ID="NextPageBtnTop" CssClass="btn btn-dark btn-sm" OnClick="NextPageBtn_Click">></asp:LinkButton>
                                    </div>
                                </div>
                                <asp:DataList ID="DataTree" runat="server" DataSourceID="DataTreeSQLSource" RepeatColumns="1"
                                    CellSpacing="1" RepeatLayout="Table" style="width:100%;">  
                                    <ItemTemplate>  
                                        <table class="custom-table-catalog">  
                                            <tr>  
                                                <th colspan="2" id="catalog-table-header">
                                                    <%# (int)Eval("isMainPage")==1 ? "<div class='ToolTip-Posts'><span>На главной странице</span><img runat='server' width=25 src='/images/bookmark-red-mainpage.png'></div>" : "" %>
                                                    <%# (int)Eval("isImportant")==1 ? "<div class='ToolTip-Posts'><span>Закреплено</span><img runat='server' width=25 src='/images/bookmark-yellow-pinned.png'></div>" : "" %>
                                                    <%# (int)Eval("IsFirstLine")==1 ? "<div class='ToolTip-Posts'><span>Рекомендация для решения на 1-ой линии</span><img runat='server' width=25 src='/images/bookmark-green-firstline.png'></div>" : "" %>
                                                    <%# Eval("IT") %>
                                                    <div class="catalog-line"></div>
                                                    <div style="display:inline-block;width:100%">
                                                        <div style="display:inline-block;font-weight:bold;width:90%">
                                                            <img src="/images/number-post.png"/><%# Eval("ID") %>&#5011;
                                                            <%# Eval("Head").ToString().Trim() == "" ? "<Без темы>" : (string)Eval("Head") %>
                                                        </div>
                                                        <div style="display:inline-block;float:right">
                                                            <%# AccessITPosts(CatalogTree.SelectedNode) ?
                                                                "<a class='btn btn-outline-light btn-small-catalog-change open_modal' data-toggle='modal' " +
                                                                "data-postid='"+ Eval("Id")+"'" +
                                                                "data-it='"+ Eval("IT")+"'" +
                                                                "data-isimportant='"+ Eval("isImportant")+"'" +
                                                                "data-ismainpage='"+ Eval("isMainPage")+"'" +
                                                                "data-isfirstline='"+ Eval("isFirstLine")+"'" +
                                                                "data-head='"+ Eval("Head")+"'" +
                                                                "data-text='"+ Eval("Text")+"'" +
                                                                "data-keywords='"+ Eval("KeyWords")+"'" +
                                                                "data-sd='"+ Eval("SD")+"'" +
                                                                "data-target='#EAModal' href='#EAModal'><img runat='server' src='/images/edit.png' /> Править</a>"
                                                                :"" %>
                                                        </div>
                                                    </div>
                                                    <br />
                                                </th>  
                                            </tr>  
                                            <tr>  
                                                <td colspan="2" class="catalog-text">  
                                                    <%# Eval("Head").ToString().Trim() == "" ? null : "<div style='font-size:26px;font-weight:bolder;'>"+((string)Eval("Head")).Trim()+"</div>" %>
                                                    <div class="custom-table-catalog-p catalog-cut-text" id='catalog_text_div_<%# Eval("Id") %>'>
                                                        <%# ((string)Eval("Text")).Trim() %>
                                                    </div>
                                                    <div id="three_dots_<%# Eval("Id") %>">...</div>
                                                    <a class="catalog-text-anchor-show"
                                                        id="catalog_text_a_<%# Eval("Id") %>"  
                                                        onclick="javascript:document.getElementById('catalog_text_div_<%# Eval("Id") %>').classList.remove('catalog-cut-text');document.getElementById('catalog_text_a_<%# Eval("Id") %>').hidden=true;document.getElementById('three_dots_<%# Eval("Id") %>').hidden=true;return false;">
                                                        <img runat="server" src="/images/catalog-show.png" />
                                                        Показать больше
                                                    </a>
                                                    <p style="font-size:9px;margin:20px 0 0 0;font-style:italic;border-top:1px solid #e4e1e1;"><b>Ключевые слова:</b> <%# Eval("KeyWords") %></p> 
                                                    <p style="font-size:9px;font-style:italic;"><b>Пример SD:</b> <%# Eval("SD") %></p><br />  
                                                    <div style="font-size:9px;color:gray;text-align:right;font-style:italic;font-weight:lighter">
                                                    Автор: <%# Eval("WhoCreated") == DBNull.Value ? "Неизвестно": ConvertFIO((string)Eval("WhoCreated")) %> 
                                                        (<%# Eval("WhenCreated") == DBNull.Value ? "Неизвестно":Eval("WhenCreated") %>),
                                                    Последнее изменение: <%# Eval("Who") == DBNull.Value ? String.Empty : ConvertFIO((string)Eval("Who")) %> <%# Eval("DateTime") == DBNull.Value ? "Нет" : " ("+Eval("DateTime")+")" %>
                                                    </div>
                                                </td>  
                                            </tr>  
                                        </table>  
                                    </ItemTemplate>  
                                </asp:DataList>  
                                <asp:SqlDataSource ID="DataTreeSQLSource" runat="server" ConnectionString="<%$ ConnectionStrings:ToCatalogDB %>"></asp:SqlDataSource>  
                                <div style="width:100%;padding:10px;">
                                    <div style="text-align:center">
                                <asp:LinkButton runat="server" ID="PrevPageBtn" CssClass="btn btn-dark btn-sm" OnClick="PrevPageBtn_Click"><</asp:LinkButton>
                                <asp:Label runat="server" ID="PageNumLabel"></asp:Label>
                                <asp:LinkButton runat="server" ID="NextPageBtn" CssClass="btn btn-dark btn-sm" OnClick="NextPageBtn_Click">></asp:LinkButton>
                                    </div>
                                </div>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            </div>
        </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </asp:Content>