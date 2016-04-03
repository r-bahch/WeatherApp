using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Model
{
    public class CurrentView //contains only the data we need to show for current weather
    {
        public string Temperature { get; set; }
        public string Pic { get; set; }
        public string Background { get; set; }
        public string Description { get; set; }
        public string Humidity { get; set; }
        public string Winds { get; set; }
        public string Pressure { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }

        public CurrentView()
        {
            Temperature = "23°C";
            Pic = "Assets/pics/01d.png";
            Background = "Assets/backgrounds/b01d.jpg";
            Description = "sunny";
            Humidity = "78%";
            Winds = "↙ 8 m/s";
            Pressure = "928 hPa";
            Sunrise = "05:27";
            Sunset = "20:05";
        }

        public CurrentView(CurrentWeatherData c)
        {
            Temperature = c.main.temp.ToString("F0") + "°C";
            Pic = "Assets/pics/" + c.weather[0].icon + ".png";
            Background = "Assets/backgrounds/b" + c.weather[0].icon + ".jpg";
            Description = c.weather[0].description;
            Humidity = c.main.humidity.ToString("F0") + "%";
            Winds = windDirection(c.wind.deg) + " " + c.wind.speed.ToString("F1") + " m/s";
            Pressure = c.main.pressure.ToString("F0") + " hPa";
            DateTime suntmp = UnixTimeStampToDateTime(c.sys.sunrise);
            Sunrise = suntmp.Hour + ":" + suntmp.Minute.ToString("D2");
            DateTime sunstmp = UnixTimeStampToDateTime(c.sys.sunset);
            Sunset = sunstmp.Hour + ":" + sunstmp.Minute.ToString("D2");

        }

        //unix to time converter
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        //degrees to arrow converter for wind direction
        private string windDirection(double deg)
        {
            if (deg >= 0 && deg < 22.5) return "↓";
            else if (deg < 67.5) return "↙";
            else if (deg < 112.5) return "←";
            else if (deg < 157.5) return "↖";
            else if (deg < 202.5) return "↑";
            else if (deg < 247.5) return "↗";
            else if (deg < 292.5) return "→";
            else if (deg < 337.5) return "↘";
            else return "↓";
        }
    }
}
