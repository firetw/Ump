using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Download : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string exportFileName = Request["file"];


        if (!string.IsNullOrEmpty(exportFileName))
        {
            System.IO.FileInfo fileInfo;// = new System.IO.FileInfo(pyhsifile);
            string filePath = string.Empty;
            string pyhsifile = string.Empty;
            string path = AppDomain.CurrentDomain.BaseDirectory + "/export";
            if (!string.IsNullOrEmpty(path))
            {
                filePath = path;
                pyhsifile = Path.Combine(filePath, exportFileName);
                fileInfo = new System.IO.FileInfo(pyhsifile);
            }
            else
            {
                fileInfo = new System.IO.FileInfo(exportFileName);
            }

            if (fileInfo.Exists)
            {
                Response.Clear();
                Response.ClearHeaders();
                Response.Buffer = false;
                Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileInfo.Name));
                Response.AddHeader("content-length", fileInfo.Length.ToString());
                if (fileInfo.Extension == ".xls" || fileInfo.Extension == ".xlsx")
                {
                    Response.ContentType = "application/msexcel";
                }
                else if (fileInfo.Extension == ".txt")
                {
                    Response.ContentType = "text/plain";
                }
                else
                {
                    Response.ContentType = "application/octet-stream";
                }
                Response.ContentEncoding = System.Text.Encoding.Default;
                Response.WriteFile(fileInfo.FullName);
                Response.Flush();
                Response.End();
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "showpop", "window.close();", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "showpop", "alert('找不到所需的文件');window.close();", true);
            }
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "showpop", "alert('找不到所需的文件');window.close();", true);
        }


    }

}