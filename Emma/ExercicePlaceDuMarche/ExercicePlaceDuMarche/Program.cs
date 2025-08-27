using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExercicePlaceDuMarche
{
    internal class Program
    {
        const string PATH = @"C:\Users\pt70wvh\Documents\GitHub\323-Programmation_fonctionnelle\exos\marché\Place_du_marche.xlsx";
        static void Main(string[] args)
        {
            if (!File.Exists(PATH))
            {
                Console.WriteLine("Le fichier n'existe pas");
                Console.ReadKey();
            }
            Excel.Application app = new Excel.Application();
            Excel.Workbook workbook = app.Workbooks.Open(PATH);
            Excel.Worksheet worksheet = workbook.Worksheets[2];
            Excel.Range range = worksheet.UsedRange;
            Excel.Range firstRow = range.Rows[1];
            firstRow.Delete();

            List<Product> products = new List<Product>();

            for (int i = 1; i <= 75; i++) //lignes
            {

                Product p = new Product()
                {
                    Location = Convert.ToInt32(range.Cells[i, 1].Value2),
                    Seller = Convert.ToString(range.Cells[i, 2].Value2),
                    ProductName = Convert.ToString(range.Cells[i, 3].Value2),
                    Quantity = Convert.ToInt32(range.Cells[i, 4].Value2),
                    Unit = Convert.ToString(range.Cells[i, 5].Value2),
                    PPU = Convert.ToSingle(range.Cells[i, 6].Value2)
                };

                products.Add(p);

            }

            int PeachCount = 0;

            //Nombre de vendeurs de pêches
            
            foreach (Product p in products)
            {
                if (p.ProductName == "Pêches")
                {
                    PeachCount++;
                }
            }
            Console.WriteLine($"Il y a {PeachCount} vendeurs de pêches");
            

            //Qui a le plus de pastèques, où et combien
            
            List<Product> watermelons = products.Where(p => p.ProductName == "Pastèques").ToList();

            Product bestSeller = null;
            int maxQuantity = -1;

            foreach (var item in watermelons)
            {
                if(item.Quantity > maxQuantity)
                {
                    maxQuantity = item.Quantity;
                    bestSeller = item;
                }
            }

            Console.WriteLine($"C'est {bestSeller.Seller} qui a le plus de pastèques (stand {bestSeller.Location}, {bestSeller.Quantity} pièces)");
            

            int peachCount = products.Where(p => p.ProductName == "Pêches").Count();
            Console.WriteLine($"Il y a {peachCount} vendeurs de pêches");

            Product watermelon = products.Where(p => p.ProductName == "Pastèques").OrderByDescending(p => p.Quantity).First();
            Console.WriteLine($"C'est {watermelon.Seller} qui a le plus de pastèques (stand {watermelon.Location}, {watermelon.Quantity} pièces)");
            Console.ReadKey();
        }

        public class Product
        {
            public int Location;
            public string Seller;
            public string ProductName;
            public int Quantity;
            public string Unit;
            public float PPU;
        }
    }
}
