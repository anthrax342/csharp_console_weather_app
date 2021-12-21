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
            DateTime from = new DateTime(DateTime.Now.Year,12,24);
            DateTime to = new DateTime(DateTime.Now.Year, 12, 26);
            DateTime input = DateTime.Now;
            if (from <= input & input <= to)
            {
                Console.WindowHeight = 42;
                Console.Title = "Weather App by anthrax3 | c -2.0 🎄";
                var image = new CanvasImage("logo-c-sharp-xmas.png");
                image.MaxWidth(16);
                AnsiConsole.Write(image);
                var font = FigletFont.Load("3D-ASCII.flf");
                AnsiConsole.Write(new FigletText(font, "Merry Xmas").LeftAligned().Color(Color.Red));
            }
            else
            {
                Console.WindowHeight = 30;
                Console.Title = "Weather App by anthrax3 | c -2.0";
                var image = new CanvasImage("logo-c-sharp.png");
                image.MaxWidth(16);
                AnsiConsole.Write(image);
                var font = FigletFont.Load("3D-ASCII.flf");
                AnsiConsole.Write(new FigletText(font, "c -2.0").LeftAligned().Color(Color.Red));
            }
            var culture = new System.Globalization.CultureInfo("en-US");
            var panel = new Panel(culture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek) + " / " + System.DateTime.Now.ToString("MM.dd.yyyy"));
            panel.Header("[italic blue] date (en-US): [/]");
            panel.Header.Centered();
            panel.BorderColor(Color.Red);
            panel.Border = BoxBorder.Rounded;
            AnsiConsole.Write(panel);
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
                var table = new Table();
                table.Border = TableBorder.Rounded;
                AnsiConsole.Markup("[italic blue]For [/]");
                Console.WriteLine(weatherMapResponse.Name);
                table.AddColumn("Country");
                table.AddColumn("Temp in °C");
                table.AddColumn("Temp in °F");
                table.AddColumn("Felt temperature in °C");
                table.AddColumn("Felt temperature in °F");
                table.AddColumn("Sea level in m");
                table.AddColumn("Sea level in ft");
                table.AddColumn("Weather condition");
                string c_id = weatherMapResponse.Sys.Country;
                string temp_c = Convert.ToString(weatherMapResponse.Main.Temp);
                string felt_c = Convert.ToString(weatherMapResponse.Main.Feels_like);
                string sea_m = Convert.ToString(weatherMapResponse.Main.Sea_level);
                string wc = Convert.ToString(weatherMapResponse.Weather[0].Description);
                table.AddRow(c_id, temp_c, Convert.ToString(Math.Round(fh, 2)), felt_c, Convert.ToString(Math.Round(fh1, 2)), sea_m, Convert.ToString(Math.Round(fo, 2)), wc);
                AnsiConsole.Write(table);
                if (weatherMapResponse.Main.Sea_level == 0)
                {
                    AnsiConsole.Markup("[bold blue]The sea level is a bit buggy, if the value is 0 it may be that the actual value is below zero or not available[/].\n");
                }
                if (weatherMapResponse.Main.Temp >= 30)
                {
                    var warm = "\nNice and warm " + Emoji.Known.Thermometer;
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
