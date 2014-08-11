using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using WebLoginer.Core;

namespace Umpay.Hjdl
{
    public class UmpayHelper
    {
        static Encoding encoding = Encoding.GetEncoding("gbk"); //Encoding.UTF8;
        protected static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 支付流程
        /// </summary>
        public static string Pay(PayRequest request)
        {
            string error = string.Empty;
            if (request == null)
            {
                error = "非法的参数!";
                _log.Error(error);
                return error;
            }

            IDictionary param = new Hashtable();
            param.Add("OrderId", request.orderId);
            param.Add("MerDate", request.merDate);

            int count = DbHelper.Instance.DataMapper.QueryForObject<int>("GetRequsetCount", param);
            if (count > 0)
            {
                return "重复的订单号";
            }
            CookieContainer container = new CookieContainer();
            CookieCollection cookies = new CookieCollection();

            WebOperResult receive = null;
            try
            {
                string signContent = request.BeforeEncode();
                _log.Info("签名内容:" + signContent);
                request.sign = SignUtil.sign(signContent);
                string postData = request.TranFormat();
                postData += "&sign=" + System.Web.HttpUtility.UrlEncode(request.sign);
                _log.Info("提交请求:" + postData);
                string url = Config.UmPayUrl + "?" + postData;
                _log.Info("Url:" + url);
                receive = Get(url, container, cookies); //Post(Config.UmPayUrl, container, cookies, postData);
                string content = receive.Text;//"<META NAME=\"MobilePayPlatform\" CONTENT =\"9996|100|576216|20090402|0000|下订单成功|3.0|wlItyXiYPPm/2QcSzf8wrl/XxzkF8m9aN14qlBBvhB30pFJE7zR4cMRO2Ods\">"; //receive.Text;
                _log.Info("收到的内容:" + content);
                PayResponse response = ParserPayResponse(content);

                request.retCode = response.retCode;
                request.retMsg = response.retMsg;
                request.retSign = response.sign;
                request.endTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                error = "提交请求失败:" + ex;
                _log.Error(error);
                return error;
            }
            try //数据入库
            {
                DbHelper.Instance.DataMapper.Insert("InsertPayRequest", request);
                _log.Info("支付请求入库成功");
            }
            catch (Exception ex)
            {
                error = "支付请求入库失败:" + ex;
                _log.Error(error);
                return error;
            }
            if (request.retCode == "0000")
            {
                return "";
            }
            else
            {
                if (request.retCode != null)
                {
                    return request.retMsg;
                }
                return "支付出现未知异常";
            }
        }
        private static PayResponse ParserPayResponse(string text)
        {
            PayResponse result = new PayResponse();
            if (string.IsNullOrEmpty(text)) throw new Exception("支付返回报文格式异常");
            Match match = Regex.Match(text, "<META\\s+NAME\\s*=\"MobilePayPlatform\"\\s+CONTENT\\s*=\\s*\"([^\"]+)\"");
            if (match != null && match.Success)
            {
                string[] tmpArr = match.Groups[1].Value.Split('|');
                if (tmpArr == null || tmpArr.Length < 8)
                {
                    throw new Exception("支付返回报文格式异常");
                }
                result.merId = tmpArr[0];
                result.goodsId = tmpArr[1];
                result.orderId = tmpArr[2];
                result.merDate = tmpArr[3];
                result.retCode = tmpArr[4];
                result.retMsg = tmpArr[5];
                result.version = tmpArr[6];
                result.sign = tmpArr[7];
            }
            return result;
        }
        public static void Confirm(PayResultRequest request)
        {
            if (request == null)
            {
                _log.Error("非法的参数");
                return;
            }
            try
            {
                DbHelper.Instance.DataMapper.Insert("InsertPayResultRequest", request);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        private static WebOperResult Get(string url, CookieContainer container, CookieCollection cookies)
        {
            RequestContext reContext = RequestContext.DefaultContext();
            reContext.Encoding = encoding;
            reContext.ContentType = "text/html";
            reContext.URL = url;
            reContext.CookieContainer = container;
            reContext.CookieContainer.Add(cookies);

            WebOperResult reResult = HttpWebHelper.Get(reContext);
            return reResult;
        }

        private static WebOperResult Post(string url, CookieContainer container, CookieCollection cookies, string postData, bool autoRedirect = true)
        {
            RequestContext postContext = RequestContext.DefaultContext();
            postContext.Encoding = encoding;
            postContext.URL = url;
            postContext.Allowautoredirect = autoRedirect;
            postContext.Method = "POST";
            postContext.Accept = "application/xhtml+xml, */*";
            postContext.ContentType = "application/x-www-form-urlencoded";
            postContext.Postdata = postData;
            postContext.CookieContainer = container;
            postContext.CookieContainer.Add(cookies);

            WebOperResult postResult = HttpWebHelper.Post(postContext);


            return postResult;
        }
    }
}
