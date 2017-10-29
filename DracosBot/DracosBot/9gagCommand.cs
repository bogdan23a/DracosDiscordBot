using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace DracosBot
{
    public class _9gagCommand
    {
        public static string[] commands = { "!9gaghot", "!9gagtrending", "!9gagfresh"};
        private static string[] gagLink = { "https://9gag.com/hot", "https://9gag.com/trending", "https://9gag.com/fresh" };
        private static string[] photosPaths;
        private static string photoToDelete;
        public static string answer(int Index)
        {
            //tries to download html 9gag source
            WebClient gagUrl = new WebClient();
            string gagSource = string.Empty;
            string gagPostSource = string.Empty;
            try
            {
                gagSource = gagUrl.DownloadString(gagLink[Index]);
               

            }
            catch (Exception e)
            {

            }

            //declare a model for searching a image source which is used in the next foreach
            string ArticleMatch = "img class=\"(.*?)\"(.*?)\"(.*?)\"";
            //get a random number between 1 and 10 for downloading a random picture from 9gag
            Random randomGag = new Random();
            int randomGagint = randomGag.Next(1, 10);
            int IsRandomGag = 0;
            //foreach match in the html source of page
            foreach (Match match in Regex.Matches(gagSource, ArticleMatch))
            {
                IsRandomGag++;
                if (IsRandomGag == randomGagint)
                {//get picture source
                    gagPostSource = match.Groups[3].Value;
                    
                    break;
                }
            }
            string ImgName = gagPostSource.Split('/')[4];
           
            string ImgPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
          
            string ImgPatha = ImgPath.Remove(ImgPath.Length - 13) + ImgName;
          
            using (gagUrl)
            {   //download the picture from the source in the debug file "ImgPatha"
                
                gagUrl.DownloadFile(new Uri(gagPostSource), ImgPatha);
                
            }
            photoToDelete = ImgPatha;

           
            return ImgPatha;
        }
        public static void clickDreaptaDelete()
        {   //delete photos from cache
            Console.Write(photoToDelete);
            File.Delete(photoToDelete);

        }
    }
}
