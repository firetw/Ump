using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Umpay.Hjdl
{
    /// <summary>
    /// 支付请求
    /// 商户发送的请求
    /// </summary>
    public class PayRequest
    {
        public PayRequest()
        {
            merId = Config.MemId;
            bankType = Config.BankType;
            amtType = Config.AmtType;
            version = Config.Version;
        }

        //http://www.testurl.com? merId=9996&goodsId=100&mobileId=13426399070&orderId=576216&merDate=20090402&amount=1000&amtType=02&bankType=3&notifyUrl=http://211.136.93.20:8081/demo1/paymentNotify3.jsp&merPriv=&expand=&version=3.0&sign=TTAY4Tivattn2ELyCNR8tmFi1dC5ViZfHdRC7awqYAwXFR8vaGA4ESYahG24oE6VYdKdnPHYhP1qiW

        /// <summary>
        /// merId	商户号	M	√	由平台分配
        /// </summary>
        public string merId { get; set; }

        /// <summary>
        /// goodsId	商品号	M	√	由平台分配
        /// </summary>
        public string goodsId { get; set; }

        /// <summary>
        /// mobileId	手机号	M	√	
        /// </summary>
        public string mobileId { get; set; }

        /// <summary>
        /// orderId	订单号	M	√	由商户生成
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// merDate	商户日期	M	√	格式：yyyyMMdd
        /// </summary>
        public string merDate { get; set; }

        /// <summary>
        /// amount	商品金额	M	√	单位：分
        /// </summary>
        public double amount { get; set; }

        /// <summary>
        /// amtType	货币类型	M	√	定值：02
        /// </summary>
        public string amtType { get; set; }

        /// <summary>
        /// bankType	银行类型	M	√	定值：3
        /// </summary>
        public int bankType { get; set; }

        /// <summary>
        /// notifyUrl	后台通知地址	M	√	
        /// </summary>
        public string notifyUrl { get; set; }

        public string merPriv { get; set; }

        public string expand { get; set; }

        public string version { get; set; }

        public string sign { get; set; }


        #region 返回的内容
        /// <summary>
        /// retCode	返回码	M	√	0000   T
        /// </summary>
        public string retCode { get; set; }
        /// <summary>
        /// retMsg	返回描述	M	√	
        /// </summary>
        public string retMsg { get; set; }
        /// <summary>
        /// sign	签名	M	
        /// </summary>
        public string retSign { get; set; }
        #endregion

        #region 时间
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        #endregion

        public string TranFormat()
        {
            //http://211.136.93.20:8081/demo1/paymentNotify3.jsp
            //TTAY4Tivattn2ELyCNR8tmFi1dC5ViZfHdRC7awqYAwXFR8vaGA4ESYahG24oE6VYdKdnPHYhP1qiW
            //string result = string.Format("merId={0}&goodsId={1}&mobileId={2}&orderId={3}&merDate={4}&amount={5}&amtType={6}&bankType={7}&notifyUrl={8}&merPriv={9}&expand={10}&version={11}",
            //    merId,
            //    goodsId,
            //    mobileId,
            //    orderId,
            //    merDate,
            //    amount,
            //    amtType,
            //    bankType,
            //    notifyUrl,
            //    merPriv,
            //    expand,
            //    version);
            //return result;

            string result = string.Format("merId={0}&goodsId={1}&mobileId={2}&orderId={3}&merDate={4}&amount={5}&amtType={6}&bankType={7}&notifyUrl={8}&merPriv={9}&expand={10}&version={11}",
              System.Web.HttpUtility.UrlEncode(merId),
              System.Web.HttpUtility.UrlEncode(goodsId),
              System.Web.HttpUtility.UrlEncode(mobileId),
              System.Web.HttpUtility.UrlEncode(orderId),
              System.Web.HttpUtility.UrlEncode(merDate),
              amount,
              System.Web.HttpUtility.UrlEncode(amtType),
              bankType,
              System.Web.HttpUtility.UrlEncode(notifyUrl),
              System.Web.HttpUtility.UrlEncode(merPriv),
              System.Web.HttpUtility.UrlEncode(expand),
              System.Web.HttpUtility.UrlEncode(version));
            return result;
        }

        public string BeforeEncode()
        {
            string result = string.Format("merId={0}&goodsId={1}&mobileId={2}&orderId={3}&merDate={4}&amount={5}&amtType={6}&bankType={7}&notifyUrl={8}&merPriv={9}&expand={10}&version={11}",
                       merId,
                       goodsId,
                       mobileId,
                       orderId,
                       merDate,
                       amount,
                       amtType,
                       bankType,
                       notifyUrl,
                       merPriv,
                       expand,
                       version);
            return result;
        }
    }
}