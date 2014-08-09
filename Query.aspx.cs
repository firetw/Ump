using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Umpay.Hjdl;

public partial class Query : System.Web.UI.Page
{
    protected static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (string.IsNullOrEmpty(this.tbStartTime.Text) && string.IsNullOrEmpty(this.tbEndTime.Text))
            {
                this.tbStartTime.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                this.tbEndTime.Text = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            }
        }


    }
    protected void btExport_Click(object sender, EventArgs e)
    {
        Hashtable hs = new Hashtable();

        #region Filters
        if (!String.IsNullOrEmpty(this.tbStartTime.Text))
        {
            hs.Add("StartTime", this.tbStartTime.Text);
        }
        if (!String.IsNullOrEmpty(this.tbEndTime.Text))
        {
            hs.Add("EndTime", this.tbEndTime.Text);
        }
        if (!String.IsNullOrEmpty(this.tbGoodId.Text))
        {
            hs.Add("GoodsId", this.tbGoodId.Text);
        }
        if (!String.IsNullOrEmpty(this.tbPhone.Text))
        {
            hs.Add("Mobileid", this.tbPhone.Text);
        }
        if (!SqlInject.ProcessSqlStr(0, this.tbStartTime.Text, this.tbEndTime.Text, this.tbGoodId.Text, this.tbPhone.Text))
        {
            Response.Write("数格式不正确");
            return;
        }
        #endregion

        try
        {
            DataTable dt = DbHelper.Instance.DataMapper.QueryForDataTable("QueryReportResult", hs);

            ExportItem item = new ExportItem();
            item.ConfigFile = "QueryReport.xml";
            item.Data = dt;

            NPOIHelper helper = new NPOIHelper();
            string fileName = helper.Export(item);

            string filePath = "Download.aspx?file=" + fileName;//AppDomain.CurrentDomain.BaseDirectory + "/export/" + fileName;
            string js = @"<script language='javascript' type='text/javascript'>window.open('" + filePath + "', '文件下载', 'toolbar=no,location=no,directories=no,status=no,scrollbars=yes,menubar=no,resizable=yes,top=100,left=200,width=650,height=300');</script>";
            string url = "http://203.171.225.8/Download.aspx?file=" + fileName;
            string script = string.Format("window.open({0} ,\"_blank\",\"fullscreen=no,status=no,resizable=yes,toolbar=no,menubar=yes,location=no,directories=no;\"); ", filePath);
            if (!Page.ClientScript.IsClientScriptBlockRegistered("downreport"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "downreport", js, false);
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex);
        }
    }

    protected void btSearch_Click(object sender, EventArgs e)
    {
        BindGV();
    }

    protected void gvQuery_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gvQuery.PageIndex = e.NewPageIndex;
        BindGV();
    }

    public void BindGV()
    {
        Hashtable hs = new Hashtable();

        #region Filters
        if (!String.IsNullOrEmpty(this.tbStartTime.Text))
        {
            hs.Add("StartTime", this.tbStartTime.Text);
        }
        if (!String.IsNullOrEmpty(this.tbEndTime.Text))
        {
            hs.Add("EndTime", this.tbEndTime.Text);
        }
        if (!String.IsNullOrEmpty(this.tbGoodId.Text))
        {
            hs.Add("GoodsId", this.tbGoodId.Text);
        }
        if (!String.IsNullOrEmpty(this.tbPhone.Text))
        {
            hs.Add("Mobileid", this.tbPhone.Text);
        }
        if (!SqlInject.ProcessSqlStr(0, this.tbStartTime.Text, this.tbEndTime.Text, this.tbGoodId.Text, this.tbPhone.Text))
        {
            Response.Write("数格式不正确");
            return;
        }
        #endregion

        try
        {
            DataTable dt = DbHelper.Instance.DataMapper.QueryForDataTable("QueryReportResult", hs);

            DataView dv = new DataView(dt);

            this.gvQuery.PageSize = Config.PageSize;
            gvQuery.DataSource = dv;
            gvQuery.DataBind();
        }
        catch (Exception ex)
        {
            _log.Error(ex);
        }
    }


}