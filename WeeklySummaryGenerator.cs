using System;
using System.Collections.Generic;
using System.Text;
using Svg;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Newtonsoft.Json;

namespace SocCredBotV1
{
    internal class WeeklySummaryGenerator
    {
        public static string[] TPTTI;

        public static string generateImage()
        {
            string path = "weeklysummary.png";
            GenerateSVG();
            var svgDocument = SvgDocument.Open("weeklysummary.svg");
            var bitmap = svgDocument.Draw();
            bitmap.Save(path, ImageFormat.Png);

            return path;
        }

        private static void GenerateSVG()//This will make the SVG file needed
        {
            CreditScoreFile creditScores = GetCreditScores(); //These are sorted already

            string[] pws = new string[5];
            pws[0] = creditScores.CreditScores[0].username + ": " + creditScores.CreditScores[0].score;
            pws[1] = creditScores.CreditScores[1].username + ": " + creditScores.CreditScores[1].score;
            pws[2] = creditScores.CreditScores[2].username + ": " + creditScores.CreditScores[2].score;
            pws[3] = creditScores.CreditScores[3].username + ": " + creditScores.CreditScores[3].score;
            pws[4] = creditScores.CreditScores[4].username + ": " + creditScores.CreditScores[4].score;

            TPTTI = new string[6] { File.ReadAllText("imagefiles/one.txt"), File.ReadAllText("imagefiles/two.txt"), File.ReadAllText("imagefiles/three.txt"), File.ReadAllText("imagefiles/four.txt"), File.ReadAllText("imagefiles/five.txt"), File.ReadAllText("imagefiles/six.txt") };

            string filecontent = TPTTI[0] + pws[0] + TPTTI[1] + pws[1] + TPTTI[2] + pws[2] + TPTTI[3] + pws[3] + TPTTI[4] + pws[4] + TPTTI[5];
            
            File.WriteAllText("weeklysummary.svg", filecontent);

        }

        private static CreditScoreFile GetCreditScores()
        {
            string json = File.ReadAllText("scores.json");
            CreditScoreFile scoreFile = JsonConvert.DeserializeObject<CreditScoreFile>(json);

            scoreFile.CreditScores.Sort();

            return scoreFile;
        }
    }
}
