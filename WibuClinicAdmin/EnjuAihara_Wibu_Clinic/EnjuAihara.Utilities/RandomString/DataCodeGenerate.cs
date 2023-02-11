﻿using System;
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