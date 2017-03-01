using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDCompare
{
    public static class StringExtension
    {

        /// <summary>
        /// 返回一个空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static StringBuilder AppendSpace(this StringBuilder sb)
        {
            return sb.Append(" ");
        }

        /// <summary>
        /// 为空时返回空GUID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string ToGuidString(this String request)
        {
            if (string.IsNullOrWhiteSpace(request))
                return Guid.Empty.ToString();
            return request;
        }

        /// <summary>
        /// 字符串转化为int
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int ToInt32(this String s)
        {
            int result;
            int.TryParse(s, out result);
            return result;
        }

        public static decimal? TryToDecimal(this String s)
        {
            decimal result;
            if (decimal.TryParse(s, out result))
                return result;
            else
                return null;
        }

        public static int? ConvertInt32(this String s)
        {
            if (s == "")
                return null;
            return Convert.ToInt32(s);
        }

        public static DateTime? ConvertDateTime(this String s)
        {
            if (s == "")
                return null;
            return Convert.ToDateTime(s);
        }

        public static string NotWhiteSpace(this String s)
        {
            if (s == "")
                return null;
            else
                return s;
        }

        /// <summary>
        /// 解决Linq动态模糊查询的问题，String自己带的Contains方法 被操作的对象不能为空
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool LinqLikeContains(this string s, string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            return value.Contains(s);
        }

        /// <summary>
        /// 如果s不为空，返回value；否则返回s
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string WhenNotNullOrEmpty(this string s, string value)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            else
                return value;
        }

        /// <summary>
        /// 如果s不为空，调用 action
        /// </summary>
        /// <param name="s"></param>
        /// <param name="action"></param>
        public static void WhenNotNullOrEmptyDo(this string s, Action action)
        {
            if (string.IsNullOrEmpty(s) == false)
                action();
        }

        public static string TryToLower(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            else
                return s.ToLower();
        }

        /// <summary>
        /// 返回由separator分割的字符串数组。
        /// <remarks>返回值不包括含有空字符串的数组元素</remarks>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] SplitWithoutEmpty(this string str, string separator)
        {
            return str.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 截取过长字符串，以...结尾
        /// </summary>
        /// <param name="htmlStr"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SubHtmlString(this string htmlStr, int length)
        {
            if (string.IsNullOrEmpty(htmlStr) || htmlStr.Length < length)
            {
                return htmlStr;
            }
            else
            {
                return htmlStr.Trim().Substring(0, length) + "...";
            }
        }

        /// <summary>
        /// 判断字符串是否包含在 stringList 列表中
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="stringList"></param>
        /// <returns></returns>
        public static bool In(this string arg, params string[] stringList)
        {
            if (stringList == null || stringList.Length <= 0)
            {
                return false;
            }

            foreach (var s in stringList)
            {
                // 只要有一个元素与this相等，就返回true
                if (arg == s)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 判断指定字符串是否不存在于 stringList 中
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="stringList"></param>
        /// <returns></returns>
        public static bool NotIn(this string arg, params string[] stringList)
        {
            if (stringList == null || stringList.Length <= 0)
            {
                return true;
            }

            bool result = !stringList.Any(t => t == arg);
            return result;
        }

        /// <summary>
        /// 如果 arg 是 null，返回 string.Empty。如果arg不是null返回 arg
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string Nvl(this string arg)
        {
            if (string.IsNullOrEmpty(arg))
                return string.Empty;

            return arg;
        }

        /// <summary>
        /// 如果 arg 是 null，返回 string.Empty。如果arg不是null返回 arg
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="nullText"></param>
        /// <returns></returns>
        public static string Nvl(this string arg, string nullText)
        {
            if (string.IsNullOrEmpty(arg))
                return nullText;

            return arg;
        }

        /// <summary>
        /// 获取字符串的前 length 个字符
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetFirst(this string arg, int length)
        {
            if (string.IsNullOrEmpty(arg))
            {
                return string.Empty;
            }

            if (arg.Length <= length)
            {
                return arg;
            }
            else
            {
                return arg.Substring(0, length);
            }
        }
    }
}
