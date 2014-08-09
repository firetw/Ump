using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Umpay.Hjdl
{
    public class SqlInject
    {


        static string[] Dml;
        static string[] Filters;
        static string[] All;

        static SqlInject()
        {
            string tmp1 = "exec|insert|select|delete|update|count|chr|mid|master|truncate|char|declare";
            string tmp2 = "create|and|exec|insert|select|delete|update|count|\\*|chr|mid|master|truncate|char|declare";

            Dml = tmp1.Split('|');
            Filters = tmp2.Split('|');

            List<string> list = new List<string>();
            list.AddRange(Dml);
            list.AddRange(Filters);

            All = list.ToArray();
        }

        /// 分析用户请求是否正常 
        /// < /summary> 
        /// < param name="Str">传入用户提交数据< /param> 
        /// < returns>返回是否含有SQL注入式攻击代码< /returns> 
        public static bool ProcessSqlStr(int type, string str)
        {
            if (string.IsNullOrEmpty(str)) return true;
            string[] filters = null;
            if (type == 1)
            {
                filters = Dml;
            }
            else if (type == 2)
            {
                filters = Filters;
            }
            else if (type == 0)
            {
                filters = All;
            }
            else
            {
                return true;
            }
            try
            {
                foreach (var item in filters)
                {
                    if (Regex.IsMatch(str, item, RegexOptions.IgnoreCase))
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool ProcessSqlStr(int type, params string[] strs)
        {
            if (strs == null || strs.Length < 1) return true;
            bool result = true;
            string[] filters = null;
            if (type == 1)
            {
                filters = Dml;
            }
            else if (type == 2)
            {
                filters = Filters;
            }
            else if (type == 0)
            {
                filters = All;
            }
            else
            {
                return result;
            }
            try
            {
                foreach (var item in strs)
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    foreach (var keyWord in filters)
                    {
                        result = result & !Regex.IsMatch(item, keyWord, RegexOptions.IgnoreCase);
                        if (!result)
                        {
                            return result;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return result;
        }
    }
}


