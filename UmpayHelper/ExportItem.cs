using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Umpay.Hjdl
{
    public class ExportItem
    {
        private Dictionary<string, Column> _columns = null;

        public string ConfigFile { get; set; }

        public Dictionary<string, Column> Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = new Dictionary<string, Column>();

                    XElement root = XElement.Load(AppDomain.CurrentDomain.BaseDirectory + "/Config/" + ConfigFile);
                    foreach (var item in root.Element("Columns").Elements("Column"))
                    {
                        string colName = GetAttribute(item, "LabelCn");// item.Attribute("LabelCn").Value;
                        string dbName = GetAttribute(item, "LabelEn");// item.Attribute("LabelEn").Value;
                        string colWidth = GetAttribute(item, "Width");// item.Attribute("LabelEn").Value;
                        string cellType = GetAttribute(item, "CellType");

                        Column col = new Column
                        {
                            LabelCn = colName,
                            LabelEn = dbName,
                            Width = string.IsNullOrEmpty(colWidth) ? 5000 : Convert.ToInt32(colWidth),
                            CellType = string.IsNullOrEmpty(cellType) ? 1 : Convert.ToInt32(cellType),
                        };

                        if (!_columns.ContainsKey(colName))
                        {
                            _columns.Add(colName, col);
                        }
                    }
                }
                return _columns;
            }
        }

        private string GetAttribute(XElement item, string attName)
        {
            if (item == null) return string.Empty;
            XAttribute att = item.Attribute(attName);
            if (att == null) return string.Empty;
            return att.Value;
        }

        public DataTable Data { get; set; }
    }

    public class Column
    {
        public string LabelCn { get; set; }

        public string LabelEn { get; set; }

        public int Width { get; set; }

        public int CellType { get; set; }
    }
}
