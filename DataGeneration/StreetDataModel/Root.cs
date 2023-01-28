using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneration.StreetDataModel
{
    public class Root
    {
        public string code { get; set; }
        public string name { get; set; }
        public List<District> district { get; set; }
    }
}
