using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WetterApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string requestUrl = "http://api.openweathermap.org/data/2.5/weather?";

        public MainWindow()
        {
            InitializeComponent();
        }

        // updates UI with given city corresponding to weather data
        public void UpdateUI(string city)
        {
            string finalImage = "sun.png";

            WeatherMapResponse result = GetWeatherData(city);
            if (result.weather != null)
            {
                string currentWeather = result.weather[0].main.ToLower();
                if (currentWeather.Contains("cloud"))
                {
                    finalImage = "cloud.png";
                }
                else if (currentWeather.Contains("snow"))
                {
                    finalImage = "snow.png";
                }
                else if (currentWeather.Contains("rain"))
                {
                    finalImage = "rain.png";
                }

                backgroundImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/" + finalImage));
                labelTemperatur.Content = result.main.temp.ToString("F1") + "°C";
                labelInfo.Content = result.weather[0].main;
            }
            else { textboxQuery.Text="Stadt nicht gefunden"; }
        }

        //Api request to get weather data for given city
        public WeatherMapResponse GetWeatherData(string city)
        {
            //try to retrieve api key from environment
            string apiKey = Environment.GetEnvironmentVariable("OpenWeatherMapApiKey");

            if (apiKey == null) 
            { 
               MessageBox.Show("API Key nicht in umgebungsvariablen gefunden."); 
            }

            HttpClient httpClient = new HttpClient();

            //builds URI with querry and apikey
            var finalUri = requestUrl + "q=" + city + "&appid=" + apiKey + "&units=metric";

            HttpResponseMessage httpResponse = httpClient.GetAsync(finalUri).Result;

            string response = httpResponse.Content.ReadAsStringAsync().Result;

            // converts JSON response to weather object
            WeatherMapResponse weatherMapResponse = JsonConvert.DeserializeObject<WeatherMapResponse>(response);

            return weatherMapResponse;
        }

        //Calls UpdateUI method with querried city
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = textboxQuery.Text;
            UpdateUI(query);
        }

        //clears textbox on click
        private void TextboxQuery_GotFocus(object sender, RoutedEventArgs e)
        {
            textboxQuery.Clear();
        }
    }
}
