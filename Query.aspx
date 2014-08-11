<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Query.aspx.cs" Inherits="Query" %>

<!DOCTYPE html>
<!--查询页面-->
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>支付数据查询</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <fieldset title="查询条件">
                <legend style="font-size: 16px; font-weight: bold">查询信息:</legend>
                <table frame="void" cellpadding="1" border="0px" cellspacing="2" style="width: 85%" align="center">
                    <tr style="background-color: #EEEEEE">
                        <td style="text-align: right; width: 15%">
                            <span>开始时间 :</span>&nbsp;
                        </td>
                        <td style="text-align: left; width: 35%">&nbsp;<asp:TextBox ID="tbStartTime" runat="server" Width="155px"></asp:TextBox>
                        </td>
                        <td style="text-align: right; width: 15%">
                            <span>结束时间 :</span>&nbsp;
                        </td>
                        <td style="text-align: left; width: 35%">&nbsp;<asp:TextBox ID="tbEndTime" runat="server" Width="155px"></asp:TextBox>
                            &nbsp;
                        </td>
                    </tr>
                    <tr style="background-color: #EEEEEE">
                        <td style="text-align: right; width: 15%">
                            <span>&nbsp;商品号 :</span>&nbsp;
                        </td>
                        <td style="text-align: left; width: 35%;">&nbsp;<asp:TextBox ID="tbGoodId" runat="server" Width="155px"></asp:TextBox>
                            &nbsp;
                        </td>
                        <td style="text-align: right; width: 15%">
                            <span>&nbsp;手机号 : </span>&nbsp;
                        </td>
                        <td style="text-align: left; width: 35%">&nbsp;<asp:TextBox ID="tbPhone" runat="server" Width="155px"></asp:TextBox>
                            &nbsp;
                        </td>
                    </tr>
                    <tr style="background-color: #EEEEEE">
                        <td colspan="4" align="center" style="height: 33px">
                            <asp:Button ID="btSearch" runat="Server" Text="确定" Width="124px" OnClick="btSearch_Click" />&nbsp;&nbsp;&nbsp;
                            &nbsp;&nbsp&nbsp;
                            &nbsp;&nbsp
                            <asp:Button ID="btExport" runat="Server" Text="导出" Width="124px" OnClick="btExport_Click" />&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                </table>
            </fieldset>

            <br />
            <br />
            <fieldset title="查询结果">
                <legend style="font-size: 16px; font-weight: bold">查询结果:</legend>
                <div style="text-align: center;">
                    <asp:GridView ID="gvQuery" runat="server" AutoGenerateColumns="False" AllowPaging="True" HorizontalAlign="Center" OnPageIndexChanging="gvQuery_PageIndexChanging" GridLines="None" Width="95%" CellPadding="4" ForeColor="#333333">
                        <Columns>
                            <asp:TemplateField HeaderText="交易时间" SortExpression="start_time">
                                <ItemStyle Font-Size="12px" Width="120px" HorizontalAlign="Left" />
                                <ItemTemplate>
                                    <span><%#Eval("start_time") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="商家编号" SortExpression="merid">
                                <ItemStyle Font-Size="12px" Width="60px" HorizontalAlign="Left" />
                                <ItemTemplate>
                                    <span><%#Eval("merid") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="商品编号" SortExpression="goodsid">
                                <ItemStyle Font-Size="12px" Width="60px" HorizontalAlign="Left" />
                                <ItemTemplate>
                                    <span><%#Eval("goodsid") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="手机号" SortExpression="mobileid">
                                <ItemStyle Font-Size="12px" Width="80" />
                                <ItemTemplate>
                                    <span><%#Eval("mobileid") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="订单号" SortExpression="orderid">
                                <ItemStyle Font-Size="12px" Width="150px" HorizontalAlign="Left" />
                                <ItemTemplate>
                                    <span><%#Eval("orderid") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="金额(分)" SortExpression="amount">
                                <ItemStyle Font-Size="12px" Width="80px" HorizontalAlign="Left" />
                                <ItemTemplate>
                                    <span><%#Eval("amount") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="交易状态" SortExpression="retmsg">
                                <ItemStyle Font-Size="12px" Width="100px" HorizontalAlign="Left" />
                                <ItemTemplate>
                                    <span><%#Eval("retmsg") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="确认状态" SortExpression="c_retcode">
                                <ItemStyle Font-Size="12px" Width="100px" HorizontalAlign="Left" />
                                <ItemTemplate>
                                    <span><%#Eval("c_retcode") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EditRowStyle BackColor="#999999" />
                        <EmptyDataTemplate>
                            <br />
                            <div align="center">
                                没有相关的记录。
                            </div>
                        </EmptyDataTemplate>
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle HorizontalAlign="Center" BackColor="#F7F6F3" ForeColor="#333333" />
                        <AlternatingRowStyle HorizontalAlign="Center" BackColor="White" ForeColor="#284775" />
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle HorizontalAlign="Center" BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#E9E7E2" />
                        <SortedAscendingHeaderStyle BackColor="#506C8C" />
                        <SortedDescendingCellStyle BackColor="#FFFDF8" />
                        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                    </asp:GridView>
                </div>
            </fieldset>
        </div>
        <div>
        </div>
    </form>
</body>
</html>
