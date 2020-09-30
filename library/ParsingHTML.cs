using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cow.library
{
    class ParsingHTML
    {
        /// <summary>
        /// 取得上市股票資訊
        /// </summary>
        /// <param name="date">搜尋日期</param>
        /// <returns>回傳Key為股票代號,Value為股票資訊</returns>
        public static Dictionary<string, List<string>> GetTWSE(string date)
        {
            Dictionary<string, List<string>> Securities = new Dictionary<string, List<string>>();
            HtmlWeb webClient = new HtmlWeb();
            var doc = webClient.Load($"https://www.twse.com.tw/exchangeReport/MI_INDEX?response=html&date={date}&type=ALLBUT0999");
            var table = doc.DocumentNode.SelectSingleNode("/html/body//th[contains(text(),'每日收盤行情')]");
            if (table != null)
            {
                table = FindTable(table);
                var tbody = table.SelectNodes(".//tbody//tr");
                foreach (var tr in tbody)
                {
                    List<string> Info = new List<string>();
                    foreach (var td in tr.SelectNodes(".//td"))
                    {
                        Info.Add("'" + td.InnerText.Trim().Replace(",", "") + "'");
                    }
                    Securities.Add(Info[0], Info);
                }
            }
            else
            {
                Console.WriteLine("休市");
            }
            return Securities;
        }
        /// <summary>
        /// 找資料在哪一個表格
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static HtmlNode FindTable(HtmlNode node)
        {
            if (node.ParentNode.Name.Equals("table", StringComparison.OrdinalIgnoreCase))
            {
                return node.ParentNode;
            }
            else
            {
                return FindTable(node.ParentNode);
            }
        }
    }
}
