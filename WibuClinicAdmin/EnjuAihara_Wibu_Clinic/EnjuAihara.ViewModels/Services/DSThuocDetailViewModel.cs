using System;

namespace EnjuAihara.ViewModels.Services
{
    public class DSThuocDetailViewModel
    {
        public string TenThuoc { get; set; }

        public string NCC { get; set; }

        public decimal? SoLuong { get; set; }

        public double? GiaTien { get; set; }

        public double? Total { get; set; }

        public string Note { get; set; }

        public Guid? WarehouseDetailId { get; set; }

        public string DVT { get; set; }
    }
}
