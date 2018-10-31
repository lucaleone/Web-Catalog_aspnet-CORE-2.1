using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace LucaLeone.WebCatalog.API.Extensions
{
    public static class StringExtensions
    {
        public static string Remove(this string s, string remove) =>
            s.Replace(remove, string.Empty);
        public static string Format(this string s, object arg0) =>
            string.Format(s, arg0);
        public static string Format(this string s, object arg0, object arg1) =>
            string.Format(s, arg0, arg1);
        public static string Format(this string s, object arg0, object arg1, object arg2) =>
            string.Format(s, arg0, arg1, arg2);
        public static string Format(this string s, params object[] args) =>
            string.Format(s, args);

        public static void LogThisMethod(this ILogger logger, string message = "",
                  [System.Runtime.CompilerServices.CallerMemberName] string callerMethodName = "",
                  [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
                  [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            sourceLineNumber = -2;//offset
            var className = sourceFilePath.Split('\\').Last().Remove(".cs");
            logger.LogDebug($"{DateTime.UtcNow:dd/MMM/yyyy} - {sourceLineNumber}: {className}.{callerMethodName}() | {message}");
        }
    }
}
