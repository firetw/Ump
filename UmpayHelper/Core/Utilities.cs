using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
 

namespace WebLoginer
{
    class Utilities
    {
        #region 编解码函数
        #region UTF8编码
        public static string UTF8(string Source)
        {
            #region 另一种方法
            //string tem="";
            //byte[] utf8=Encoding.UTF8.GetBytes(Source);
            //foreach (byte b in utf8)
            //{
            //    tem += "%" + string.Format("{0:X2}", b);
            //}
            //return tem;
            #endregion

            return HttpUtility.UrlEncode(Source);
        }
        #endregion

        #region %uxxxx编码
        public static string EncodeStr(string Source)
        {
            //MessageBox.Show(EncodeStr("%u6D4B%u8BD5"));
            return HttpUtility.UrlEncodeUnicode(Source);
        }
        #endregion

        #region &#xx|%uxx|%xx%yy格式解码
        public static string DecodeStr(string Source)
        {
            //MessageBox.Show(DecodeStr("&#-28711;&#63"));
            //MessageBox.Show(DecodeStr("%u6D4B%u8BD5"));
            string tem = Source;
            if (Source.Contains("&#"))
            {
                tem = "";
                if (Source.EndsWith(";")) Source = Source.Substring(0, Source.Length - 1);
                string[] tmp = Source.Replace("&#", "").Split(';');
                for (int i = 0; i < tmp.Length; i++)
                {
                    string t = int.Parse(tmp[i]).ToString("x4");
                    tem += "%u" + t.Substring(t.Length - 4);
                }
            }
            return HttpUtility.UrlDecode(tem);
        }
        #endregion

        #region MD5加密
        public static string MD5(string Source)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(Source, "MD5");
        }
        #endregion

        #endregion

        #region 从Byte数组中得到图片
        //public static Image GetImageFromByte(byte[] PicBytes)
        //{
        //    MemoryStream ms = new MemoryStream(PicBytes);
        //    ms.Position = 0;
        //    Image img = Image.FromStream(ms);
        //    ms.Close();
        //    return img;
        //}
        #endregion

        #region 字符串操作函数
        #region 处理Cookie
        public static string ControlCookies(string Cookies)
        {
            try
            {
                string rStr = "";
                Regex r = new Regex("(?<=,)(?<cookie>[^ ]+=(?!deleted;)[^;]+);");
                Match m = r.Match("," + Cookies);
                while (m.Success)
                {
                    if (!rStr.Contains(m.Groups["cookie"].Value)) rStr += m.Groups["cookie"].Value + "; ";
                    m = m.NextMatch();
                }
                return rStr;
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region 过滤html标签
        public static string StripHTML(string stringToStrip)
        {
            stringToStrip = Regex.Replace(stringToStrip, "</p(?:\\s*)>(?:\\s*)<p(?:\\s*)>", "\n\n", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            stringToStrip = Regex.Replace(stringToStrip, "", "\n", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            stringToStrip = Regex.Replace(stringToStrip, "\"", "''", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            stringToStrip = StripHtmlXmlTags(stringToStrip);
            return stringToStrip;
        }
        private static string StripHtmlXmlTags(string content)
        {
            return Regex.Replace(content, "<[^>]+>", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
        #endregion

        #region 取中间字符串
        public static string GetMidStr(string Source, string StartStr, string EndStr)
        {
            try
            {
                int StartPos = Source.IndexOf(StartStr, 0) + StartStr.Length;
                int EndPos = Source.IndexOf(EndStr, StartPos);
                return Source.Substring(StartPos, EndPos - StartPos);
            }
            catch { return ""; }
        }
        #endregion

        #region 字符串倒序输出
        public static string ReverseStr(string text)
        {
            return new string(text.ToCharArray().Reverse().ToArray());
        }
        #endregion

        #endregion

        #region 时间操作函数
        #region 时间戳
        public static string GetTime()
        {
            DateTime timeStamp = new DateTime(1970, 1, 1);
            long a = (DateTime.UtcNow.Ticks - timeStamp.Ticks) / 10000;
            return a.ToString();
        }
        #endregion

        #region 获取网络时间
        public static string GetNetTime(string NetTime)
        {
            NetTime = NetTime.Replace(" GMT", "");
            DateTime dt = Convert.ToDateTime(NetTime);
            dt = TimeZoneInfo.ConvertTimeFromUtc(dt, TimeZoneInfo.Local);
            return dt.ToString();
        }
        #endregion

        #region 时间相加
        public static string OperationTime(string BaseTime, string AddTime, string AddType)
        {
            DateTime dt = Convert.ToDateTime(BaseTime);
            DateTime rt = DateTime.Now;
            switch (AddType)
            {
                case "年": rt = dt.AddYears(int.Parse(AddTime)); break;
                case "月": rt = dt.AddMonths(int.Parse(AddTime)); break;
                case "日": rt = dt.AddDays(double.Parse(AddTime)); break;
                case "时": rt = dt.AddHours(double.Parse(AddTime)); break;
                case "分": rt = dt.AddMinutes(double.Parse(AddTime)); break;
                case "秒": rt = dt.AddSeconds(double.Parse(AddTime)); break;
                case "毫秒": rt = dt.AddMilliseconds(double.Parse(AddTime)); break;
            }
            return rt.ToString();
        }
        #endregion

        #region 英文月份变成数字月份
        public static string strToNum(string source)
        {
            string ret = "";
            string[] tem = new string[12];
            string[] temNum = new string[12];
            tem = "Jan Feb Mar Apr May Jun Jul Aug Sep Oct Nov Dec".Split(' ');
            temNum = "01 02 03 04 05 06 07 08 09 10 11 12 ".Split(' ');
            for (int i = 0; i < 12; i++)
            {
                if (source == tem[i])
                {
                    ret = temNum[i];
                    break;
                }
            }
            return ret;
        }
        #endregion

        #endregion

        #region 上传类函数
        #region 图片转Byte数组
        public static byte[] FileToBytes(string FilePath)
        {
            bool isPic = false;
            string[] fileType = new string[] { "tiff", "psd", "eps", "raw", "pdf", "png", "pxr", "mac", "jpg", "bmp", "tga", "vst", "pcd", "pct", "gif", "ai", "fpx", "img", "cal", "wi", "png", "eps", "ai", "sct", "pdf", "pdp", "dxf" };
            for (int i = 0; i < fileType.Length; i++)
            {
                if (FilePath.EndsWith("." + fileType[i]))
                {
                    isPic = true;
                    break;
                }
            }
            if (isPic)
            {
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    using (Image Img = Image.FromFile(FilePath))
                //    {
                //        using (Bitmap Bmp = new Bitmap(Img))
                //        {
                //            Bmp.Save(ms, Img.RawFormat);
                //        }
                //    }
                //    return ms.ToArray();
                //}
                return null;
            }
            else
            {
                Stream oStream = File.OpenRead(FilePath);
                byte[] arrBytes = new byte[oStream.Length];
                int offset = 0;
                while (offset < arrBytes.LongLength)
                {
                    offset += oStream.Read(arrBytes, offset, arrBytes.Length - offset);
                }
                return arrBytes;
            }
        }
        #endregion

        #region 数组组合
        public static byte[] MergerArrays(byte[] Array1, byte[] Array2)
        {
            byte[] Temp = new byte[Array1.Length + Array2.Length];
            Array1.CopyTo(Temp, 0);
            Array2.CopyTo(Temp, Array1.Length);
            return Temp;
        }
        #endregion

        #endregion

    }
}