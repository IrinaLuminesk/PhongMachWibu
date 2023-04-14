﻿using System;

namespace EnjuAihara.ViewModels.MasterData
{
    public class IllnessSearchViewModel
    {
        //Tìm kiếm

        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }

        //Kết quả

        public int STT { get; set; }

        public Guid IllnessId { get; set; }

        public string IllnessName { get; set; }

        public string Status { get; set; }

        public int? TotalCase { get; set; }
    }
}
