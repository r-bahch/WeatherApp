using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Model
{
    public class WeatherData
    {
        public string cod { get; set; }
        public double message { get; set; }
        public City city { get; set; }
        public int cnt { get; set; }
        public List<Day> list { get; set; }
    }
}
