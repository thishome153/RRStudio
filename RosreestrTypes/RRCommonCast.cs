using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RRTypes.CommonCast
{
	public static class CasterCN
	{
		public const char SplitChar = ':';

        public static string CNToId(string cadNum)
        {
            var strNumbers = cadNum.Split(SplitChar);
            List<long> numbers = new List<long>();
            foreach (var item in strNumbers)
            {
                long itemaslong;
                try
                {
                    itemaslong = long.Parse(item);
                    numbers.Add(itemaslong);
                }
                catch (Exception ex)
                {
                    numbers.Add(-1);
                }
            }
            return string.Join(":", numbers.ToArray());
        }

		public static bool IsCN(string value)
		{
			return System.Text.RegularExpressions.Regex.IsMatch(value, "^[0-9]{2}:[0-9]{2}:[0-9]{6,7}:[0-9]{1,}$");
		}

	}


}
