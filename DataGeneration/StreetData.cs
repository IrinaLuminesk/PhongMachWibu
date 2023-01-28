using DataGeneration.Entities;
using DataGeneration.StreetDataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneration
{
    public class StreetData
    {
     
        public static List<string> GetAllStreetFiles()
        {
            string root = @"C:\Project Crowley\PhongMachWibu\Resources\Đường phố Vietnam";
            List<string> fileEntries = Directory.GetFiles(root).ToList();
            return fileEntries;
        }

        public static List<Root> ConvertJsonToText()
        {
            List<string> files = GetAllStreetFiles();
            List<Root> list = new List<Root>();
            if (files.Count > 0 && files != null)
            {
                foreach (var i in files)
                {
                    string text = File.ReadAllText(i);
                    Root a = new Root();
                    a = JsonConvert.DeserializeObject<Root>(text, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore });
                    list.Add(a);
                }
            }
           
            return list;
        }


    }
}
