using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Spectre.Console;

namespace wetter_app_neu_console
{
    internal class Program
    {
        public static List<string> cities = new List<string>();
        //Main
        static void Main(string[] args)
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Weather App by anthrax3 | c -2.6";

            //Christmas date verification
            DateTime from = new DateTime(DateTime.Now.Year,12,24);
            DateTime to = new DateTime(DateTime.Now.Year, 12, 26);
            DateTime input = DateTime.Now;
            if (from <= input & input <= to)
            {
                Console.WindowHeight = 42;
                var image = new CanvasImage("logo-c-sharp-xmas.png");
                image.MaxWidth(16);
                AnsiConsole.Write(image);
                var font = FigletFont.Load("3D-ASCII.flf");
                AnsiConsole.Write(new FigletText(font, "Merry Xmas").LeftAligned().Color(Color.Red));
            }
            else
            {
                Console.WindowHeight = 30;
                var image = new CanvasImage("logo-c-sharp.png");
                image.MaxWidth(16);
                AnsiConsole.Write(image);
                var font = FigletFont.Load("3D-ASCII.flf");
                AnsiConsole.Write(new FigletText(font, "c -2.6").LeftAligned().Color(Color.Red));
            }

            //Date display
            var culture = new System.Globalization.CultureInfo("en-US");
            var panel = new Panel(culture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek) + " / " + System.DateTime.Now.ToString("MM.dd.yyyy"));

            //Panel creation with Spectre.Console
            panel.Header("[italic blue] date (en-US): [/]");
            panel.Header.Centered();
            panel.BorderColor(Color.Red);
            panel.Border = BoxBorder.Rounded;
            AnsiConsole.Write(panel);

            //Status display of the running process
            AnsiConsole.Status()
                .Start("loading...", ctx =>
                {
                    ctx.Spinner(Spinner.Known.Shark);

                    //Connection test to the ´owm´ server
                    var request = (HttpWebRequest)WebRequest.Create("https://openweathermap.org/");
                    request.UserAgent = "client";
                    request.KeepAlive = false;
                    request.Timeout = 5000;

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.ContentLength == 0 && response.StatusCode == HttpStatusCode.NoContent)
                        {
                            AnsiConsole.Markup("[underline red]Connection to the owm servers unsuccessful![/]\n");
                        }
                        else
                        {
                            AnsiConsole.Markup("[italic blue]connection to the ´owm´ servers was successful![/]\n");
                            Console.Title = Console.Title = "Weather App by anthrax3 | c -2.6 -- Server Status: " + Convert.ToString(response.StatusCode); 
                        }
                    }

                });

            //command prompt
            AnsiConsole.Markup("[rapidblink blue]Please enter a city or a postal code[/][rapidblink]:[/] \n");
            Console.ForegroundColor = ConsoleColor.Green;
            string city = Console.ReadLine();

            //Http client generation and api query
            HttpClient client = new HttpClient();
            string requesturl = "https://api.openweathermap.org/data/2.5/weather?q=" + city + "&appid=7acf7e9cd7f56789fe7911d6576444c8&units=metric";
            HttpResponseMessage httpResponse = client.GetAsync(requesturl).Result;

            //result success status
            if (client.GetAsync(requesturl).Result.IsSuccessStatusCode == false)
            {
                System.Media.SystemSounds.Beep.Play();
                Console.Clear();
                AnsiConsole.Markup("[underline red]Input error! Please try again[/]\n");
                Main(args);
            }
            else
            {
                Console.WindowHeight = 40;
                //Conversion of the raw data (Json) into usable results
                string response = httpResponse.Content.ReadAsStringAsync().Result;
                WeatherMapResponse weatherMapResponse = JsonConvert.DeserializeObject<WeatherMapResponse>(response);

                //Conversion from metric to imperial (optional information)
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                double fh, fh1;
                fh = weatherMapResponse.Main.Temp * 9 / 5 + 32;
                fh1 = weatherMapResponse.Main.Feels_like * 9 / 5 + 32;

                //Table creation with Spectre.Console
                var table = new Table();
                table.Border = TableBorder.Rounded;
                AnsiConsole.Markup("[italic blue]For [/]");
                Console.WriteLine(weatherMapResponse.Name);

                string prompt_time = DateTime.Now.ToLongTimeString();
                cities.Add(weatherMapResponse.Name + " | " + prompt_time);

                    //IP address of the server to be connected
                    try
                    {
                        IPHostEntry hostname = Dns.GetHostEntry("api.openweathermap.org");
                        IPAddress[] ip = hostname.AddressList;
                        Console.WriteLine("connected to " + ip[0]);
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.WriteException(ex);
                    }
                table.AddColumn("Country");
                table.AddColumn("Temp in °C");
                table.AddColumn("Temp in °F");
                table.AddColumn("Felt temperature in °C");
                table.AddColumn("Felt temperature in °F");
                table.AddColumn("Sea level in m");
                table.AddColumn("Wind speed in m/s");
                table.AddColumn("Weather condition");

                //Conversion of the data in string for display in the table
                string c_id = weatherMapResponse.Sys.Country;
                string temp_c = Convert.ToString(weatherMapResponse.Main.Temp);
                string felt_c = Convert.ToString(weatherMapResponse.Main.Feels_like);
                string sea_m = Convert.ToString(weatherMapResponse.Main.Sea_level);
                string wc = Convert.ToString(weatherMapResponse.Weather[0].Description);
                string ws = Convert.ToString(weatherMapResponse.Wind.Speed);
                table.AddRow(c_id, temp_c, Convert.ToString(Math.Round(fh, 2)), felt_c, Convert.ToString(Math.Round(fh1, 2)), sea_m, ws, wc);
                AnsiConsole.Write(table);

                //output of the weather icon
                var iconcode = weatherMapResponse.Weather[0].Icon;
                WebClient wclient = new WebClient();
                wclient.DownloadFile("http://openweathermap.org/img/w/" + iconcode + ".png", iconcode + ".png");
                var image = new CanvasImage(iconcode + ".png");
                image.MaxWidth(16);
                AnsiConsole.Write(image);
                File.Delete(iconcode + ".png");


                //bar chart for the minimum, current and maximum temperature + Conversion of sub-zero temperatures into plus degrees for output
                if (weatherMapResponse.Main.Temp_min < 0)
                    {
                        weatherMapResponse.Main.Temp_min = -weatherMapResponse.Main.Temp_min;
                        AnsiConsole.Markup("[italic]the[/] [italic blue]minimun[/] [italic]is minus[/]\n");
                    }
                if (weatherMapResponse.Main.Temp_max < 0)
                {
                    weatherMapResponse.Main.Temp_max = -weatherMapResponse.Main.Temp_max;
                    AnsiConsole.Markup("[italic]the[/] [italic red]maximum[/] [italic]is minus[/]\n");
                }
                if (weatherMapResponse.Main.Temp < 0)
                {
                    weatherMapResponse.Main.Temp = -weatherMapResponse.Main.Temp;
                    AnsiConsole.Markup("[italic]the[/] [italic green]current[/] [italic]is minus[/]\n");
                }
                Console.WriteLine();
                    AnsiConsole.Write(new BarChart()
                       .Width(60)
                       .Label("[blue bold]minimum[/], [green bold]current[/] and [red bold]maximum[/] temperature in °C:\n")
                       .CenterLabel()
                       .AddItem("[blue bold]minimum[/]:", Math.Round(weatherMapResponse.Main.Temp_min, 2), Color.Blue)
                       .AddItem("[green bold]current[/]:", Math.Round(weatherMapResponse.Main.Temp, 2), Color.Green)
                       .AddItem("[red bold]maximum[/]:", Math.Round(weatherMapResponse.Main.Temp_max, 2), Color.Red));
            }
            Console.WriteLine();
            var line = new Rule();
            AnsiConsole.Write(line);

            //Key query
            Console.WriteLine("\nPress E to exit; R to repeat; C to show cities that have been entered. ");
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
                else if (key == ConsoleKey.C)
                    {
                        Console.Clear();
                        Console.WindowHeight = 20;

                        foreach (string s in cities)
                            {
                                Console.WriteLine(s);
                            }
                        Console.WriteLine("\nPress E to exit; R to repeat; C to clear the list and repeat.");

                        do
                        {
                            key = Console.ReadKey(true).Key;
                        if (key == ConsoleKey.E)
                        {
                            Environment.Exit(0);
                        }
                        else if (key == ConsoleKey.R)
                        {
                            Console.Clear();
                            Main(args);
                        }
                        else if (key == ConsoleKey.C)
                            {
                                cities.Clear();
                                Console.Clear();
                                Main(args);
                            }
                    } while (true);
                    }
            } while (true);
        }
    }

    //Classes with properties
    class WeatherMapResponse
    //Query class with city name
    {
        private Main main;
        private Sys sys;
        private string name;
        private List<Weather> weather;
        private Wind wind;
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
        public Wind Wind
        {
            get { return wind; }
            set { wind = value; }
        }
    }
    class Main
    //Class for temperature, perceived temperature, sea level, minimum and maximum temperature
    {
        private float temp;
        private float feels_like;
        private float sea_level;
        private float temp_min;
        private float temp_max;
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
        public float Temp_min
        {
            get { return temp_min; }
            set { temp_min = value; }
        }
        public float Temp_max
        {
            get { return temp_max; }
            set { temp_max = value; }
        }
    }
    class Sys
    //Class for the country
    {
        private string country;
        public string Country
        {
            get { return country; }
            set { country = value; }
        }
    }
    class Weather
    //Class for weather description and iconid
    {
        private string description;
        private string icon;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public string Icon
        {
            get { return icon; }
            set { icon = value; }
        }
    }
    class Wind
    //class for wind speed
    {
        private float speed;

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }
    }
}