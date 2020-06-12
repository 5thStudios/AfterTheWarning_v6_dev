<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UpdateMsgs.ascx.cs" Inherits="afterthewarning.com.UserControls.UpdateMsgs" %>

<h1>Update Msgs</h1>
<hr />

<form runat="server">
    <asp:Label runat="server" ID="lblErrors" ForeColor="IndianRed" />
    <asp:Label runat="server" ID="lblResults" ForeColor="CadetBlue" />
    <hr />

    <code runat="server" id="codeB4" style="color:indianred" />
    <hr />
    <code runat="server" id="codeAfter" style="color:cadetblue;" />
    
    <br />
    <br />
    Total Records: <asp:Label runat="server" ID="lblTotalRecords" ForeColor="ForestGreen" Font-Bold="true" /><br />
    Records Updated: <asp:Label runat="server" ID="lblRecordCount" ForeColor="ForestGreen" Font-Bold="true" /><br />
    Time to Process: <asp:Label runat="server" ID="lblTimeToProcess" ForeColor="CadetBlue" Font-Bold="true" />

    <br />
    <br />
    <asp:GridView runat="server" ID="gv1" />
    <br />
    <br />
    <asp:GridView runat="server" ID="gv2" />
    <br />
    <br />
    <asp:GridView runat="server" ID="gv3" />
</form>