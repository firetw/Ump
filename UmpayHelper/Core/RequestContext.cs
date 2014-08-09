using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using ICSharpCode.SharpZipLib.GZip;
using System.Net.Security;


namespace WebLoginer.Core
{
    public class RequestContext
    {
        static System.Net.WebProxy _proxy = null;
        #region 请求URL
        string _URL = string.Empty;
        public string URL
        {
            get { return _URL; }
            set { _URL = value; }
        }
        #endregion

        #region 请求方式
        string _Method = "GET";
        public string Method
        {
            get { return _Method; }
            set { _Method = value; }
        }
        #endregion

        #region 默认请求超时时间
        int? _Timeout = 100000;
        public int? Timeout
        {
            get { return _Timeout; }
            set { _Timeout = value; }
        }
        #endregion

        #region 默认写入Post数据超时时间
        int? _ReadWriteTimeout = 30000;
        public int? ReadWriteTimeout
        {
            get { return _ReadWriteTimeout; }
            set { _ReadWriteTimeout = value; }
        }
        #endregion

        #region 建立持久性连接
        Boolean _KeepAlive = true;
        public Boolean KeepAlive
        {
            get { return _KeepAlive; }
            set { _KeepAlive = value; }
        }
        #endregion

        #region 请求头
        string _Accept = "text/html,application/xhtml+xml,*/*";
        public string Accept
        {
            get { return _Accept; }
            set { _Accept = value; }
        }
        #endregion

        #region 请求响应的HTTP内容类型
        string _ContentType = "application/x-www-form-urlencoded";//一般不是"text/html"
        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }
        #endregion

        #region 浏览器类型
        string _UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36";
        public string UserAgent
        {
            get { return _UserAgent; }
            set { _UserAgent = value; }
        }
        #endregion

        #region 网页数据编码
        Encoding _Encoding = null;
        public Encoding Encoding
        {
            get { return _Encoding; }
            set { _Encoding = value; }
        }
        #endregion

        #region Post的数据类型
        private PostDataType _PostDataType = PostDataType.String;
        public PostDataType PostDataType
        {
            get { return _PostDataType; }
            set { _PostDataType = value; }
        }
        #endregion

        #region Post数据--字符串
        string _Postdata = string.Empty;
        public string Postdata
        {
            get { return _Postdata; }
            set { _Postdata = value; }
        }
        #endregion

        #region Post数据--Byte
        private byte[] _PostdataByte = null;
        public byte[] PostdataByte
        {
            get { return _PostdataByte; }
            set { _PostdataByte = value; }
        }
        #endregion

        #region 代理对象
        private WebProxy _WebProxy;
        public WebProxy WebProxy
        {
            get { return _WebProxy; }
            set { _WebProxy = value; }
        }
        #endregion

        #region 代理服务器用户名
        private string proxyusername = string.Empty;
        public string ProxyUserName
        {
            get { return proxyusername; }
            set { proxyusername = value; }
        }
        #endregion

        #region 代理服务器密码
        private string proxypwd = string.Empty;
        public string ProxyPwd
        {
            get { return proxypwd; }
            set { proxypwd = value; }
        }
        #endregion

        #region 如果要使用IE代理就设置为ieproxy
        private string proxyip = string.Empty;
        public string ProxyIp
        {
            get { return proxyip; }
            set { proxyip = value; }
        }
        #endregion

        #region Cookie对象集合
        CookieCollection cookies = null;
        public CookieCollection Cookies
        {
            get { return cookies; }
            set { cookies = value; }
        }

        public CookieContainer CookieContainer { get; set; }
        #endregion

        #region 请求时的Cookie
        string _Cookie = string.Empty;
        public string Cookie
        {
            get { return _Cookie; }
            set { _Cookie = value; }
        }
        #endregion

        #region 来源地址
        string _Referer = string.Empty;
        public string Referer
        {
            get { return _Referer; }
            set { _Referer = value; }
        }
        #endregion

        #region 证书绝对路径
        string _CerPath = string.Empty;
        public string CerPath
        {
            get { return _CerPath; }
            set { _CerPath = value; }
        }
        #endregion

        #region 全文是否转小写
        private Boolean isToLower = false;
        public Boolean IsToLower
        {
            get { return isToLower; }
            set { isToLower = value; }
        }
        #endregion

        #region 是否允许重定向
        private Boolean allowautoredirect = true;
        public Boolean Allowautoredirect
        {
            get { return allowautoredirect; }
            set { allowautoredirect = value; }
        }
        #endregion

        #region 最大连接数
        private int connectionlimit = 1024;
        public int Connectionlimit
        {
            get { return connectionlimit; }
            set { connectionlimit = value; }
        }
        #endregion

        #region 返回类型
        private ResultType resulttype = ResultType.String;
        public ResultType ResultType
        {
            get { return resulttype; }
            set { resulttype = value; }
        }
        #endregion

        #region header对象
        private WebHeaderCollection header = new WebHeaderCollection();
        public WebHeaderCollection Header
        {
            get { return header; }
            set { header = value; }
        }
        #endregion

        #region HTTP 版本
        private Version _ProtocolVersion = HttpVersion.Version11;
        public Version ProtocolVersion
        {
            get { return _ProtocolVersion; }
            set { _ProtocolVersion = value; }
        }
        #endregion

        #region 是否使用100-Continue
        private Boolean _expect100continue = true;
        public Boolean Expect100Continue
        {
            get { return _expect100continue; }
            set { _expect100continue = value; }
        }
        #endregion

        #region 设置509证书集合
        private X509CertificateCollection _ClentCertificates;
        public X509CertificateCollection ClentCertificates
        {
            get { return _ClentCertificates; }
            set { _ClentCertificates = value; }
        }
        #endregion

        #region Post参数编码
        private Encoding _PostEncoding;
        public Encoding PostEncoding
        {
            get { return _PostEncoding; }
            set { _PostEncoding = value; }
        }
        #endregion

        #region Cookie返回类型
        private ResultCookieType _ResultCookieType = ResultCookieType.String;
        public ResultCookieType ResultCookieType
        {
            get { return _ResultCookieType; }
            set { _ResultCookieType = value; }
        }
        #endregion

        #region 身份验证信息
        private ICredentials _ICredentials = CredentialCache.DefaultCredentials;
        public ICredentials ICredentials
        {
            get { return _ICredentials; }
            set { _ICredentials = value; }
        }
        #endregion


        public string Validate()
        {
            StringBuilder builder = new StringBuilder();
            if (string.IsNullOrEmpty(URL))
            {
                builder.Append(string.Format("{0}为空或NULL", URL));
            }
            return builder.ToString();
        }

        public RequestContext Clone()
        {
            return ObjectCopier.Clone<RequestContext>(this);
        }

        public static RequestContext DefaultContext()
        {
            RequestContext context = new RequestContext
            {
                UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36",
                Method = "GET",
                ContentType = "application/x-www-form-urlencoded",
                Timeout = null,
                ReadWriteTimeout = null,
                Encoding = Encoding.UTF8,
                WebProxy = GetWebProxy(),//
                Header = new WebHeaderCollection(),
                ProtocolVersion = HttpVersion.Version11,
                Cookies = new CookieCollection(),
                KeepAlive = true,
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
                CookieContainer = new CookieContainer()
            };
            context.Header.Add("Accept-Encoding", "gzip,deflate,sdch");
            context.Header.Add("Accept-Language", "zh-CN,zh;q=0.8,en;q=0.6");
            return context;
        }

        public static System.Net.WebProxy GetWebProxy()
        {
            if (System.Configuration.ConfigurationManager.AppSettings["Fiddler"] != "1") return null;
            if (_proxy == null)
                _proxy = new System.Net.WebProxy("127.0.0.1", 8888);
            return _proxy;
        }
    }
}
