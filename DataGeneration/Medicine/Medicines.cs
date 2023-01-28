using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataGeneration.Medicine
{
    class Medicines
    {
        public static List<DataGeneration.MedicineModel.Medicine> GetAllText()
        {
            string root = @"C:\Project Crowley\PhongMachWibu\Resources\Drug\Products.txt";
            string sep = "\t";
            List<DataGeneration.MedicineModel.Medicine> a = new List<MedicineModel.Medicine>();
            foreach (string line in System.IO.File.ReadLines(root))
            {
                string[] model = line.Split(sep.ToCharArray());
                a.Add(new MedicineModel.Medicine() { MedicineCode = "MED-" + model[0], MedicineName = model[5], HamLuong = model[2], ActiveIngredient = model[6] });
            }
            return a;
        }
    }
}
