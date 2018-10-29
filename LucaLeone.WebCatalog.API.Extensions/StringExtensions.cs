using System;
using System.Collections.Generic;
using System.Text;

namespace LucaLeone.WebCatalog.API.Extensions
{
    public static class StringExtensions
    {
        public static string Format(this string s, object arg0) =>
            string.Format(s, arg0);
        public static string Format(this string s, object arg0, object arg1) =>
            string.Format(s, arg0, arg1);
        public static string Format(this string s, object arg0, object arg1, object arg2) =>
            string.Format(s, arg0, arg1, arg2);
        public static string Format(this string s, params object[] args) =>
            string.Format(s, args);
    }
}
