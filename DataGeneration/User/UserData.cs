using DataGeneration.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneration.User
{
    class UserData
    {
        public static QuanLyPhongMachWibuEntities _context = new QuanLyPhongMachWibuEntities();
        public static Random ran = new Random();
        public static void GetUser()
        {

            string root = @"C:\Project Crowley\PhongMachWibu\Resources\Người dùng\User.txt";
            List<string> list = File.ReadLines(root).ToList();
            int m = 1;
            foreach (var i in list)
            {
                string[] temp = i.Split(',');
                string code = "USER-";

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
                UsersModel model = new UsersModel() 
                {
                    UserID = Guid.NewGuid(),
                    UserCode = code,
                    Actived = true, 
                    FirstName = temp[0],
                    LastName = temp[1],
                    Birthday = Convert.ToDateTime(temp[2]),
                    Address = (ran.Next(1,100)).ToString() + " " + temp[3],
                    Phone = temp[4],
                    Email = temp[5]
                };

                _context.UsersModels.Add(model);
                _context.SaveChanges();
            }
           

        }
    }
}
