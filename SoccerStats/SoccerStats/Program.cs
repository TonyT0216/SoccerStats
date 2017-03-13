using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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

        public static List<string[]> ReadSoccerResults(string fileName)
        {
            var soccerResults = new List<string[]>();
            using (var reader = new StreamReader(fileName))
            {
                string line = "";
                while((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');
                    soccerResults.Add(values);
                }
            }
            return soccerResults;
        }
    }
}
