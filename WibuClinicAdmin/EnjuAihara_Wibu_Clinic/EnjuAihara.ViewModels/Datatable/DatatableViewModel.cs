using System.Collections.Generic;

namespace EnjuAihara.ViewModels.Datatable
{
    public class DatatableViewModel
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<ColumnViewModel> columns { get; set; }
        public SearchViewModel search { get; set; }
        public List<OrderViewModel> order { get; set; }

        public class ColumnViewModel
        {
            public string data { get; set; }
            public string name { get; set; }
            public bool searchable { get; set; }
            public bool orderable { get; set; }
            public SearchViewModel search { get; set; }
        }

        public class SearchViewModel
        {
            public string value { get; set; }
            public string regex { get; set; }
        }

        public class OrderViewModel
        {
            public int column { get; set; }
            public string dir { get; set; }
        }
    }
}
