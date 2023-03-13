using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnjuAihara.EntityFramework;

namespace EnjuAihara.Utilities.RandomString
{
    public class DataCodeGenerate
    {
        public static QuanLyPhongMachWibuEntities _context = new QuanLyPhongMachWibuEntities();
        public static string UserCodeGen()
        {
            string code = _context.AccountModels.OrderByDescending(x => x.AccountCode).Take(1).FirstOrDefault().AccountCode;
            string[] temp = code.Split('-');
            return CreateCode(temp[1], temp[0]);
        }
        public static string ProviderCodeGen()
        {
            string code = _context.ProviderModels.OrderByDescending(x => x.ProviderCode).Take(1).FirstOrDefault().ProviderCode;
            string[] temp = code.Split('-');
            return CreateCode(temp[1], temp[0]);
        }
        public static string IngredientCodeGen()
        {
            string code = _context.IngredientModels.OrderByDescending(x => x.IngredientCode).Take(1).FirstOrDefault().IngredientCode;
            string[] temp = code.Split('-');
            return CreateCode(temp[1], temp[0]);
        }
        public static string NguoiDungCodeGen()
        {
            string code = _context.UsersModels.OrderByDescending(x => x.UserCode).Take(1).FirstOrDefault().UserCode;
            string[] temp = code.Split('-');
            return CreateCode(temp[1], temp[0]);
        }
        public static string ThuocCodeGen()
        {
            string code = _context.MedicineModels.OrderByDescending(x => x.MedicineCode).Take(1).FirstOrDefault().MedicineCode;
            string[] temp = code.Split('-');
            return CreateCode(temp[1], temp[0]);
        }
        public static string WarehouseCodeGen()
        {
            string code = _context.WarehouseMasterModels.OrderByDescending(x => x.ImportCode).Take(1).FirstOrDefault().ImportCode;
            string[] temp = code.Split('-');
            return CreateCode(temp[1], temp[0]);
        }
        public static string CreateCode(string num, string table)
        {
            string code = table;
            int m = Convert.ToInt32(num);
            m += 1;
            if (m.ToString().Length < 6)
            {
                switch (m.ToString().Length)
                {
                    case 1:
                        code += "-00000" + m.ToString();
                        break;
                    case 2:
                        code += "-0000" + m.ToString();
                        break;
                    case 3:
                        code += "-000" + m.ToString();
                        break;
                    case 4:
                        code += "-00" + m.ToString();
                        break;
                    case 5:
                        code += "-0" + m.ToString();
                        break;
                }
            }
            return code;
        }
    }
}
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