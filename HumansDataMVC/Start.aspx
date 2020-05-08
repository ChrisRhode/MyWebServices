<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Start.aspx.cs" Inherits="HumansDataMVC.Start" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MVC Coding Example</title>
</head>
<body>
    <h1>MVC coding example</h1>
    This website implements a Model View Controller for hypothetical human data.  Each human has a unique ID
    (fake Social Security Number, please do not use real SSN's!), some attributes (e.g. Name), and 0 or more Children.
    Children are implemented as a collection of Humans.
    <form id="form1" runat="server">
        <div>
           
        </div>
        <asp:LinkButton ID="linkStartWithSampleData" runat="server" OnClick="linkStartWithSampleData_Click">Start with DB loaded with sample data (5 humans, 2 are children of one of them)</asp:LinkButton><br />
        <asp:LinkButton ID="linkStartWithEmptyDB" runat="server" OnClick="linkStartWithEmptyDBClick">Start with empty DB</asp:LinkButton>
    </form>
</body>
</html>
