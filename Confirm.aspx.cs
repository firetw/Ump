using ICSharpCode.SharpZipLib.GZip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Umpay.Hjdl;

public partial class Confirm : System.Web.UI.Page
{
    protected static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString.Count < 1) return;

        try
        {
            ConfirmTran();
        }
        catch (Exception ex)
        {
            _log.Error(ex);
        }

    }
    /// <summary>
    /// 确认支付
    /// </summary>
    private void ConfirmTran()
    {
        Response.Clear();

        PayResultRequest req = new PayResultRequest();
        req.startTime = DateTime.Now;
        req.amount = Request.QueryString["amount"];
        req.amtType = Request.QueryString["amtType"];
        req.bankType = Convert.ToInt32(Request.QueryString["bankType"]);
        req.goodsId = Request.QueryString["goodsId"];
        req.merDate = Request.QueryString["merDate"];
        req.merId = Request.QueryString["merId"];
        req.merPriv = Request.QueryString["merPriv"];
        req.mobileId = Request.QueryString["mobileId"];
        req.orderId = Request.QueryString["orderId"];
        req.payDate = Request.QueryString["payDate"];
        req.retCode = Request.QueryString["retCode"];
        req.settleDate = Request.QueryString["settleDate"];
        req.sign = Request.QueryString["sign"];
        req.transType = Request.QueryString["transType"];
        req.version = Request.QueryString["version"];

        if (!SqlInject.ProcessSqlStr(0, req.TranFormat()))
        {
            Response.Write("数格式不正确");
            return;
        }
        if (!SqlInject.ProcessSqlStr(0, req.sign))
        {
            Response.Write("数格式不正确");
            return;
        }

        ///TODO:
        PayResutlResponse res = new PayResutlResponse();
        res.goodsId = req.goodsId;
        res.merDate = req.merDate;
        res.merId = req.merId;
        res.orderId = req.orderId;

        IDictionary param = new Hashtable();
        param.Add("OrderId", req.orderId);
        param.Add("MerDate", req.merDate);

        //签名验证:
        String unsign = req.TranFormat(); //"merId=" + req.merId + "&goodsId=" + req.goodsId + "&orderId=" + req.orderId + "&merDate=" + req.merDate + "&payDate=" + req.payDate + "&amount=" + req.amount + "&amtType=" + req.amtType + "&bankType=" + req.bankType + "&mobileId=" + req.mobileId + "&transType=" + req.transType + "&settleDate=" + req.settleDate + "&merPriv=" + req.merPriv + "&retCode=" + req.retCode + "&version=" + req.version;
        /*******************************************************
           * 3.验签
         *******************************************************/
        bool verifyResult = SignUtil.verify(unsign, req.sign);
        if (verifyResult)
        {
            _log.Info(string.Format("签名内容:{0}\r\n\t签名:{1}\r\n\t验证成功", unsign, req.sign));
            if (req.retCode != "0000")
            {
                //ORDERID=#OrderId# AND MERDATE=#MerDate#
                _log.Info(string.Format("返回码不成功,查看支付是否成功. 签名内容:{0}\r\n\t签名:{1}\r\n\t验证成功", unsign, req.sign));
                try
                {
                    DataTable dt = DbHelper.Instance.DataMapper.QueryForDataTable("GetRequset", param);

                    if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["RETCODE"] != DBNull.Value && dt.Rows[0]["RETCODE"].ToString() == "0000")
                    {
                        _log.Info(string.Format("返回码不成功,支付成功,将完成交易. 签名内容:{0}\r\n\t签名:{1}", unsign, req.sign));
                        ///查看之前的状态是否为成功
                        res.retCode = "0000";
                        res.retMsg = "";
                        res.sign = "";
                    }
                    else
                    {
                        _log.Info(string.Format("返回码不成功,支付不成功,将取消交易. 签名内容:{0}\r\n\t签名:{1}", unsign, req.sign));
                        res.retCode = "1111";
                        res.retMsg = "";
                        res.sign = "";
                    }
                }
                catch (Exception ex)
                {
                    res.retCode = "1111";
                    _log.Error(ex);
                }
            }
            else
            {
                _log.Info(string.Format("签名内容:{0}\r\n\t签名:{1}\r\n\t返回码成功,将完成交易", unsign, req.sign));
                res.retCode = "0000";
                res.retMsg = "";
                res.sign = "";
            }
        }
        else
        {
            _log.Info(string.Format("签名内容:{0}\r\n\t签名:{1}\r\n\t验证失败,将取消交易", unsign, req.sign));
            res.retCode = "1111";
            res.retMsg = "签名验证失败";
            res.sign = "";
        }
        req.vendorRetCode = res.retCode;
        req.vendorRetMsg = res.retMsg;
        req.vendorRetsign = res.sign;

        req.endTime = DateTime.Now;
        UmpayHelper.Confirm(req);

        string signStr = res.TranFormat();
        signStr = signStr + "|" + SignUtil.sign(signStr);
        _log.Info(string.Format("返回的内容:{0}", signStr));
        Response.Write(string.Format("<META NAME=\"MobilePayPlatform\" CONTENT=\"{0}\">", signStr));
    }

    private string Read()
    {
        string ContentEncoding = Request.Headers["Content-Encoding"];//: gzip
        StringBuilder text = new StringBuilder();
        Encoding enc = Encoding.UTF8;
        string content = string.Empty;
        switch (ContentEncoding)
        {
            case "gzip":
                using (Stream reader = new GZipInputStream(Request.InputStream))
                {
                    MemoryStream ms = new MemoryStream();
                    int nSize = 4096;
                    byte[] writeData = new byte[nSize];
                    while (true)
                    {
                        try
                        {
                            nSize = reader.Read(writeData, 0, nSize);
                            if (nSize > 0)
                                ms.Write(writeData, 0, nSize);
                            else
                                break;
                        }
                        catch (GZipException)
                        {
                            break;
                        }
                    }
                    reader.Close();
                    text.Append(enc.GetString(ms.GetBuffer()));
                    content = text.ToString();
                }
                break;
            default:
                using (Stream stream = Request.InputStream)
                {

                    byte[] buffer = new byte[1024];
                    int n = 0;
                    while ((n = stream.Read(buffer, 0, 1024)) > 0)
                    {
                        text.Append(enc.GetString(buffer, 0, n));
                    }
                    content = text.ToString();
                }
                break;
        }
        return content;
    }
}
