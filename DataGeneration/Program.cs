﻿using DataGeneration.Entities;
using DataGeneration.Illness;
using DataGeneration.StreetDataModel;
using DataGeneration.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
            //UserData.GetUser();
            #endregion


            //UsersModel model = new UsersModel()
            //{
            //    FirstName = "Khánh Nhân",
            //    LastName = "Nguyễn",
            //    Actived = true,
            //    Address = "Việt Nam",
            //    Birthday = DateTime.Parse("01-01-2001"),
            //    Email = "1951012084nhan@ou.edu.vn",
            //    ImagePath = "https://res.cloudinary.com/do0kwmira/image/upload/v1660140267/Aihara_Enju_exdv8j.webp",
            //    UserID = Guid.NewGuid(),
            //    Phone = "0123456789",
            //    UserCode = "USER-000202"
            //};
            //_context.UsersModels.Add(model);
            //_context.SaveChanges();


            //AccountModel model = new AccountModel()
            //{
            //    AccountCode = "USER-00002",
            //    AccountId = Guid.NewGuid(),
            //    Actived = true,
            //    ImagePath = "https://res.cloudinary.com/do0kwmira/image/upload/v1662201917/ok3x3kwqelgnusxabxyr.jpg",
            //    CreateDate = DateTime.Now,
            //    LastLoginTime = DateTime.Now,
            //    UserName = "QuyPhuoc123",
            //    Password = SHA256Encrypt("QuyPhuoc123."),
            //    UserId = Guid.Parse("E96BF721-09AB-403B-B29E-1F7F4EB2CCAE")
            //};
            //_context.AccountModels.Add(model);
            //_context.SaveChanges();



            //var list = _context.MedicineProvideModels.ToList();
            //foreach (var i in _context.WarehouseModels.ToList())
            //{
            //    int index = ran.Next(0, list.Count());
            //    i.MedicineProviderId = list[index].MedicineProvideId;
            //    _context.SaveChanges();
            //}


            //var MedicinePro = _context.MedicineProvideModels.ToList();
            //foreach (var i in _context.MedicineCompoundModels.ToList())
            //{
            //    int ra = ran.Next(0, MedicinePro.Count() - 1);
            //    i.MedicineId = MedicinePro[ra].MedicineProvideId;
            //    _context.SaveChanges();
            //}

            var ware = _context.WarehouseModels.ToList();
            int m = 1;
            foreach (var i in ware)
            {
                string code = "WARE-";
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
                i.ImportCode = code;
                i.SalePercentage = 1.1;
                i.SalePrice = i.BoughtPrice * 1.1;
                i.CreateDate = DateTime.Now;
                i.CreateBy = Guid.Parse("4BD00E35-2342-483F-85C2-0997B30414E7");
                _context.SaveChanges();
            }

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

        public static string SHA256Encrypt(string text)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(text));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }

}
