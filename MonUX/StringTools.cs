using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonUX
{
    public static class StringTools
    {
        public static string Repeat(this string input, int count)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            StringBuilder builder = new StringBuilder();

            for (int index = 0; index < count; index++)
                builder.Append(input);

            return builder.ToString();
        }
    }
}
