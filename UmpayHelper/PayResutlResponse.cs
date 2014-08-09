﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Umpay.Hjdl
{
    /// <summary>
    /// PayResutlResponse 的摘要说明
    /// </summary>
    public class PayResutlResponse
    {
        public PayResutlResponse()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
            merId = Config.MemId;
            version = Config.Version;
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
        /// merDate	商户日期	M	√	格式：yyyyMMdd
        /// </summary>
        public string merDate { get; set; }
        /// <summary>
        /// retCode	返回码	M	√	0000：交易成功
        ///1111：交易取消
        /// </summary>
        public string retCode { get; set; }
        /// <summary>
        /// retMsg	返回描述	M	√	商品货物信息，比如卡号，密码等，会下发给用户（即发货）
        /// </summary>
        public string retMsg { get; set; }
        /// <summary>
        /// version	版本号	M	√	定值：3.0
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// sign	签名	M	
        /// </summary>
        public string sign { get; set; }




        /// <summary>
        /// 格式化
        /// </summary>
        /// <returns></returns>
        public string TranFormat()
        {
            return string.Format("merId={0}|goodsId={1}|orderId={2}|merDate={3}|retCode={4}|retMsg={5}|version={6}",
                merId,
                goodsId,
                orderId,
                merDate,
                retCode,
                retMsg,
                version);
        }


        //merId	商户号	M	√	由平台分配
        //goodsId	商品号	M	√	由平台分配
        //orderId	订单号	M	√	由商户生成
        //merDate	商户日期	M	√	格式：yyyyMMdd
        //retCode	返回码	M	√	0000：交易成功
        //1111：交易冲正
        //retMsg	返回描述	M	√	商品货物信息，比如卡号，密码等，会下发给用户（即发货）
        //version	版本号	M	√	定值：3.0
        //sign	签名	M		

    }
}