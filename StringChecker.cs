using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SocCredBotV1
{
    public class scResult
    {
        public int score { get; set; }
        public List<string> words { get; set; }

    }

    public class StringChecker
    {
        private static List<string> forbiddenWords;

        private static void loadForbidden()
        {
            string file = "forbidden.txt";
            forbiddenWords = new List<string>(File.ReadAllLines(file));
        }


        public static scResult CheckString(string strToCheck)
        {
            Console.WriteLine(strToCheck);
            loadForbidden();

            scResult res = new scResult();
            res.score = 0;

            if (strToCheck == null)
            {
                return res;
            }
            else
            {
                res.words = new List<string>();
                foreach (string word in forbiddenWords)
                {
                    int score = 0;
                    int count = (strToCheck.Length - strToCheck.Replace(word, "").Length) / word.Length;
                    score = score + count * -5;

                    if (score < 0)
                    {
                        res.words.Add(word);
                        res.score = res.score + score;
                    }
                }

                return res;
            }
        }
    }
}
