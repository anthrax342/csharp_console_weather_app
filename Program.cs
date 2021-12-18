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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Please enter a city: ");
            string city = Console.ReadLine();
            HttpClient client = new HttpClient();
            string requesturl = "https://api.openweathermap.org/data/2.5/weather?q=" + city + "&appid=7acf7e9cd7f56789fe7911d6576444c8&units=metric";
            HttpResponseMessage httpResponse = client.GetAsync(requesturl).Result;
            if(client.GetAsync(requesturl).Result.IsSuccessStatusCode == false)
            {
                Console.Clear();
                Console.WriteLine("Input error, please try again ");
                Main(args);
            }
            else
            {
                string response = httpResponse.Content.ReadAsStringAsync().Result;
                WeatherMapResponse weatherMapResponse = JsonConvert.DeserializeObject<WeatherMapResponse>(response);
                Console.Clear();
                Console.WriteLine("In " + weatherMapResponse.Name + " " + "`" + weatherMapResponse.Sys.Country + "`" + " it´s " + 
                weatherMapResponse.Main.Temp + "°C; " + "feels like " + weatherMapResponse.Main.Feels_like + 
                "°C; " + "the sea level is " + weatherMapResponse.Main.Sea_level + "m \n");
                Console.WriteLine("Weather condition: " + weatherMapResponse.Weather[0].Description);
                if (weatherMapResponse.Main.Temp >= 30)
                {
                    Console.WriteLine("\nNice and warm(: ");
                }
                if (weatherMapResponse.Main.Temp <= 0)
                {
                    Console.WriteLine("\nIt's pretty cold  (: ");
                }
            }
            Console.WriteLine("\nPress E to exit; R to repeat. ");
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
        private Sys sys;
        private string name;
        private List<Weather> weather;
        public Main Main
        {
            get { return main; }
            set { main = value; }
        }
        public Sys Sys
        {
            get { return sys; }
            set { sys = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public List<Weather> Weather
        {
            get { return weather; }
            set { weather = value; }
        }
    }
    class Main
    {
        private float temp;
        private float feels_like;
        private float sea_level;
        public float Temp
        {
            get { return temp; }
            set { temp = value; }
            

            }
        public float Feels_like
        {
            get { return feels_like; }
            set { feels_like = value; }
        }
        public float Sea_level
        {
            get { return sea_level; }
            set { sea_level = value; }
        }
    }
    class Sys
    {
        private string country;
        public string Country
        {
            get { return country; }
            set { country = value; }
        }
    }
    class Weather
    {
        private string description;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
    }
}
