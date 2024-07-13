using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace compression_tool
{
    public class CompressionTool
    {
        public static Dictionary<char, int> GetCharCount(string[] args)
        {
            string filePath = args[0];
            Dictionary<char, int> charCountDictionary = new Dictionary<char, int>();

            try
            {
                using StreamReader streamReader = new StreamReader(filePath);

                string content = streamReader.ReadToEnd();


                for (int i = 0; i < content.Length; i++)
                {
                    if (charCountDictionary.ContainsKey(content[i])) charCountDictionary[content[i]]++;
                    else charCountDictionary.Add(content[i], 1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to read file: " + filePath + "\nError: " + ex.Message);
            }

            return charCountDictionary;
        }

        public static List<KeyValuePair<char, int>> CharCountPiorityQueue(Dictionary<char, int> charCountDict)
        {
            List<KeyValuePair<char,int>> prioQueue = new List<KeyValuePair<char,int>>();

            foreach(var keyValuePair in charCountDict)
            {
                prioQueue.Add(new KeyValuePair<char, int>(keyValuePair.Key, keyValuePair.Value));
            }

            prioQueue.Sort((a, b) => a.Value.CompareTo(b.Value));

            return prioQueue;
        }
    }
}