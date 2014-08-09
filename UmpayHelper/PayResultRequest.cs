using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Umpay.Hjdl
{
    /// <summary>
    /// PayResultRequest 的摘要说明
    /// 平台发起的请求0
    /// </summary>
    public class PayResultRequest
    {
        public PayResultRequest()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
            //bankType = 3;
            //transType = "1";
        }

        /// <summary>
        /// merId	商户号	M	√	由平台分配
        /// </summary>
        public string merId { get; set; }

        /// <summary>
        /// goodsId	商品号	M	√	由平台分配
        /// </summary>
        public string goodsId { get; set; }

        /// <summary>
        /// orderId	订单号	M	√	由商户生成
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        ///merDate	商户日期	M	√	格式：yyyyMMdd
        /// </summary>
        public string merDate { get; set; }

        /// <summary>
        /// payDate	支付日期	M	√	平台支付日期，格式：yyyyMMdd
        /// </summary>
        public string payDate { get; set; }

        /// <summary>
        /// amount	商品金额	M	√	单位：分
        /// </summary>
        public string amount { get; set; }

        /// <summary>
        /// amtType	货币类型	M	√	定值：02
        /// </summary>
        public string amtType { get; set; }

        /// <summary>
        /// bankType	银行类型	M	√	定值：3
        /// </summary>
        public int bankType { get; set; }
        /// <summary>
        /// mobileId	手机号	M	√	
        /// </summary>
        public string mobileId { get; set; }

        /// <summary>
        /// transType	交易类型	M	√	0：新增
        ///1：续费（针对包月商品）
        /// </summary>
        public string transType { get; set; }

        /// <summary>
        /// 清算日期	M	√	账务清算日期，格式：yyyyMMdd
        /// </summary>
        public string settleDate { get; set; }


        /// <summary>
        /// merPriv	商户私有信息	O	√	
        /// </summary>
        public string merPriv { get; set; }

        /// <summary>
        ///retCode	返回码	M	√	0000：支付成功
        /// </summary>
        public string retCode { get; set; }

        public string version { get; set; }

        public string sign { get; set; }


        #region 商家返回的内容
        /// <summary>
        /// retCode	返回码	M	√	0000：交易成功
        ///1111：交易取消
        /// </summary>
        public string vendorRetCode { get; set; }
        /// <summary>
        /// retMsg	返回描述	M	√	商品货物信息，比如卡号，密码等，会下发给用户（即发货）
        /// </summary>
        public string vendorRetMsg { get; set; }


        /// <summary>
        /// sign	签名	M	
        /// </summary>
        public string vendorRetsign { get; set; }

        #endregion

        #region 时间
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        #endregion

        /// <summary>
        /// 格式化
        /// </summary>
        /// <returns></returns>
        public string TranFormat()
        {
            return string.Format("merId={0}&goodsId={1}&orderId={2}&merDate={3}&payDate={4}&amount={5}&amtType={6}&bankType={7}&mobileId={8}&transType={9}&settleDate={10}&merPriv={11}&retCode={12}&version={13}",
                merId,
                goodsId,
                orderId,
                merDate,
                payDate,
                amount,
                amtType,
                bankType,
                mobileId,
                transType,
                settleDate,
                merPriv,
                retCode,
                version);
        }
    }
}