using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using WeatherApp.Model;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WeatherApp.ViewModel
{

    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        //properties to define whether to use gps or manual location
        public string Location = "";
        public bool UseGps = true;



        /// <summary>
        /// The <see cref="Current" /> property's name.
        /// </summary>
        public const string CurrentPropertyName = "Current";

        private CurrentView _current = null;

        /// <summary>
        /// Sets and gets the Current property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public CurrentView Current
        {
            get
            {
                return _current;
            }

            set
            {
                if (_current == value)
                {
                    return;
                }

                _current = value;
                RaisePropertyChanged(CurrentPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="City" /> property's name.
        /// </summary>
        public const string CityPropertyName = "City";

        private string _city = null;

        /// <summary>
        /// Sets and gets the City property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string City
        {
            get
            {
                return _city;
            }

            set
            {
                if (_city == value)
                {
                    return;
                }

                _city = value;
                RaisePropertyChanged(CityPropertyName);
            }
        }


        /// <summary>
        /// List of Forecast objects, containig only the information we need 
        /// from the corresponding objects
        /// The <see cref="Forecast" /> property's name.
        /// </summary>
        public const string ForecastPropertyName = "Forecast";

        private ObservableCollection<ForecastView> _forecast = null;

        /// <summary>
        /// Sets and gets the Forecast property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<ForecastView> Forecast
        {
            get
            {
                return _forecast;
            }

            set
            {
                if (_forecast == value)
                {
                    return;
                }

                _forecast = value;
                RaisePropertyChanged(ForecastPropertyName);
            }
        }





        
        //async function for updating weather data
        public async void Update(string location = "", bool useGps = true)
            //when using GPS, set location to empty string
        {
            //показваме зареждащ чеп
            StatusBarProgressIndicator progressbar = StatusBar.GetForCurrentView().ProgressIndicator;
            progressbar.Text = "Loading...";
            await progressbar.ShowAsync();

            //koordinatite
            string latitude = null, longitude = null;
            bool downloadSuccess = true;

            if (useGps)
            {
                try
                {
                    Geolocator geoLocator = new Geolocator();
                    geoLocator.DesiredAccuracyInMeters = 500;

                    Geoposition position =
                        await geoLocator.GetGeopositionAsync(
                        TimeSpan.FromMinutes(2),     //how often to refresh GPS coordinates
                        TimeSpan.FromMinutes(11));

                    latitude = position.Coordinate.Point.Position.Latitude.ToString("0.00000000");
                    longitude = position.Coordinate.Point.Position.Longitude.ToString("0.0000000");
                }

                catch (Exception)
                {
                    downloadSuccess = false;
                }

                if (downloadSuccess == false)
                {
                    await progressbar.HideAsync();
                    var dialog = new MessageDialog("Error: Cannot get location!\nCheck Settings!");
                    await dialog.ShowAsync();
                    return;
                }
            }
            

            //Processing the JSON
            downloadSuccess = true; //variable for handling exceprions

            string jsonFor = null; //forecast data
            string jsonCur = null; //current weather data

            // Variables containig the URL adresses sent to OpenWeatherMap
            string urlFor = "http://api.openweathermap.org/data/2.5/" +
                "forecast/daily?" +
                (useGps ? ("lat=" + latitude + "&lon=" + longitude) : ("q="+location)) + 
                "&cnt=7" +
                "&mode=json" +
                "&units=metric" +
                "&APPID=c119243972f7963d7d0a2e44c1a7fdfb";

            string urlCur = "http://api.openweathermap.org/data/2.5/" +
                "weather?" +
                (useGps ? ("lat=" + latitude + "&lon=" + longitude) : ("q=" + location)) +
                "&mode=json" +
                "&units=metric" +
                "&APPID=c119243972f7963d7d0a2e44c1a7fdfb";

            //getting the data
            try
            {
                using (var httpClient = new HttpClient())
                {
                    jsonFor = await httpClient.GetStringAsync(urlFor);
                    jsonCur = await httpClient.GetStringAsync(urlCur);
                }
            }

            catch (Exception)
            {
                downloadSuccess = false;
            }

            //show error message
            if (downloadSuccess == false) //if there is an error while getting data, for ex. when data connection is not enabled
            {
                await progressbar.HideAsync(); //hide the progress bar
                var dialog = new MessageDialog("Error while connecting to web service.\nCheck data connection!");
                await dialog.ShowAsync(); //show message
                return;
            }

            try
            {
                //convert json strings
                WeatherData apiForecast = JsonConvert.DeserializeObject<WeatherData>(jsonFor);
                CurrentWeatherData apiCurrent = JsonConvert.DeserializeObject<CurrentWeatherData>(jsonCur);
                //setting binded properties
                //current city name
                City = apiForecast.city.name + 
                    (apiForecast.city.name == "" ? "" : ", ") + 
                    apiForecast.city.country;
                Current = new CurrentView(apiCurrent);
                ObservableCollection<Day> dayList = new ObservableCollection<Day>(apiForecast.list);
                Forecast = new ObservableCollection<ForecastView>();
                for (int i = 0; i < dayList.Count; i++)
                {
                    Forecast.Add(new ForecastView(dayList[i]));
                }
            }
            catch (Exception)
            {
                downloadSuccess = false;
            }

            //show error message for when json is not properly converted
            if (downloadSuccess == false)
            {
                await progressbar.HideAsync();
                var dialog = new MessageDialog("Can't get data for \"" + location + "\".\nCheck spelling!");
                await dialog.ShowAsync();
                return;
            }
            

            //hide the progressbar
            await progressbar.HideAsync();
        }




        private RelayCommand _refreshCommand;

        /// <summary>
        /// call the update function when the refresh button is pressed
        /// Gets the RefreshCommand.
        /// </summary>
        public RelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand
                    ?? (_refreshCommand = new RelayCommand(
                    () =>
                    {
                        Update(Location, UseGps);
                    }));
            }
        }



        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // create design time data
                
                // set default day data
                List<Weather> ww = new List<Weather>();
                ww.Add (new Weather{icon="01d.png"});
                Day day = new Day
                {
                    temp = new Temp { day = 14, eve = 9, min = 8, max = 20, morn = 10, night = 11 },
                    Time = DateTime.Now,
                    weather = ww 
                };
             
                //makee a forecast list with default data
                Forecast = new ObservableCollection<ForecastView>();
                for (int i = 1; i < 8; i++) {
                    Forecast.Add(new ForecastView(day));
                }

                //default data for current view
                Current = new CurrentView(); 
            }

            else
            {
                //when first loading the app
                Update(Location, UseGps);
               
                // Registers for incoming Notification messages from LocationPage.
                Messenger.Default.Register<NotificationMessage>(this, (message) =>
                {
                    Location = message.Notification;
                    UseGps = (message.Notification.Length == 0);
                    Update(Location, UseGps);
                });

            }
        }
    }
}