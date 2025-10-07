using System;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace Swapi1
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            // Appel à l'API pour récupérer les films
            var moviesJson = await Call("films");
            // On transforme le JSON récupéré en objet C# qu'on peut utiliser (FilmResult)
            var moviesResult = JsonSerializer.Deserialize<FilmResult>(moviesJson);
            // Récupération de la liste des films
            var movies = moviesResult.results;

            // On cherche le nom du film le plus long
            Console.WriteLine("Longest movie name:" +
              movies.Where(
              m => m.title.Length == movies.Max(m2 => m2.title.Length)) // On filtre les films avec le titre le plus long
              .Select(r => r.title + $" [{r.title.Length} letters]") // On prépare le texte avec le titre et la longueur
              .First());// On prend le premier résultat
            // On affiche aussi le nombre total de films récupérés
            Console.WriteLine($"Total movies: {moviesResult.count}");

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // Appel à l'API pour récupérer les personnages
            var peopleJson = await Call("people");
            // On transforme le JSON récupéré en objet C# qu'on peut utiliser (PeopleResult)
            var peopleResult = JsonSerializer.Deserialize<PeopleResult>(peopleJson);
            // Récupération de la liste des personnages
            var people = peopleResult.results;

            // On cherche le personnage qui est présent dans le plus de film
            var maxFilmCount = people.Max(p => p.films.Length);

            var mostPresentPeople = people.Where(
                p => p.films.Length == maxFilmCount);

            foreach (var person in mostPresentPeople)
            {
                Console.WriteLine($"Most present person: {person.name}, [{person.films.Length} Films]");
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // Appel à l'API pour récupérer les personnages
            var planetsJson = await Call("planets");
            // On transforme le JSON récupéré en objet C# qu'on peut utiliser (PeopleResult)
            var planetsResult = JsonSerializer.Deserialize<PlanetsResult>(planetsJson);
            // Récupération de la liste des personnages
            var planets = planetsResult.results;

            // IMPORTANT : convertir les populations avant les comparaisons
            foreach (var p in planets)
            {
                p.ParsePopulation();
            }

            // On cherche quelle est la planète la plus peuplée
            var maxPeopleCount = planets.Max(p => p.populationNumber);
            var MostPopulatedPlanet = planets.Where(p => p.populationNumber == maxPeopleCount).First();

            Console.WriteLine($"La planète la plus peuplée est {MostPopulatedPlanet.name} [{MostPopulatedPlanet.populationNumber} personnes]");

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Appel à l'API pour récupérer les vaisseaux
            var starshipsJson = await Call("starships");
            var starshipsResult = JsonSerializer.Deserialize<StarshipResult>(starshipsJson);
            var starships = starshipsResult.results;

            // Conversion des valeurs numériques
            foreach (var s in starships)
            {
                s.ParseNumericValues();
            }

            // 1) Combien de starfighter X-Wing peut-on acheter si on vend un Star Destroyer ?
            var starDestroyer = starships.FirstOrDefault(s => s.name.Contains("Star Destroyer", StringComparison.OrdinalIgnoreCase));
            var xWing = starships.FirstOrDefault(s => s.name.Contains("X-wing", StringComparison.OrdinalIgnoreCase));

            if (starDestroyer != null && xWing != null && xWing.cost_in_creditsNumber > 0)
            {
                var howManyXWing = starDestroyer.cost_in_creditsNumber / xWing.cost_in_creditsNumber;
                Console.WriteLine($"Avec la vente d’un {starDestroyer.name}, on peut acheter {howManyXWing} X-Wing.");
            }
            else
            {
                Console.WriteLine("Impossible de calculer le nombre de X-Wing (prix manquant).");
            }

            // 2) Est-ce qu’Obi-Wan Kenobi peut piloter un Millennium Falcon ?
            var obiWan = people.FirstOrDefault(p => p.name == "Obi-Wan Kenobi");
            var millenniumFalcon = starships.FirstOrDefault(s => s.name == "Millennium Falcon");

            // On suppose que si Obi-Wan est dans les pilotes du Millennium Falcon
            if (millenniumFalcon != null && millenniumFalcon.pilots.Contains("https://swapi.dev/api/people/10/")) // Obi-Wan id =10
            {
                Console.WriteLine("Obi-Wan Kenobi peut piloter le Millennium Falcon.");
            }
            else
            {
                Console.WriteLine("Obi-Wan Kenobi ne peut PAS piloter le Millennium Falcon.");
            }

            // 3) Quel est le vaisseau le plus rapide en vitesse lumière (vmax = atmosphérique max * hyperdrive) ?
            var fastest = starships
                .Where(s => s.max_atmosphering_speedNumber > 0 && s.hyperdrive_ratingNumber > 0)
                .OrderByDescending(s => s.max_atmosphering_speedNumber * s.hyperdrive_ratingNumber)
                .FirstOrDefault();

            if (fastest != null)
            {
                Console.WriteLine($"Le vaisseau le plus rapide est {fastest.name} avec vmax = {fastest.max_atmosphering_speedNumber * fastest.hyperdrive_ratingNumber}");
            }

            // 4) Combien de vaisseaux sont plus rapides que la moyenne de la vitesse atmosphérique ?
            var validStarships = starships.Where(s => s.max_atmosphering_speedNumber > 0).ToList();
            var avgSpeed = validStarships.Average(s => s.max_atmosphering_speedNumber);
            var fasterThanAvg = validStarships.Count(s => s.max_atmosphering_speedNumber > avgSpeed);

            Console.WriteLine($"{fasterThanAvg} vaisseaux sont plus rapides que la moyenne ({avgSpeed}).");

            // 5) Budget total de la flotte en CHF (1 crédit = 0.778 CHF)
            var totalCostCredits = starships.Where(s => s.cost_in_creditsNumber > 0).Sum(s => s.cost_in_creditsNumber);
            var totalCostCHF = totalCostCredits * 0.778M;

            Console.WriteLine($"Le budget total de la flotte est {totalCostCredits} crédits ≈ {totalCostCHF} CHF.");

            Console.ReadLine();

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // Fonction interne pour faire un appel HTTP
            async Task<string> Call(string query)
            {
                var client = new HttpClient();
                var response = await client.GetAsync("https://swapi.dev/api/" + query);
                var json = await response.Content.ReadAsStringAsync();

                return json;
            }
        }

        class FilmResult
        {
            public int count { get; set; }
            public List<Film> results { get; set; }
        }
        class Film
        {
            public string title { get; set; }
        }

        class PeopleResult
        {
            public List<People> results { get; set; }
        }
        class People
        {
            public string name { get; set; }
            public string[] films { get; set; }
        }

        class PlanetsResult
        {
            public List<Planets> results { get; set; }
        }
        class Planets
        {
            public string name { get; set; }
            public string population { get; set; }
            public ulong populationNumber { get; set; }
            public string[] residents { get; set; }

            public void ParsePopulation()
            {
                if (ulong.TryParse(population, out ulong result))
                    populationNumber = result;
                else
                    populationNumber = 0;
            }
        }

        class StarshipResult
        {
            public List<Starship> results { get; set; }
        }
        class Starship
        {
            public string name { get; set; }
            public string cost_in_credits { get; set; }
            public string max_atmosphering_speed { get; set; }
            public string hyperdrive_rating { get; set; }
            public string[] pilots { get; set; }

            public decimal cost_in_creditsNumber { get; set; }
            public decimal max_atmosphering_speedNumber { get; set; }
            public decimal hyperdrive_ratingNumber { get; set; }

            public void ParseNumericValues()
            {
                cost_in_creditsNumber = decimal.TryParse(cost_in_credits, out var c) ? c : 0;
                max_atmosphering_speedNumber = decimal.TryParse(max_atmosphering_speed, out var s) ? s : 0;
                hyperdrive_ratingNumber = decimal.TryParse(hyperdrive_rating, out var h) ? h : 0;
            }
        }
    }
}