using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace SocCredBotV1
{
    class CreditScoreFile
    {
        public List<CreditScores> CreditScores { get; set;}
    }

    class CreditScores 
    {
        public string username { get; set; }
        public int score { get; set; }
    }


    public static class CreditScoresIO
    {
        private static CreditScoreFile LoadCreditScoreFile()
        {
            string json = File.ReadAllText("scores.json");

            CreditScoreFile csf = JsonConvert.DeserializeObject<CreditScoreFile>(json);

            return csf;
        }
        public static void genfile()
        {
            CreditScoreFile csf = new CreditScoreFile();
            csf.CreditScores = new List<CreditScores>();
            CreditScores cs = new CreditScores();
            cs.score = 100;
            cs.username = "tester1";
            csf.CreditScores.Add(cs);
            SaveCreditScoreFile(csf);
        }
        private static void SaveCreditScoreFile(CreditScoreFile csf)
        {
            string json = JsonConvert.SerializeObject(csf);
            File.WriteAllText("scores.json", json);
        }

        public static int ModifyCreditScoreValue(string username, int scoreModifier)
        {
            int newScore = 0;

            bool updatedScore = false;
            CreditScoreFile creds = LoadCreditScoreFile();
            for(int i = 0; i < creds.CreditScores.Count; i++)
            {
                if(creds.CreditScores[i].username == username)
                {
                    newScore = creds.CreditScores[i].score + scoreModifier;
                    creds.CreditScores[i].score = newScore;
                    updatedScore = true;
                    break;
                }
            }
            if (!updatedScore)
            {
                //The person doesn't exist in the score database
                CreditScores newcs = new CreditScores();
                newcs.username = username;
                newcs.score = 100 + scoreModifier;
                creds.CreditScores.Add(newcs);

                newScore = newcs.score;
            }
            SaveCreditScoreFile(creds);
            return newScore;
        }
    }
}