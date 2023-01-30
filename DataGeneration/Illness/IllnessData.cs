using DataGeneration.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneration.Illness
{
    public class IllnessData
    {
        public static QuanLyPhongMachWibuEntities _context = new QuanLyPhongMachWibuEntities();
        public static void GetAllBenh()
        {
            string root = @"C:\Project Crowley\PhongMachWibu\Resources\Các loại bệnh\Benh.txt";
            List<string> list = File.ReadLines(root).Distinct().ToList();
            foreach (var i in list)
            {
                IllnessModel model = new IllnessModel() { IllnessId = Guid.NewGuid(), IllnessName = i.ToString(), Actived = true};
                _context.IllnessModels.Add(model);
                _context.SaveChanges();
            }
            
        }
    }
}
