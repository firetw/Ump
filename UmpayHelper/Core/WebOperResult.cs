using ICSharpCode.SharpZipLib.GZip;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace WebLoginer.Core
{
    /// <summary>
    /// 表示WEB登录的返回结果，无法从外部实例化此类
    /// </summary>
    public sealed class WebOperResult
    {
        private CookieCollection cookies;
        private WebHeaderCollection headers;
        private string content;

        public CookieContainer CookieContainer { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="response">HttpWebResponse</param>
        /// <param name="responseEncoding">Encoding</param>
        public WebOperResult(HttpWebResponse response, Encoding responseEncoding)
        {
            Response = response;
            if (null != response.Headers)
            {
                this.headers = new WebHeaderCollection();
                for (int i = 0; i < response.Headers.Count; i++)//Copy Headers
                {
                    this.headers.Add(response.Headers.Keys[i], response.Headers[i]);
                }
            }

            if (null != response.Cookies)
            {
                this.cookies = new CookieCollection();
                Cookie[] cks = new Cookie[response.Cookies.Count];
                response.Cookies.CopyTo(cks, 0);
                foreach (Cookie ck in cks)
                {
                    this.cookies.Add(ck);
                }
            }

            StringDictionary items = Utils.DetectContentTypeHeader(response.Headers[HttpResponseHeader.ContentType]);
            string charset = items["charset"] == null ? "" : items["charset"];
            Encoding enc = Encoding.Default;
            if (charset == "")
            {
                enc = null == responseEncoding ? Encoding.Default : responseEncoding;
            }
            else
            {
                enc = Encoding.GetEncoding(charset);
            }
            string ContentEncoding = response.Headers["Content-Encoding"];//: gzip
            StringBuilder text = new StringBuilder();
            switch (ContentEncoding)
            {
                case "gzip":
                    using (Stream reader = new GZipInputStream(response.GetResponseStream()))
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
                        this.content = text.ToString();
                    }
                    break;
                default:
                    using (Stream stream = response.GetResponseStream())
                    {

                        byte[] buffer = new byte[1024];
                        int n = 0;
                        while ((n = stream.Read(buffer, 0, 1024)) > 0)
                        {
                            text.Append(enc.GetString(buffer, 0, n));
                        }
                        this.content = text.ToString();
                    }
                    break;
            }
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="response">HttpWebResponse</param>
        /// <param name="responseEncoding">Encoding</param>
        public WebOperResult(HttpWebResponse response, Encoding responseEncoding, bool readContent)
        {
            Response = response;
            if (null != response.Headers)
            {
                this.headers = new WebHeaderCollection();
                for (int i = 0; i < response.Headers.Count; i++)//Copy Headers
                {
                    this.headers.Add(response.Headers.Keys[i], response.Headers[i]);
                }
            }

            if (null != response.Cookies)
            {
                this.cookies = new CookieCollection();
                Cookie[] cks = new Cookie[response.Cookies.Count];
                response.Cookies.CopyTo(cks, 0);
                foreach (Cookie ck in cks)
                {
                    this.cookies.Add(ck);
                }
            }

            StringDictionary items = Utils.DetectContentTypeHeader(response.Headers[HttpResponseHeader.ContentType]);
            string charset = items["charset"] == null ? "" : items["charset"];
            Encoding enc = Encoding.Default;
            if (charset == "")
            {
                enc = null == responseEncoding ? Encoding.Default : responseEncoding;
            }
            else
            {
                enc = Encoding.GetEncoding(charset);
            }
            if (readContent)
            {
                using (Stream stream = response.GetResponseStream())
                {
                    StringBuilder text = new StringBuilder();
                    byte[] buffer = new byte[1024];
                    int n = 0;
                    while ((n = stream.Read(buffer, 0, 1024)) > 0)
                    {
                        text.Append(enc.GetString(buffer, 0, n));
                    }
                    this.content = text.ToString();
                }
            }

        }

        /// <summary>
        /// 获取响应头包含的Cookie信息
        /// </summary>
        public CookieCollection Cookies
        {
            get
            {
                return this.cookies;
            }
            set
            {
                this.cookies = value;
            }

        }

        /// <summary>
        /// 获取响应头集合
        /// </summary>
        public WebHeaderCollection Headers
        {
            get
            {
                return this.headers;
            }
        }

        public HttpWebResponse Response { get; private set; }

        /// <summary>
        /// 获取响应内容
        /// </summary>
        public string Text
        {
            get
            {
                return this.content;
            }
        }
    }

    public static class Utils
    {
        /*
         * 用于进行URL修正处理的Regex
         */
        //处理空白,尾部保留字符,锚链接
        private static Regex urlFixRegex1 = new Regex("\\s|/+$|#+.*$|&+$|\\?+$", RegexOptions.Compiled);

        //处理重复的保留字符:/
        private static Regex urlFixRegex2 = new Regex("(?<!:)(/+)", RegexOptions.Compiled);

        //处理重复的保留字符:?
        private static Regex urlFixRegex3 = new Regex("\\?+", RegexOptions.Compiled);

        //处理重复的保留字符:&
        private static Regex urlFixRegex4 = new Regex("&+", RegexOptions.Compiled);

        /// <summary>
        /// 检验绝对路径
        /// </summary>
        private static Regex absUrlRegex = new Regex("^(http|https|ftp)://.+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// 检验MIME类型和字符集
        /// </summary>
        private static Regex contentTypeRegx = new Regex("^\\s*(?<MIME>[\\w\\-/]+)\\s*\\;?\\s*(charset\\s*=\\s*(?<CHARSET>[\\w\\-]+))?.*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// 从HTTP Content-Type头中检查内容,返回的字典可能包含以下键:mime,charset
        /// </summary>
        /// <param name="contentTypeHeader">HTTP Content-Type字符串</param>
        /// <returns>StringDictionary</returns>
        public static StringDictionary DetectContentTypeHeader(string contentTypeHeader)
        {
            StringDictionary items = new StringDictionary();
            if (null == contentTypeHeader || "" == contentTypeHeader)
            {
                return items;
            }

            Match m = contentTypeRegx.Match(contentTypeHeader.ToLower());
            if (m.Success)
            {
                items.Add("mime", m.Groups["MIME"].Value);
                items.Add("charset", m.Groups["CHARSET"].Value);
            }
            return items;
        }

        public static string GetAppPath()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!path.EndsWith("\\"))
            {
                path += "\\";
            }
            return path;
        }

        /// <summary>
        /// 修正URL字符串
        /// </summary>
        /// <param name="url">URL字符串</param>
        /// <returns>修正后的URL字符串</returns>
        public static string AmendUrlString(string url)
        {
            if (null == url || "" == url)
            {
                return "";
            }

            url = urlFixRegex1.Replace(url, "");
            url = urlFixRegex2.Replace(url, "/");
            url = urlFixRegex3.Replace(url, "?");
            url = urlFixRegex4.Replace(url, "&");

            //int p=url.IndexOf('?');
            //if(p>-1)
            //{
            //    url = url.Substring(0, p) + url.Substring(p).ToLower();
            //}
            return url;
        }

        /// <summary>
        /// 检验一个URL字符串是否为绝对URL,为空时返回false
        /// </summary>
        /// <param name="url">URL字符串</param>
        /// <returns>bool</returns>
        public static bool IsAbsoluteUrlString(string url)
        {
            if (null == url || "" == url)
            {
                return false;
            }

            return absUrlRegex.IsMatch(url);
        }

        public static Encoding TryGetStreamEncoding(byte[] buffer)
        {
            if (buffer.Length < 4)
            {
                return Encoding.Default;
            }

            byte[] _BigEndianUnicode = new byte[2] { 0xFE, 0xFF };
            byte[] _Unicode = new byte[2] { 0xFF, 0xFE };
            byte[] _UTF32 = new byte[4] { 0xFF, 0xFE, 0x00, 0x00 };
            byte[] _UTF8 = new byte[3] { 0xEF, 0xBB, 0xBF };

            if (buffer[0] == _BigEndianUnicode[0] && buffer[1] == _BigEndianUnicode[1])
            {
                return Encoding.BigEndianUnicode;
            }

            if (buffer[0] == _UTF32[0] && buffer[1] == _UTF32[1] && buffer[2] == _UTF32[2] && buffer[3] == _UTF32[3])
            {
                return Encoding.UTF32;
            }

            if (buffer[0] == _Unicode[0] && buffer[1] == _Unicode[1])
            {
                return Encoding.Unicode;
            }

            if (buffer[0] == _UTF8[0] && buffer[1] == _UTF8[1] && buffer[2] == _UTF8[2])
            {
                return Encoding.UTF8;
            }

            return Encoding.Default;
        }

        public static string GetBytesFriendly(long bytes)
        {
            return GetBytesFriendly(bytes, "0.00");
        }
        /// <summary>
        /// 获取字节数的友好表示，以B,KB,MB,GB,TB为单位
        /// </summary>
        /// <param name="bytes">字节数</param>
        /// <returns>string</returns>
        public static string GetBytesFriendly(long bytes, string numFormat)
        {
            string unit = "";
            double n = 0f;

            if (bytes >= Math.Pow(2, 40)) //TB
            {
                unit = "TB";
                n = (double)bytes / Math.Pow(2, 40);
            }

            else if (bytes >= Math.Pow(2, 30)) //GB
            {
                unit = "GB";
                n = (double)bytes / Math.Pow(2, 30);
            }

            else if (bytes >= Math.Pow(2, 20)) //MB
            {
                unit = "MB";
                n = (double)bytes / Math.Pow(2, 20);
            }

            else if (bytes >= Math.Pow(2, 10)) //KB
            {
                unit = "KB";
                n = (double)bytes / Math.Pow(2, 10);
            }
            else
            {
                numFormat = "";
            }

            return String.Format("{0" + (numFormat == "" ? "" : ":" + numFormat) + "}{1}", n, unit);
        }

        /// <summary>
        /// 将NameValueCollection转换为URL QueryString格式
        /// </summary>
        /// <param name="nc">NameValueCollection</param>
        /// <returns>转换后的字符串，如果参数nc为null返回空字符串</returns>
        public static string ConvertNameValueString(NameValueCollection nc)
        {
            if (null == nc || nc.Count == 0)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            foreach (string key in nc.Keys)
            {
                if (null != key)
                {
                    sb.AppendFormat("{0}={1}&", key, nc[key]);
                }
            }
            return sb.ToString(0, sb.Length - 1);
        }

        /// <summary>
        /// 将符合URL QueryString格式的字符串转化为NameValueCollection
        /// </summary>
        /// <param name="queryString">URL QueryString格式的字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns>NameValueCollection,queryString为空或者转化失败时返回NULL</returns>
        public static NameValueCollection ConvertNameValueString(string queryString, Encoding encoding)
        {
            if (null == queryString || "" == queryString)
            {
                return null;
            }
            if (null == encoding)
            {
                return System.Web.HttpUtility.ParseQueryString(queryString);
            }
            return System.Web.HttpUtility.ParseQueryString(queryString, encoding);
        }

        /// <summary>
        /// 将符合URL QueryString格式的字符串转化为NameValueCollection,默认使用UTF8编码
        /// </summary>
        /// <param name="queryString">URL QueryString格式的字符串</param>
        /// <returns>NameValueCollection,queryString为空或者转化失败时返回NULL</returns>
        public static NameValueCollection ConvertNameValueString(string queryString)
        {
            return ConvertNameValueString(queryString, null);
        }
    }
}
