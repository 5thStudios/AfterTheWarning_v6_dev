<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImportScriptures.ascx.cs" Inherits="dev.afterthewarning.com.UserControls.ImportScriptures" %>


<h1>Import Scriptures</h1>
<hr />

<form runat="server">
    <asp:Label runat="server" ID="lblErrors" ForeColor="IndianRed" />
    <asp:Label runat="server" ID="lblResults" ForeColor="CadetBlue" />
    <br />
    <br />
    Records: <asp:Label runat="server" ID="lblRecordCount" ForeColor="ForestGreen" Font-Bold="true" /><br />
    Time to Process: <asp:Label runat="server" ID="lblTimeToProcess" ForeColor="CadetBlue" Font-Bold="true" />
    
    <%--<br />
    <br />
    <code runat="server" id="cd"></code>
    <br />
    <br />
    <code runat="server" id="cd2"></code>--%>

    <br />
    <br />
    <asp:GridView runat="server" ID="gv1" />
    <br />
    <br />
    <asp:GridView runat="server" ID="gv2" />
    <br />
    <br />
    <asp:GridView runat="server" ID="gv3" />
    <br />
    <br />
    <asp:GridView runat="server" ID="gv4" />
</form>