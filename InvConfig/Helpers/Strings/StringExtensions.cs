using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helpers.Strings
{
    public static class StringExtensions
    {
        public static string Left(this string p_param, int p_left)
        {
            try
            {
                return p_param.Substring(0, p_left);
            }
            catch
            {
                throw;
            }
        }

        public static string Right(this string p_param, int p_length)
        {
            try
            {
                return p_param.Substring(p_param.Length - p_length);
            }
            catch
            {
                throw;
            }
            
        }

        public static string Mid(this string p_param, int p_startIndex, int p_length)
        {
            try
            {
                return  p_param.Substring(p_startIndex, p_length);
            }
            catch
            {
                throw;
            }
        }

        public static string Mid(this string p_param, int p_startIndex)
        {
            try 
            {
               return p_param.Substring(p_startIndex);
            }
            catch
            {
                throw;
            }
        }

        public static int Instr(this string p_param, string p_searchString, StringComparison p_compareMethod)
        {
            return p_param.IndexOf(p_searchString, p_compareMethod);
        }
    }
}
