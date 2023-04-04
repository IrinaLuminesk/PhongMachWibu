using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.ViewModels.MasterData
{
    public class MostAskQuestionSearchViewModel
    {
        //Dùng để search PagingServerSide
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }

        //
        public int STT { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public int? OrderIndex { get; set; }

        public string Actived { get; set; }

        public Guid? MostAskQuestionId { get; set; }
    }
}
