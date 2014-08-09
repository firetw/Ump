using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Umpay.Hjdl;

public partial class _Default : System.Web.UI.Page
{
    protected static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (string.IsNullOrEmpty(this.tbOrderId.Text))
            {
                this.tbMerDate.Text = DateTime.Now.ToString("yyyyMMdd");
                this.tbMerId.Text = Config.MemId;
                this.tbNotifyUrl.Text = Config.NotifyUrl;
                this.tbVersion.Text = Config.Version;
                this.tbOrderId.Text = "YZ" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
            }
        }

    }
    protected void btTest_Click(object sender, EventArgs e)
    {
        try
        {



            PayRequest request = new PayRequest
            {
                amount = Convert.ToDouble(this.tbAmount.Text),
                amtType = this.tbAmtType.Text,
                bankType = Convert.ToInt32(this.tbBankType.Text),
                expand = this.tbExpand.Text,
                goodsId = this.tbGoodsId.Text,
                merDate = this.tbMerDate.Text,
                merId = this.tbMerId.Text,
                merPriv = this.tbMerPriv.Text,
                mobileId = this.tbMobileId.Text,
                notifyUrl = this.tbNotifyUrl.Text,
                orderId = this.tbOrderId.Text,
                startTime = DateTime.Now,
                version = this.tbVersion.Text
            };

            if (!SqlInject.ProcessSqlStr(0, request.amtType, request.expand, request.goodsId, request.merDate, request.merId,
                request.notifyUrl, request.orderId, request.version))
            {
                Response.Write("数格式不正确");
                return;
            }

            string error = UmpayHelper.Pay(request);
            if (string.IsNullOrEmpty(error))
            {
                this.lbMsg.Text = "支付成功";
            }
            else
            {
                this.lbMsg.Text = error;
            }
        }
        catch (Exception ex)
        {
            this.lbMsg.Text = ex.ToString();
            _log.Error(ex);
        }
    }
}