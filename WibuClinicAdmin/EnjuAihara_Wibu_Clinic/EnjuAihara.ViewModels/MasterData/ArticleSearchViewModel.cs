using System;

namespace EnjuAihara.ViewModels.MasterData
{
    public class ArticleSearchViewModel
    {
        //Dùng để search PagingServerSide
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }

        public int STT { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public DateTime? CreateDate { get; set; }

        public string CreateDateString { get; set; }

        public string CreateBy { get; set; }

  
        public string Status { get; set; }

        public Guid? ArticleId { get; set; }



       
    }
}
