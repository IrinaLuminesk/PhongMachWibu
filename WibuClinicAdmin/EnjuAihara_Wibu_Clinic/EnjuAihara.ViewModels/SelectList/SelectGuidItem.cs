using System;

namespace EnjuAihara.ViewModels.SelectList
{
    public class SelectGuidItem
    {
        public Guid id { get; set; }

        public string name { get; set; }
    }


    public class SelectGuidItemWithNull
    {
        public Guid? id { get; set; }

        public string name { get; set; }
    }
}
