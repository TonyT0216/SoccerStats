﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace SoccerStats
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directory = new DirectoryInfo(currentDirectory);
            var fileName = Path.Combine(directory.FullName, "SoccerGameResults.csv");
            var fileContents = ReadSoccerResults(fileName);
            fileName = Path.Combine(directory.FullName, "players.json");
            var players = DeserializePlayers(fileName);
            var topTenPlayers = GetTopTenPlayers(players);
            foreach (var player in topTenPlayers)
            {
                List<NewsResult> newsResults = GetNewsForPlayer(string.Format("{0} {1}", player.FirstName, player.LastName));
                foreach(var result in newsResults)
                {
                    Console.WriteLine(string.Format("Date: {0:f}, Headline: {1}, Summary: {2} \r \n", result.DatePublished, result.Headline, result.Summary));
                    Console.ReadKey();
                }
            }

            fileName = Path.Combine(directory.FullName, "topten.json");
            SerializePlayersToFile(topTenPlayers, fileName);
            //foreach(var player in players)
            //{
            //    Console.WriteLine(player.FirstName);  
            //}
            // var file = new FileInfo(fileName);
            // If block will run if the file exists
            //if (file.Exists)
            //    {
            //    using (var reader = new StreamReader(file.FullName))
            //    {
            //        Console.SetIn(reader);
            //        Console.WriteLine(Console.ReadLine());
            //    }
            //}


            //var files = directory.GetFiles("*.txt");
            //foreach(var file in files)
            //{
            //    Console.WriteLine(file.Name);
            //}

        }

        public static string ReadFile(String fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                return reader.ReadToEnd();
            }
        }

        public static List<GameResult> ReadSoccerResults(string fileName)
        {
            var soccerResults = new List<GameResult>();
            using (var reader = new StreamReader(fileName))
            {
                string line = "";
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var gameResult = new GameResult();
                    string[] values = line.Split(',');
                    DateTime gameDate;
                    if (DateTime.TryParse(values[0], out gameDate))
                    {
                        gameResult.GameDate = gameDate;
                    }
                    gameResult.TeamName = values[1];
                    HomeOrAway homeOrAway;
                    if (Enum.TryParse(values[2], out homeOrAway))
                    {
                        gameResult.HomeOrAway = homeOrAway; 
                    }
                    int parseInt;
                    if(int.TryParse(values[3], out parseInt))
                    {
                        gameResult.Goals = parseInt;
                    }
                    if (int.TryParse(values[4], out parseInt))
                    {
                        gameResult.GoalAttempts = parseInt;
                    }
                    if (int.TryParse(values[5], out parseInt))
                    {
                        gameResult.ShotsOnGoal = parseInt;
                    }
                    if (int.TryParse(values[6], out parseInt))
                    {
                        gameResult.ShotsOffGoal = parseInt;
                    }
                    double possessionPercent;
                    if(double.TryParse(values[7], out possessionPercent))
                    {
                        gameResult.PossessionPercent = possessionPercent;
                    }
                    soccerResults.Add(gameResult);
                }
            }
            return soccerResults;
        }

        public static List<Player> DeserializePlayers(string fileName)
        {
            var players = new List<Player>();
            var serializer = new JsonSerializer();
            using (var reader = new StreamReader(fileName))
            using (var jsonReader = new JsonTextReader(reader))
            {
                players = serializer.Deserialize<List<Player>>(jsonReader);
            }
            return players;
        }
        public static List<Player> GetTopTenPlayers(List<Player> players)
        {
            var topTenPlayers = new List<Player>();
            players.Sort(new PlayerComparer());
            int counter = 0;
            foreach(var player in players)
            {
                topTenPlayers.Add(player);
                counter++;
                if(counter == 10)
                {
                    break;
                }

            }
            return players;
        }

        public static void SerializePlayersToFile(List<Player> players, string fileName)
        {
            var serializer = new JsonSerializer();
            using (var writer = new StreamWriter(fileName))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                serializer.Serialize(jsonWriter, players);
            }
        }

        public static string GetGoogleHomePage()
        {
            var webClient = new WebClient();
            byte[] googleHome = webClient.DownloadData("https://www.google.com");

            using (var stream = new MemoryStream(googleHome))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static List<NewsResult> GetNewsForPlayer(string playerName)
        {
            var results = new List<NewsResult>();
            var webClient = new WebClient();
            webClient.Headers.Add("Ocp-Apim-Subscription-Key", "95b3a4440f5342be801879983de9dadc");
            byte[] searchResults = webClient.DownloadData(string.Format("https://api.cognitive.microsoft.com/bing/v5.0/news/search?q={0}&mkt=en-us", playerName));
            var serializer = new JsonSerializer();  
            using (var stream = new MemoryStream(searchResults))
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                try
                {
                    results = serializer.Deserialize<NewsSearch>(jsonReader).NewsResults;
                }
                catch(NullReferenceException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                //results = serializer.Deserialize<NewsSearch>(jsonReader).NewsResults;
                //if(results == null)
                //{
                //    continue;
                //}
            }
            return results;
        }
    }
}
