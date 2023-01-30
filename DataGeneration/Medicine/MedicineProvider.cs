using DataGeneration.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneration.Medicine
{
    class MedicineProvider
    {
        public static QuanLyPhongMachWibuEntities _context = new QuanLyPhongMachWibuEntities();
        public static Random ran = new Random();
        public static void GetAllProvider()
        {
            string root = @"C:\Project Crowley\PhongMachWibu\Resources\Drug\Applications.txt";
            List<string> list = File.ReadLines(root).ToList();
            list.RemoveAt(0);
            List<string> prolst = new List<string>();
            string sep = "\t";
            foreach (var i in list)
            {
                string[] temp = i.Split(sep.ToCharArray());
                prolst.Add(temp[3]);
            }
            int m = 1;
            foreach (var u in prolst.Distinct().Take(500).ToList())
            {
                string code = "PRO-";
                if (m.ToString().Length < 6)
                {
                    switch (m.ToString().Length)
                    {
                        case 1:
                            code += "00000" + m.ToString();
                            break;
                        case 2:
                            code += "0000" + m.ToString();
                            break;
                        case 3:
                            code += "000" + m.ToString();
                            break;
                        case 4:
                            code += "00" + m.ToString();
                            break;
                        case 5:
                            code += "0" + m.ToString();
                            break;
                    }
                }
                m++;
                ProviderModel model = new ProviderModel() { ProviderId = Guid.NewGuid(), Actived = true, ProviderCode = code, ProviderName = u.ToString() };
                _context.ProviderModels.Add(model);
                _context.SaveChanges();
            }

        }


        public static void GetProviderMed()
        {
            List<ProviderModel> prolst = _context.ProviderModels.ToList();
            List<DataGeneration.Entities.MedicineModel> med = _context.MedicineModels.ToList();
            foreach (var i in med)
            {
                int numberofpro = ran.Next(1, 3);
                for (int m = 0; m < numberofpro; m++)
                {
                    int RandomPro = ran.Next(0, prolst.Count() - 1);
                    int RanMonth = ran.Next(6, 20);
                    double boughtprice = ran.Next(100000, 1000000);
                    decimal boughtquantity = (decimal)ran.Next(100000, 200000);
                    ProviderModel pro = prolst.ElementAt(RandomPro);
                    MedicineProvideModel model = new MedicineProvideModel()
                    {
                        MedicineProvideId = Guid.NewGuid(),
                        ProviderId = pro.ProviderId,
                        Actived = true,
                        MedicineId = i.MedicineId,
                        BoughtDate = DateTime.Now,
                        ExpiredDate = DateTime.Now.AddMonths(RanMonth),
                        BoughtPrice = Convert.ToDouble(boughtprice),
                        BoughtQuantity = boughtquantity,
                        InstockQuantity = boughtquantity
                    };
                    _context.MedicineProvideModels.Add(model);
                    _context.SaveChanges();
                }
            }
        }
    }
}
