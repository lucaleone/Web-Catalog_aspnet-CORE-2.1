using System;
using System.Collections.Generic;
using System.Text;

namespace LucaLeone.WebCatalog.API.Extensions
{
    public static class GenericExtensions
    {
        public static void ThrowIfNull<T>(this T obj, Exception ex) where T : class
        {
            if(obj == null)
                throw ex;
        }

        public static bool Between<T>(this T obj, T lower, T upper, BetweenOptions option = BetweenOptions.Exclusive) where T : IComparable<T>
        {
            if(option == BetweenOptions.Exclusive)
                return obj.CompareTo(lower) > 0 && obj.CompareTo(upper) < 0;
            else
                return obj.CompareTo(lower) >= 0 && obj.CompareTo(upper) <= 0;
        }
    }

    public enum BetweenOptions{
        Exclusive,
        Inclusive
    }
}
