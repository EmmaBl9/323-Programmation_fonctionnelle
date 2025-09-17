using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using static ExercicePlaceDuMarche2.Program;

namespace ExercicePlaceDuMarche2
{
    internal class Program
    {
        static void Main(string[] args)
        {


            List<Product> products = new List<Product>
            {
                new Product { Location = 1, Producer = "Bornand", ProductName = "Pommes", Quantity = 20,Unit = "kg", PricePerUnit = 5.50f },
                new Product { Location = 1, Producer = "Bornand", ProductName = "Poires", Quantity = 16,Unit = "kg", PricePerUnit = 5.50f },
                new Product { Location = 1, Producer = "Bornand", ProductName = "Pastèques", Quantity = 14,Unit = "pièce", PricePerUnit = 5.50f },
                new Product { Location = 1, Producer = "Bornand", ProductName = "Melons", Quantity = 5,Unit = "kg", PricePerUnit = 5.50f },
                new Product { Location = 2, Producer = "Dumont", ProductName = "Noix", Quantity = 20,Unit = "sac", PricePerUnit = 5.50f },
                new Product { Location = 2, Producer = "Dumont", ProductName = "Raisin", Quantity = 6,Unit = "kg", PricePerUnit = 5.50f },
                new Product { Location = 2, Producer = "Dumont", ProductName = "Pruneaux", Quantity = 13,Unit = "kg", PricePerUnit = 5.50f },
                new Product { Location = 2, Producer = "Dumont", ProductName = "Myrtilles", Quantity = 12,Unit = "kg", PricePerUnit = 5.50f },

            };
            //La quantité de groseilles disponibles sur le marché

            var totalGroseilles = products.Where(p => p.ProductName.Equals("Groseilles")).Sum(p => p.Quantity);
            Console.WriteLine($"Quantité totale de groseilles disponible sur le marché : {totalGroseilles}");
            Console.WriteLine();

            //Le chiffre d’affaire possible total pour chaque marchand(tout produit confondu)
            var CA = products
                    .GroupBy(p => p.Producer)
                    .Select(g => new
                    {
                        Producer = g.Key,
                        TotalCA = g.Sum(p => p.Quantity * p.PricePerUnit)
                    });

            //Afichage
            foreach (var item in CA)
            {
                Console.WriteLine($"Chiffre d'affaires de {item.Producer} : {item.TotalCA:F2} €");
            }
            Console.WriteLine();

            //Le plus grand, le plus petit et la moyenne de ces chiffres d’affaire

            var maxCA = CA.Max(x => x.TotalCA);
            var minCA = CA.Min(x => x.TotalCA);
            var avgCA = CA.Average(x => x.TotalCA);

            Console.WriteLine($"Le plus grand chiffre d'affaire est {maxCA}");
            Console.WriteLine($"Le plus petit chiffre d'affaire est {minCA}");
            Console.WriteLine($"La moyenne de ces chiffres d'affaires est {avgCA}");

            //Le marchand ayant le plus de noix à vendre

            //Le marchand ayant le plus d’affinités avec ses produits




            var i18n = new Dictionary<string, string>()
            {
                { "Pommes","Apples"},
                { "Poires","Pears"},
                { "Pastèques","Watermelons"},
                { "Melons","Melons"},
                { "Noix","Nuts"},
                { "Raisin","Grapes"},
                { "Pruneaux","Plums"},
                { "Myrtilles","Blueberries"},
                { "Groseilles","Berries"},
                { "Tomates","Tomatoes"},
                { "Courges","Pumpkins"},
                { "Pêches","Peaches"},
                { "Haricots","Beans"}
            };

            //Les 3 premières lettres du producteur suivies de "..." suivis de la dernière lettre du nom (Dumont --> Dum...t) [pseudo-anonymisation]
            //Le nom de l’aliment en anglais dictionnaire disponible ici
            //Le chiffre d’affaire maximum possible de la journée avec cet aliment(CA = Quantity * PricePerUnit)

            var result = products.Select(product => new
            {
                Seller = product.Producer.Substring(0, 2) + "..." + product.Producer.Last(),
                Product = i18n[product.ProductName],
                CA = product.Quantity * product.PricePerUnit
            }).ToList();

            //Exporter le résultat dans un fichier CSV
            ExportAsCsv(result);

            //Anonymisation renforcée : Premier caractère +nombre de caractères +dernier caractère(ex: "Dumont" → "D5t")
            //Catégorisation automatique : Classer chaque produit selon sa quantité
            //Valeur unitaire ajustée: Prix majoré de 15 % si stock faible, 5 % si normal, prix normal si élevé
            //Indicateur de rentabilité: "Premium" si CA > 100, "Standard" sinon

            ExportDashboard(products.Select(newProduct => new
            {
                Seller = newProduct.Producer.First() + newProduct.Producer.Length.ToString() + newProduct.Producer.Last(),
                Stock = StockCategory(newProduct.Quantity),
                Price = AdjustedPrice(newProduct.PricePerUnit, newProduct.Quantity).ToString("F2"),
                Profitability = IsProfitable(newProduct.Quantity, newProduct.PricePerUnit)

            }));

        }
        static void ExportAsCsv(IEnumerable<dynamic> products)
        {
            using (var writer = new StreamWriter(@"C:\Users\pt70wvh\Documents\GitHub\323-Programmation_fonctionnelle\Emma\ExercicePlaceDuMarche2\Vendeur.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(products.ToList());
            }
        }

        static void ExportDashboard(IEnumerable<dynamic> products)
        {
            var jsonString = JsonSerializer.Serialize(products);
            File.WriteAllText("C:\\Users\\pt70wvh\\Documents\\GitHub\\323-Programmation_fonctionnelle\\Emma\\ExercicePlaceDuMarche2\\Dashboard.json", jsonString);
        }

        static string StockCategory(int Quantity)
        {
            if (Quantity < 10) return "Stock faible";
            else if (Quantity <= 15) return "Stock normal";
            else return "Stock eleve";

        }
        static double AdjustedPrice(float pricePerUnit, int quantity)
        {
            if (quantity < 10) return pricePerUnit * 1.15;
            else if (quantity <= 15) return pricePerUnit * 1.05;
            else return pricePerUnit;
        }
        static string IsProfitable(int Quantity, float pricePerUnit)
        {
            if (pricePerUnit * Quantity >= 100) return "Premium";
            else return "Standard";
        }

        public class Product
        {
            public int Location;
            public string Producer;
            public string ProductName;
            public int Quantity;
            public string Unit;
            public float PricePerUnit;
        }
    }
}