using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cerberus
{
    public class misc
    {
        public static Dictionary<int, int> MergeDictionariesWithSum(Dictionary<int, int> dict1, Dictionary<int, int> dict2)
        {
            var mergedDict = new Dictionary<int, int>();

            foreach (var kvp in dict1)
            {
                mergedDict[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in dict2)
            {
                if (mergedDict.ContainsKey(kvp.Key))
                {
                    mergedDict[kvp.Key] += kvp.Value;
                }
                else
                {
                    mergedDict[kvp.Key] = kvp.Value;
                }
            }

            return mergedDict;
        }
    }
}