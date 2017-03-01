using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PDCompare
{
    public static class Util
    {
        /// <summary>
        /// 压缩SQL语句。去掉所有注释和格式，返回一行可执行的紧缩的SQL语句。
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        public static string CompressSqlText(string sqlText)
        {
            string result = sqlText;

            // 去掉单行注释
            result = new Regex("--.*").Replace(result, "");

            // 去掉所有换行符
            result = result.Replace(Environment.NewLine, " ");
            result = result.Replace("\r", " ");
            result = result.Replace("\n", " ");

            // 去掉所有多行注释
            result = new Regex("/[*].*?[*]/").Replace(result, " ");

            // 合并连续空格
            result = new Regex("\\s+").Replace(result, " ");

            // 去掉不必要的空格
            result = new Regex("\\s*[(]\\s*").Replace(result, "(");
            result = new Regex("\\s*[)]\\s*").Replace(result, ")");
            result = new Regex("\\s*[;]\\s*").Replace(result, ";");
            result = new Regex("\\s*[,]\\s*").Replace(result, " ");
            result = new Regex("\\s*[+]\\s*").Replace(result, "+");
            result = new Regex("\\s*[-]\\s*").Replace(result, "-");
            result = new Regex("\\s*[*]\\s*").Replace(result, "*");
            result = new Regex("\\s*[/]\\s*").Replace(result, "/");

            result = new Regex("\\s*[%]\\s*").Replace(result, "%");
            result = new Regex("\\s*[|][|]\\s*").Replace(result, "||");
            result = new Regex("\\s*[=]\\s*").Replace(result, "=");

            result = new Regex("\\s*[<]\\s*").Replace(result, "<");
            result = new Regex("\\s*[<][=]\\s*").Replace(result, "<=");
            result = new Regex("\\s*[>]\\s*").Replace(result, ">");
            result = new Regex("\\s*[>][=]\\s*").Replace(result, ">=");
            result = new Regex("\\s*[!][=]\\s*").Replace(result, "!=");
            result = new Regex("\\s*[<][>]\\s*").Replace(result, "<>");

            // 去掉首尾的空格
            result = result.Trim();

            return result;
        }
    }
}
