using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Umpay.Hjdl;

public partial class Direct : System.Web.UI.Page
{

    protected static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    protected void Page_Load(object sender, EventArgs e)
    {
        //需要订单号 商品号 手机号 金额(单位分)
        //orderid   goodsid mobileid    account
        Response.Clear();
        string orderidStr = HttpUtility.UrlDecode(Request["orderId"]);
        string goodsidStr = HttpUtility.UrlDecode(Request["goodsId"]);
        string mobileidStr = Request["mobileId"];
        string amountStr = Request["amount"];//单位分

        #region 数值验证
        if (string.IsNullOrEmpty(orderidStr))
        {
            Response.Write("订单号为空");
            return;
        }
        if (string.IsNullOrEmpty(goodsidStr))
        {
            Response.Write("商品编号为空");
            return;
        }
        if (string.IsNullOrEmpty(mobileidStr))
        {
            Response.Write("手机号为空");
            return;
        }
        if (string.IsNullOrEmpty(amountStr))
        {
            Response.Write("交易金额为空");
            return;
        }
        int amountValue = 0;
        if (Int32.TryParse(amountStr, out amountValue))
        {
            if (amountValue < 0)
            {
                Response.Write("金额数值不正确");
                return;
            }
        }
        else
        {
            Response.Write("金额数格式不正确");
            return;
        }
        if (!SqlInject.ProcessSqlStr(0, orderidStr, goodsidStr, mobileidStr, amountStr))
        {
            Response.Write("数格式不正确");
            return;
        }
        #endregion
        string format = string.Format("orderId={0}&goodsId={1}&mobileId={2}&amount={3}", orderidStr, goodsidStr, mobileidStr, amountStr);
        try
        {
            DateTime current = DateTime.Now;
            PayRequest request = new PayRequest
            {
                amount = Convert.ToDouble(amountStr),
                amtType = Config.AmtType,
                bankType = Config.BankType,
                expand = "",
                goodsId = goodsidStr,
                merDate = current.ToString("yyyyMMdd"),
                merId = Config.MemId,
                merPriv = "",
                mobileId = mobileidStr,
                notifyUrl = Config.NotifyUrl,
                orderId = orderidStr,
                startTime = current,
                version = Config.Version
            };
            string error = UmpayHelper.Pay(request);
            if (string.IsNullOrEmpty(error))
            {
                Response.Write("支付成功");
            }
            else
            {
                Response.Write(error);
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex);
        }
    }
}