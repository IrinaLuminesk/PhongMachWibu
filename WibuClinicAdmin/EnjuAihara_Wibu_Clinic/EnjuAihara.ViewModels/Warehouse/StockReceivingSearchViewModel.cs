using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.ViewModels.Warehouse
{
    public class StockReceivingSearchViewModel
    {
        //Dùng để search PagingServerSide
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }


        //Các tham số search
        public string ImportCodeSearch { get; set; }
        public Guid? MedicineIdSearch { get; set; }
        public Guid? ProviderIdSearch { get; set; }
        public bool Actived { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        //Ket qua tra ve
        public int STT;
        public string ProviderName { get; set; }
        public string MedicineCode { get; set; }
        public string ImportCode { get; set; }
        public string MedicineName { get; set; }
        public decimal? BoughtQuantity { get; set; }
        public string Status { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public double? BoughtPrice { get; set; }
        public double? SalePercentage { get; set; }
        public double? SalePrice { get; set; }
        public string CreateBy { get; set; }
        public Guid? WarehouseMasterId { get; set; }
        public Guid? WarehouseDetailId { get; set; }
        public string ExpiryString { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? BoughtDate { get; set; }
        public string CreateDateString { get; set; }
        public string BoughtDateString { get; set; }

    }
}
