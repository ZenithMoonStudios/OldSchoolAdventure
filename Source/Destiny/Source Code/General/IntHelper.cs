using System;

namespace Destiny
{
    public class IntHelper
    {
        public static bool TryParse(string p_String, out int p_Value)
        {
            bool result = true;
            p_Value = 0;
            try
            {
                p_Value = Convert.ToInt32(p_String);
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}
