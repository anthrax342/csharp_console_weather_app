using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace wetter_app_neu_console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bitte Trage eine Stadt ein:");
            string city = Console.ReadLine();
            HttpClient client = new HttpClient();
            string requesturl = "https://api.openweathermap.org/data/2.5/weather?q=" + city + "&appid=7acf7e9cd7f56789fe7911d6576444c8&units=metric";
            HttpResponseMessage httpResponse = client.GetAsync(requesturl).Result;
            if(client.GetAsync(requesturl).Result.IsSuccessStatusCode == false)
            {
                Console.Clear();
                Console.WriteLine("Eingabefehler, bitte versuchen Sie es erneut");
                Main(args);
            }
            else
            {
                string response = httpResponse.Content.ReadAsStringAsync().Result;
                WeatherMapResponse weatherMapResponse = JsonConvert.DeserializeObject<WeatherMapResponse>(response);
                Console.WriteLine("In " + city + " ist es " + weatherMapResponse.Main.Temp + "°C");
            }
            Console.WriteLine("\nE drücken zum verlassen, oder R zum wiederholen.");
            ConsoleKey key;
            do
            {
                key = Console.ReadKey(true).Key;
                if(key == ConsoleKey.E)
                {
                    Environment.Exit(0);
                }
                else if (key == ConsoleKey.R)
                    {
                        Console.Clear();
                        Main(args);
                    }
            } while (true);
        }
    }
    class WeatherMapResponse
    {
        private Main main;
        public Main Main
        {
            get { return main; }
            set
            { main = value; }
        }
    }
    class Main
    {
        private float temp;
        public float Temp
        {
            get { return temp; }
            set { 
                    temp = value; 
                    if(temp >= 35)
                    {
                        Console.WriteLine("\nGanz schön warm \n");
                    }
                    if(temp <= 0)
                    {
                    Console.WriteLine("\nGanz schön kalt \n");
                    }
                }
        }
    }
}
