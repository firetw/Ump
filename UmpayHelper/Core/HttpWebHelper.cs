using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.DirectoryServices.Protocols;
using System.ServiceModel.Security;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Web;

namespace WebLoginer.Core
{
    /// <summary>  
    /// 有关HTTP请求的辅助类  
    /// </summary>  
    public class HttpWebHelper
    {
        //private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        private static readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36";
        private static List<string> _postFilterHeader = new List<string>();

        static HttpWebHelper()
        {
            _postFilterHeader.Add("Accept");
            _postFilterHeader.Add("Content-Length");
            _postFilterHeader.Add("Date");
        }

        public HttpWebResponse TunnelHttpResponse(string url, int? timeout, CookieCollection cookies, Version version = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            HttpWebRequest request = null;

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = WebRequestMethods.Http.Connect;

            //DNT: 1
            //Connection: Keep-Alive
            //Pragma: no-cache

            request.Headers.Add("DNT", "1");
            request.Headers.Add("Pragma", "no-cache");
            //Content-Length: 0
            //request.Headers.Add("Content-Length", "0");
            request.KeepAlive = true;

            if (version != null)
                request.ProtocolVersion = version;

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            else
            {
                request.CookieContainer = new CookieContainer();
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        public static WebOperResult Get(RequestContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            string validateStr = context.Validate();
            if (!string.IsNullOrEmpty(validateStr)) throw new Exception(validateStr);

            string url = context.URL;
            HttpWebRequest request = null;

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            SetRequest(request, context);
            return new WebOperResult(request.GetResponse() as HttpWebResponse, context.Encoding);
        }

        public static Byte[] GetVerifyImage(RequestContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            string validateStr = context.Validate();
            if (!string.IsNullOrEmpty(validateStr)) throw new Exception(validateStr);

            string url = context.URL;
            HttpWebRequest request = null;

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            SetRequest(request, context);

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            if (response == null) return null;
            MemoryStream ms = new MemoryStream();
            if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))//GZIIP处理
                ms = GetMemoryStream(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress));
            else
                ms = GetMemoryStream(response.GetResponseStream());
            ms.Seek(0, SeekOrigin.Begin);
            byte[] responseBytes = ms.ToArray();
            ms.Close();
            return responseBytes;
        }
        private static MemoryStream GetMemoryStream(Stream streamResponse)
        {
            MemoryStream _stream = new MemoryStream();
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = streamResponse.Read(buffer, 0, Length);
            while (bytesRead > 0)
            {
                _stream.Write(buffer, 0, bytesRead);
                bytesRead = streamResponse.Read(buffer, 0, Length);
            }
            return _stream;
        }

        private static void SetRequest(HttpWebRequest request, RequestContext context)
        {
            request.UserAgent = context.UserAgent;
            request.ProtocolVersion = context.ProtocolVersion;
            request.KeepAlive = context.KeepAlive;
            request.Proxy = context.WebProxy;
            if (!string.IsNullOrEmpty(context.Referer))
                request.Referer = context.Referer;

            if (context.Timeout.HasValue)
                request.Timeout = context.Timeout.Value;
            if (context.ReadWriteTimeout.HasValue)
                request.Timeout = context.ReadWriteTimeout.Value;

            request.ServicePoint.Expect100Continue = context.Expect100Continue;
            request.Accept = context.Accept;
            request.Method = context.Method;
            request.ContentType = context.ContentType;


            //modify by wangning 20140625测试下 重用CookieContainer是什么样子的
            //if (context.Cookies != null)
            //{
            //    request.CookieContainer = new CookieContainer();
            //    request.CookieContainer.Add(context.Cookies);
            //}

            //if (context.Cookies != null)
            //{
            //    request.CookieContainer = context.Cookies;
            //}

            if (context.CookieContainer != null)
            {
                request.CookieContainer = context.CookieContainer;
            }
            else
            {
                request.CookieContainer = new CookieContainer();
            }

            request.AllowAutoRedirect = context.Allowautoredirect;

            if (context.Header != null)
            {
                foreach (var item in context.Header.AllKeys)
                {
                    if (!request.Headers.AllKeys.Contains<string>(item))
                    {
                        try
                        {
                            if (_postFilterHeader.Contains(item)) continue;
                            request.Headers.Add(item, context.Header[item]);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(item + "    " + ex);
                        }
                    }
                }
            }


        }
        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        public static WebOperResult Post(RequestContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            string validateStr = context.Validate();
            if (!string.IsNullOrEmpty(validateStr)) throw new Exception(validateStr);

            string url = context.URL;
            Encoding requestEncoding = context.Encoding;

            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }

            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            SetRequest(request, context);


            if (!string.IsNullOrEmpty(context.Postdata))
            {
                byte[] data = requestEncoding.GetBytes(context.Postdata);
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            else
            {
                request.ContentLength = 0;
            }


            if (!context.Allowautoredirect)
            {
                WebOperResult midOper = new WebOperResult(request.GetResponse() as HttpWebResponse, requestEncoding);

                //特殊格式cookie格式转换
                //CookieCollection collection = new CookieCollection();
                //collection.Add(midOper.Cookies[0]);
                //Cookie cookie = midOper.Cookies[1];
                //cookie.Value = cookie.Value + HttpUtility.UrlEncode(",xj");
                //collection.Add(cookie);
                //collection.Add(midOper.Cookies[3]);
                //collection.Add(midOper.Cookies[4]);

                RequestContext getContext = RequestContext.DefaultContext();
                getContext.ContentType = "text/html";
                getContext.URL = midOper.Response.Headers["Location"];

                getContext.CookieContainer = request.CookieContainer;
                getContext.CookieContainer.Add(midOper.Response.Cookies);

                //Cookie为什么没有带过去,因为作用域的问题
                WebOperResult tmpResult = HttpWebHelper.Get(getContext);
                return tmpResult;
            }
            else
            {
                return new WebOperResult(request.GetResponse() as HttpWebResponse, requestEncoding);
            }
            //return new WebOperResult(request.GetResponse() as HttpWebResponse, requestEncoding);
        }
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }


        public static string Login(String url, String paramList, string MyEncode, ref string myCookieContainer, string refer)
        {
            HttpWebResponse res = null;
            string strResult = string.Empty; ;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.UserAgent = DefaultUserAgent;// "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET4.0C; .NET4.0E; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
                if (!string.IsNullOrEmpty(refer))
                {
                    req.Referer = refer;
                }
                req.AllowAutoRedirect = false;
                CookieContainer cookieCon = new CookieContainer();
                req.CookieContainer = cookieCon;
                StringBuilder UrlEncoded = new StringBuilder();
                Char[] reserved = { '?', '=', '&' };
                byte[] SomeBytes = null;

                //if (paramList != null)
                //{
                //    int i = 0, j;
                //    while (i < paramList.Length)
                //    {
                //        j = paramList.IndexOfAny(reserved, i);
                //        if (j == -1)
                //        {
                //            UrlEncoded.Append(HttpUtility.UrlEncode(paramList.Substring(i, paramList.Length - i)));
                //            break;
                //        }
                //        UrlEncoded.Append(HttpUtility.UrlEncode(paramList.Substring(i, j - i)));
                //        UrlEncoded.Append(paramList.Substring(j, 1));
                //        i = j + 1;
                //    }
                //    SomeBytes = Encoding.UTF8.GetBytes(UrlEncoded.ToString());
                //    req.ContentLength = SomeBytes.Length;
                //    Stream newStream = req.GetRequestStream();
                //    newStream.Write(SomeBytes, 0, SomeBytes.Length);
                //    newStream.Close();
                //}
                //else
                //{
                //    req.ContentLength = 0;
                //}

                byte[] data = Encoding.UTF8.GetBytes(paramList);
                req.ContentLength = data.Length;
                using (Stream stream = req.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }


                res = (HttpWebResponse)req.GetResponse();

                //MessageBox.Show(((int)res.StatusCode)+"");

                myCookieContainer = req.CookieContainer.GetCookieHeader(new Uri(url));
                if (res.StatusCode == HttpStatusCode.Redirect)
                {
                    //MessageBox.Show(res.Headers["Location"]);
                    string cookies = res.Headers["Set-Cookie"].Replace(",", "%2c");
                    cookies = cookies.Replace("httponly%2c", "");
                    //MessageBox.Show(cookies);
                    return SendDataByGet(res.Headers["Location"], "", "utf-8", cookies, refer);
                }

                Stream ReceiveStream = res.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding(MyEncode);
                StreamReader sr = new StreamReader(ReceiveStream, encode);
                Char[] read = new Char[256];
                int count = sr.Read(read, 0, 256);
                while (count > 0)
                {
                    String str = new String(read, 0, count);
                    strResult += str;
                    count = sr.Read(read, 0, 256);
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
                throw new Exception("Web.Login：/r/n" + e.ToString());
            }
            finally
            {
                if (res != null)
                {
                    res.Close();
                }
            }
            return strResult;
        }



        public static string SendDataByGet(string url, string data, string encode, string cookies, string refer)
        {
            string retString = string.Empty;
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + (data == "" ? "" : "?") + data);
                string host = string.Empty;
                Uri uri = new Uri(url);
                host = uri.Scheme + "://" + uri.Authority + "/";
                CookieContainer cc = new CookieContainer();
                string[] SegCookie = new string[] { };
                SegCookie = cookies.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < SegCookie.Length; i++)
                {
                    string[] tmp = new string[] { };
                    tmp = SegCookie[i].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tmp.Length == 2)
                    {
                        Cookie c = new Cookie();
                        c.Name = tmp[0].Trim();
                        c.Value = tmp[1];
                        cc.Add(new Uri(host), c);
                    }
                }
                request.CookieContainer = cc;

                request.Method = "GET";
                request.ContentType = "text/html;charset=" + encode;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET4.0C; .NET4.0E; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
                if (!string.IsNullOrEmpty(refer))
                {
                    request.Referer = refer;
                }
                else
                {
                    request.Referer = url;
                }
                response = (HttpWebResponse)request.GetResponse();

                string abc = string.Empty;
                for (int i = 0; i < response.Headers.Count; i++)
                {
                    abc += response.Headers.Keys[i].ToString() + "=====>" + response.Headers[response.Headers.Keys[i].ToString()] + "/r/n";
                }
                //MessageBox.Show(abc);

                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(encode));
                retString = myStreamReader.ReadToEnd();

                myStreamReader.Close();
                myResponseStream.Close();
                request.Abort();
                response.Close();
                return retString;
            }
            catch (Exception ex) { throw new Exception("Web.SendDataByGet：/r/n" + ex.ToString()); }
            finally
            {
                if (response != null)
                    response.Close();
            }

        }
    }
}
