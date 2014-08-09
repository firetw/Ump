<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <div style="text-align: center">
            <table style="text-align: center; width: 200px" align="center">
                <tr>
                    <td><span>amount:</span></td>
                    <td>
                        <asp:TextBox ID="tbAmount" runat="server" Text="100"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><span>amtType:</span></td>
                    <td>
                        <asp:TextBox ID="tbAmtType" runat="server" Text="02"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><span>bankType:</span></td>
                    <td>
                        <asp:TextBox ID="tbBankType" runat="server" Text="3"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><span>expand:</span></td>
                    <td>
                        <asp:TextBox ID="tbExpand" runat="server" Text=""></asp:TextBox></td>
                </tr>
                <tr>
                    <td><span>goodsId:</span></td>
                    <td>
                        <asp:TextBox ID="tbGoodsId" runat="server" Text="001"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><span>merDate:</span></td>
                    <td>
                        <asp:TextBox ID="tbMerDate" runat="server" Text=""></asp:TextBox></td>
                </tr>
                <tr>
                    <td><span>merId:</span></td>
                    <td>
                        <asp:TextBox ID="tbMerId" runat="server" Text="6882"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><span>merPriv:</span></td>
                    <td>
                        <asp:TextBox ID="tbMerPriv" runat="server" Text=""></asp:TextBox></td>
                </tr>
                <tr>
                    <td><span>mobileId:</span></td>
                    <td>
                        <asp:TextBox ID="tbMobileId" runat="server" Text="13525510756"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><span>notifyUrl:</span></td>
                    <td>
                        <asp:TextBox ID="tbNotifyUrl" runat="server" Text=""></asp:TextBox></td>
                </tr>
                <tr>
                    <td><span>orderId:</span></td>
                    <td>
                        <asp:TextBox ID="tbOrderId" runat="server" Text=""></asp:TextBox></td>
                </tr>
                <tr>
                    <td><span>version:</span></td>
                    <td>
                        <asp:TextBox ID="tbVersion" runat="server" Text="3.0"></asp:TextBox></td>
                </tr>
            </table>

            <br />
            <br />
            <asp:Label ID="lbMsg" runat="server" Text=""></asp:Label>
            <br />
            <br />
            <asp:Button ID="btTest" Text="测试" runat="server" Width="100" Height="30" OnClick="btTest_Click" />
        </div>
    </form>
</body>
</html>
