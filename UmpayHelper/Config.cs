using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Umpay.Hjdl
{
    /// <summary>
    /// Config 的摘要说明
    /// </summary>
    public class Config
    {
        protected static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Config()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        static Config()
        {
            try
            {
                NotifyUrl = System.Configuration.ConfigurationManager.AppSettings["NotifyUrl"];
                UmPayUrl = System.Configuration.ConfigurationManager.AppSettings["UmPayUrl"];
                PageSize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["PageSize"]);

                CodeMap = new Dictionary<string, string>();
                string config = AppDomain.CurrentDomain.BaseDirectory + "/Config/CodeMap.xml";
                XElement root = XElement.Load(config);

                foreach (var item in root.Elements("item"))
                {
                    string key = item.Attribute("code").ToString();
                    if (CodeMap.ContainsKey(key))
                        CodeMap.Add(key, item.Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _log.Error(ex);
            }
        }

        public const string MobilePayPlatform = "MobilePayPlatform";

        public const string MemId = "6882";
        public const int BankType = 3;
        public const string AmtType = "02";
        public const string Version = "3.0";

        public static String NotifyUrl { get; set; }
        public static String UmPayUrl { get; set; }
        public static Int32 PageSize { get; set; }

        public static Dictionary<string, string> CodeMap { get; set; }

    }
}