using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Model
{
    public class ForecastView //contains only the data we need for each day in forecast view
    {
        public string DayOfWeek { get; set; }
        public string Date { get; set; }
        public string Temperature { get; set; }
        public string Pic { get; set; }
        public string todayTemp { get; set; }

        public ForecastView(Day d) {
            DayOfWeek = d.Time.DayOfWeek.ToString();
            Date = d.Time.Day.ToString("D2") + "." + d.Time.Month.ToString("D2");
            Temperature = d.temp.max.ToString("F0") + "°C/" + d.temp.min.ToString("F0") + "°C";
            todayTemp = "↑" + d.temp.max.ToString("F0") + "°C  ↓" + d.temp.min.ToString("F0") + "°C";
            Pic = "Assets/pics/" + d.weather[0].icon + ".png";
        }
    }
}
