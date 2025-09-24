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

            var mostPresentPeople = people.Where(p => p.films.Length == maxFilmCount);

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

            // On cherche quelle est la planète la plus peuplée ?
            var MostPopulatedPlanet =



            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////



            // On cherche combien de starfighter X-Wing est - ce que je peux m'acheter si je vends un Star Destroyer ?
            // On cherche est - ce qu'Obi-wan Kenobi peut piloter un Millennium Falcon ?
            // On cherche quelle est le vaisseau le plus rapide en vitesse lumière(vmax = vitesse atmosphérique max *ratio hyperespace) ?
            // On cherche combien de vaisseaux sont plus rapides que la moyenne de la vitesse atmosphérique de tous les vaisseaux?
            // On cherche quel est le budget nécessaire(en franc suisse (1 crédit = 0.778 CHF)) à l’achat de la flotte totale?

            // Fonction interne pour faire un appel HTTP à l'API SWAPI et récupérer le JSON
            async Task<string> Call(string query)
            {
                var client = new HttpClient();
                var response = await client.GetAsync("https://swapi.dev/api/" + query);
                var json = await response.Content.ReadAsStringAsync();

                return json;
            }
        }
        // Classe pour stocker le résultat complet de la requête films (nombre total et liste de films)
        class FilmResult
        {
            public int count { get; set; }
            public List<Film> results { get; set; }
        }
        // Classe pour stocker les infos d'un film (ici juste le titre)
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
            public string[] residents { get; set; }
        }
    }
}