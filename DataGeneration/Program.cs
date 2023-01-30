using DataGeneration.Entities;
using DataGeneration.Illness;
using DataGeneration.StreetDataModel;
using DataGeneration.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneration
{
    class Program
    {
        static void Main(string[] args)
        {
            QuanLyPhongMachWibuEntities _context = new QuanLyPhongMachWibuEntities();
            Random ran = new Random();
            #region Street
            //List<Root> list = StreetData.ConvertJsonToText();
            //foreach (var i in list)
            //{
            //    foreach (var j in i.district)
            //    {
            //        var result = _context.CityModels.Where(x => x.CityCode.Equals(i.code)).FirstOrDefault();
            //        Entities.DistrictModel a = new Entities.DistrictModel() { DistrictId = Guid.NewGuid(), DistrictName = j.name, DonViHanhChinh = j.pre, Actived = true, CityId = result.CityId };
            //        _context.DistrictModels.Add(a);
            //        _context.SaveChanges();
            //    }
            //}
            //List<string> a = new List<string>();
            //foreach (var i in list)
            //{
            //    foreach (var j in i.district)
            //    {
            //        foreach (var m in j.street)
            //        {
            //            a.Add(m);
            //        }
            //    }
            //}
            //SaveData(list);
            //var result = a.Select(x => x.name).Distinct().ToList();
            //foreach (var k in result)
            //{
            //    _context.WardModels.Add(new WardModel() { WardId = Guid.NewGuid(), WardName = k, Actived = true, DonViHanhChinh = "Thị trấn" });
            //    _context.SaveChanges();
            //}
            #endregion
            //List<DataGeneration.MedicineModel.Medicine> a = Medicine.Medicines.GetAllText();
            #region Medicine
            //var result = a.Select(x => x.MedicineName).Distinct();
            //int m = 1;
            //foreach (var i in result)
            //{
            //    string code = "MED-";
            //    string[] unit = { "Ống", "Viên", "Túp", "Chai" };
            //    if (m.ToString().Length < 6)
            //    {
            //        switch (m.ToString().Length)
            //        {
            //            case 1:
            //                code += "00000" + m.ToString();
            //                break;
            //            case 2:
            //                code += "0000" + m.ToString();
            //                break;
            //            case 3:
            //                code += "000" + m.ToString();
            //                break;
            //            case 4:
            //                code += "00" + m.ToString();
            //                break;
            //            case 5:
            //                code += "0" + m.ToString();
            //                break;
            //        }
            //    }
            //    m++;
            //    _context.MedicineModels.Add(new Entities.MedicineModel() { MedicineId = Guid.NewGuid(), MedicineName = i, MedicineCode = code, Unit = unit[ran.Next(unit.Count())], Actived = true });
            //    _context.SaveChanges();
            //}
            #endregion
            #region Ingredient
            //List<String> b = new List<string>();
            //var result = a.Select(x => x.ActiveIngredient).Distinct().ToList();
            //foreach (var i in result)
            //{
            //    string[] temp = i.Split(';');
            //    foreach (var u in temp)
            //    {
            //        b.Add(u.Trim(' ').ToLower());
            //    }
            //}
            //int m = 1;
            //foreach (var z in b.Distinct())
            //{
            //    string code = "ING-";
            //    if (m.ToString().Length < 6)
            //    {
            //        switch (m.ToString().Length)
            //        {
            //            case 1:
            //                code += "00000" + m.ToString();
            //                break;
            //            case 2:
            //                code += "0000" + m.ToString();
            //                break;
            //            case 3:
            //                code += "000" + m.ToString();
            //                break;
            //            case 4:
            //                code += "00" + m.ToString();
            //                break;
            //            case 5:
            //                code += "0" + m.ToString();
            //                break;
            //        }
            //    }
            //    _context.MedicineIngredientModels.Add(new MedicineIngredientModel() { IngredientId = Guid.NewGuid(), IngredientCode = code, IngredientName = z, Actived = true });
            //    _context.SaveChanges();
            //    m++;
            //}
            #endregion
            #region Ingredient in Medicine
            //var inmed = _context.IngredientInMedicines.Select(x => x.MedicineId).ToList();
            //var medlst = _context.MedicineModels.Where(x => !inmed.Contains(x.MedicineId)).ToList();
            //var ingrelst = _context.MedicineIngredientModels.ToList();
            //foreach (var i in medlst)
            //{
            //    int IngreNum = ran.Next(1, 4);
            //    var templst = ingrelst;
            //    for (int j = 1; j <= IngreNum; j++)
            //    {
            //        int u = ran.Next(0, templst.Count() - 1);
            //        _context.IngredientInMedicines.Add(new IngredientInMedicine() { Id = Guid.NewGuid(), IngredientId = templst.ElementAt(u).IngredientId, MedicineId = i.MedicineId });
            //        _context.SaveChanges();
            //        templst.RemoveAt(u);
            //    }
            //}
            #endregion







            #region Bệnh
            //IllnessData.GetAllBenh();
            #endregion


            #region Nhà cung cấp
            //Medicine.MedicineProvider.GetAllProvider();
            //Medicine.MedicineProvider.GetProviderMed();
            #endregion


            #region Người dùng
            UserData.GetUser();
            #endregion
        }
        public static void SaveData(List<Root> lst)
        {
            QuanLyPhongMachWibuEntities _context = new QuanLyPhongMachWibuEntities();
            foreach (var i in lst)
            {
                foreach (var j in i.district)
                {
                    var district = GetDistrict(j.name);
                    foreach (var m in j.street)
                    {
                        var street = GetStreet(m);
                        DistrictStreetModel temp = new DistrictStreetModel() {DistrictId = district.Result.DistrictId, DistrictStreetId = Guid.NewGuid(), StreetId = street.Result.StreetId };
                        _context.Entry(temp).State = EntityState.Added;
                        _context.SaveChanges();
                    }
                }
            }
        }
        private static async Task<DistrictModel> GetDistrict(string name)
        {
            DistrictModel district = null;

            using (var context = new QuanLyPhongMachWibuEntities())
            {

                district = await (context.DistrictModels.Where(s => s.DistrictName.Equals(name)).FirstOrDefaultAsync<DistrictModel>());
            }

            return district;
        }

        private static async Task<StreetModel> GetStreet(string name)
        {
            StreetModel street = null;

            using (var context = new QuanLyPhongMachWibuEntities())
            {

                street = await (context.StreetModels.Where(s => s.StreetName.Equals(name)).FirstOrDefaultAsync<StreetModel>());
            }

            return street;
        }
    }

}
