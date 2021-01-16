using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DWorldProject.Models.IyziPay
{
    public class RequestFormatter
    {
        public static string FormatPrice(string price)
        {
            if (!price.Contains("."))
            {
                return price + ".0";
            }
            int subStrIndex = 0;
            string priceReversed = StringHelper.Reverse(price);
            for (int i = 0; i < priceReversed.Length; i++)
            {
                if (priceReversed[i].Equals('0'))
                {
                    subStrIndex = i + 1;
                }
                else if (priceReversed[i].Equals('.'))
                {
                    priceReversed = "0" + priceReversed;
                    break;
                }
                else
                {
                    break;
                }
            }
            return StringHelper.Reverse(priceReversed.Substring(subStrIndex));
        }

        public class StringHelper
        {
            public static string Reverse(string s)
            {
                char[] charArray = s.ToCharArray();
                Array.Reverse(charArray);
                return new string(charArray);
            }
        }
    }
}
