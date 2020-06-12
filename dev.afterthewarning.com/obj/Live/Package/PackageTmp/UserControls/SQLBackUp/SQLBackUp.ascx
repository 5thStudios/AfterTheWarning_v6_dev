<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SQLBackUp.ascx.cs" Inherits="AllianceMedia.SQLBackUp.UserControls.SQLBackUp" %>

<style>
    html {
        padding: 0;
        margin: 0;
    }

    body {
        padding: 0;
        margin: 0;
        height: auto;
        overflow: hidden !important;
    }

    .backup-list {
        margin: 20px 0;
    }
</style>
<h3>SQL BackUp</h3>
<asp:Literal runat="server" ID="litIntro">
<p>Click the button below to run a full backup of your Umbraco MSSQL database.</p>
<p>A .bak file is created and saved in the App_Data directory in a new folder called SQLBackUp.</p>
</asp:Literal>
<asp:Literal runat="server" ID="litIntroCE">
<p>Click the button below to run a full backup of your Umbraco SQL CE database.</p>
<p>A copy of your .sdf file is created and saved in the App_Data directory in a new folder called SQLBackUp.</p>
</asp:Literal>

<asp:Button runat="server" Text="Create a Backup" ID="btnRun" OnClick="btnRun_Click" />

<asp:Literal runat="server" ID="litMsg" />

<asp:GridView ID="BackUpGridView" runat="server" AutoGenerateColumns="false" EmptyDataText="No back ups available..."
    BackColor="#FFFFFF"
    CellPadding="5"
    CellSpacing="0"
    Width="50%"
    BorderWidth="0"
    BorderColor="#333333"
    GridLines="Horizontal"
    CssClass="backup-list"
    OnRowDataBound="BackUpGridView_RowDataBound">
    <HeaderStyle
        Font-Size="16px"
        Font-Bold="true"
        BackColor="#444444"
        ForeColor="#ffffff"
        HorizontalAlign="Left"
        Height="18" />
    <RowStyle
        Font-Size="13px"
        VerticalAlign="top"
        BackColor="#eeeeee"
        ForeColor="#333333"
        HorizontalAlign="Left" />
    <AlternatingRowStyle
        BackColor="#cccccc" />
    <Columns>
        <asp:TemplateField HeaderText="Name" SortExpression="Name">
            <ItemTemplate>
                <%--<asp:LinkButton runat="server" ID="lbFolderItem" CommandName="OpenFolder" CommandArgument='<%# Eval("Name") %>'></asp:LinkButton>--%>
                <asp:Literal runat="server" ID="litFileItem"></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>

        <%-- <asp:BoundField DataField="Text" HeaderText="File Name" />--%>

        <asp:BoundField DataField="FileSystemType" HeaderText="Type" SortExpression="FileSystemType" />
        <asp:TemplateField HeaderText="Size" SortExpression="Size">
            <ItemTemplate>
                <%# DisplaySize((long?) Eval("Size")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField>
            <ItemTemplate>
                <asp:LinkButton ID="lnkDownload" Text="Download" CommandArgument='<%# Eval("Name") %>' runat="server" OnClick="DownloadFile" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:LinkButton ID="lnkDelete" Text="Delete" CommandArgument='<%# Eval("Name") %>' runat="server" OnClick="DeleteFile" OnClientClick="return confirm('Are you sure you wish to delete this file?');" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

<asp:Button runat="server" ID="btnDelAll" Text="Delete All BackUp Files" CssClass="footer-button" OnClientClick="return confirm('Are you sure you wish to delete all files?');" />
<asp:Button runat="server" ID="btnDownloadAll" Text="Download All BackUp Files" CssClass="footer-button" />
<p style="margin-top: 10px;">Version: 1.3</p>
