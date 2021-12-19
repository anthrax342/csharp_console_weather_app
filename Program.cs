using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Spectre.Console;

namespace wetter_app_neu_console
{
    internal class Program
    {

        static void Main(string[] args)
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Weather App by anthrax3 | c -1.6";
            string title = @"
               _   __   
  ___        / | / /_  
 / __|  _____| || '_ \ 
| (__  |_____| || (_) |
 \___|       |_(_)___/      
                        ";
            Console.WriteLine(title);
            AnsiConsole.Markup("[rapidblink blue]Please enter a city[/][rapidblink]:[/] \n");
            Console.ForegroundColor = ConsoleColor.Green;
            string city = Console.ReadLine();
            HttpClient client = new HttpClient();
            string requesturl = "https://api.openweathermap.org/data/2.5/weather?q=" + city + "&appid=7acf7e9cd7f56789fe7911d6576444c8&units=metric";
            HttpResponseMessage httpResponse = client.GetAsync(requesturl).Result;
            if(client.GetAsync(requesturl).Result.IsSuccessStatusCode == false)
            {
                Console.Clear();
                AnsiConsole.Markup("[underline red]Input error, please try again[/]\n");
                Main(args);
            }
            else
            {
                string response = httpResponse.Content.ReadAsStringAsync().Result;
                WeatherMapResponse weatherMapResponse = JsonConvert.DeserializeObject<WeatherMapResponse>(response);
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                double fh, fh1, fo;
                fh = weatherMapResponse.Main.Temp * 9 / 5 + 32;
                fh1 = weatherMapResponse.Main.Feels_like * 9 / 5 + 32;
                fo = weatherMapResponse.Main.Sea_level * 3.2808398950131;
                Console.WriteLine("In " + weatherMapResponse.Name + " " + "`" + weatherMapResponse.Sys.Country + "`" + " it´s " + 
                weatherMapResponse.Main.Temp + "°C " + "[" + Math.Round(fh, 2) + "°F]" + "; " + "feels like " + weatherMapResponse.Main.Feels_like + 
                "°C " + "[" + Math.Round(fh1, 2) + "°F]" + "; "  + "the sea level is " + weatherMapResponse.Main.Sea_level + "m " + "[" + Math.Round(fo, 2) + "ft] \n");
                Console.WriteLine("Weather condition: " + weatherMapResponse.Weather[0].Description);
                if (weatherMapResponse.Main.Temp >= 30)
                {
                    var warm = "\nNice and warm " + Emoji.Known.Sun;
                    AnsiConsole.MarkupLine(warm);
                }
                if (weatherMapResponse.Main.Temp <= 0)
                {
                    var cold = "\nIt´s pretty cold " + Emoji.Known.Snowflake;
                    AnsiConsole.MarkupLine(cold);
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
