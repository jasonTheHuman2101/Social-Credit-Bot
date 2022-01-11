using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace SocCredBotV1
{
    internal class DailySummaryCreator
    {
        private static CreditScoreFile GetCreditScores()
        {
            string json = File.ReadAllText("scores.json");
            CreditScoreFile scoreFile = JsonConvert.DeserializeObject<CreditScoreFile>(json);

            scoreFile.CreditScores.Sort();

            return scoreFile;
        }

        public static string GenerateSummaryText()//This will make the SVG file needed
        {
            CreditScoreFile creditScores = GetCreditScores(); //These are in order already

            string summary = creditScores.CreditScores[0].username + ": " + creditScores.CreditScores[0].score + "\n" + creditScores.CreditScores[1].username + ": " + creditScores.CreditScores[1].score + "\n" + creditScores.CreditScores[2].username + ": " + creditScores.CreditScores[2].score + "\n" + creditScores.CreditScores[3].username + ": " + creditScores.CreditScores[3].score + "\n" + creditScores.CreditScores[4].username + ": " + creditScores.CreditScores[4].score;
            return summary;
        }
    }
}
