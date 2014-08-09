using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Umpay.Hjdl;

public partial class QueryOrder : System.Web.UI.Page
{

    protected static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Clear();
        string orderId = Request["orderId"];
        if (string.IsNullOrEmpty(orderId))
        {
            Response.Write("订单号为空");
            return;
        }

        if (!SqlInject.ProcessSqlStr(0, orderId))
        {
            Response.Write("数格式不正确");
            return;
        }

        try
        {
            IList<string> list = DbHelper.Instance.DataMapper.QueryForList<string>("QueryRequestByOrderId", orderId);
            if (list == null || list.Count < 1)
            {
                Response.Write("交易失败");
                return;
            }
            Response.Write(list[0]);
        }
        catch (Exception ex)
        {
            _log.Error(ex);
        }


    }
}