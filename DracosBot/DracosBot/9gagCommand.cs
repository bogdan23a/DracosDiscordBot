using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;

namespace DracosBot
{
    class _9gagCommand
    {
        public static int Index;
        public static string[] command = { "!9gaghot", "!9gagtrending", "!9gagfresh" };
        public static string[] GagLink = { "https://9gag.com/hot", "https://9gag.com/trending", "https://9gag.com/fresh" };
        private static string Answer(int Index)
        {
            WebClient  GagUrl= new WebClient();
            string GagSource = string.Empty;
            string GagPostSource = string.Empty;
            try
            {
                GagSource = GagUrl.DownloadString(GagLink[Index]);

            }
            catch(Exception e)
            {
               
            }

            string ArticleMatch = "img class=\"(.*?)\"(.*?)\"(.*?)\"";
            Random randomGag = new Random();
            int randomGagint = randomGag.Next(1, 10);
            int IsRandomGag = 0;

            foreach(Match match in Regex.Matches(GagSource,ArticleMatch))
            {
                IsRandomGag++;
                if(IsRandomGag==randomGagint)
                {
                    GagPostSource = match.Groups[3].Value;
                    break;
                } 
            }
            string ImgName = GagPostSource.Split('/')[4];
            string ImgPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string ImgPatha = ImgPath.Remove(ImgPath.Length - 13) + ImgName;
            using (GagUrl)
            {
                GagUrl.DownloadFile(new Uri(GagPostSource),ImgPatha);
            }
            return ImgPatha;
        }
    }
}
