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
        public static string[] commands = { "!9gaghot", "!9gagtrending", "!9gagfresh" };
        public static string[] gagLink = { "https://9gag.com/hot", "https://9gag.com/trending", "https://9gag.com/fresh" };
        public static string answer(int Index)
        {
            WebClient  gagUrl= new WebClient();
            string gagSource = string.Empty;
            string gagPostSource = string.Empty;
            try
            {
                gagSource = gagUrl.DownloadString(gagLink[Index]);

            }
            catch(Exception e)
            {
               
            }

            string ArticleMatch = "img class=\"(.*?)\"(.*?)\"(.*?)\"";
            Random randomGag = new Random();
            int randomGagint = randomGag.Next(1, 10);
            int IsRandomGag = 0;

            foreach(Match match in Regex.Matches(gagSource,ArticleMatch))
            {
                IsRandomGag++;
                if(IsRandomGag==randomGagint)
                {
                    gagPostSource = match.Groups[3].Value;
                    break;
                } 
            }
            string ImgName = gagPostSource.Split('/')[4];
            string ImgPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string ImgPatha = ImgPath.Remove(ImgPath.Length - 13) + ImgName;
            using (gagUrl)
            {
                gagUrl.DownloadFile(new Uri(gagPostSource),ImgPatha);
            }
            return ImgPatha;
        }
    }
}
