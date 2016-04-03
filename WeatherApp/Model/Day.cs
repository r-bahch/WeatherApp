using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Model
{

    //this is List from the json
    public class Day
    {
        private int _dt; //date time

        public int dt
        {
            get { return _dt; }
            set
            {
                _dt = value;
                //newly created Time propery is set
                var tmp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                Time = tmp.AddSeconds(value);
            }
        }

        //time property for convenience, because the returned dt is an integer
        private DateTime _time;

        public DateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }

        public Temp temp { get; set; }
        public double pressure { get; set; }
        public int humidity { get; set; }
        public List<Weather> weather { get; set; }
        public double speed { get; set; }
        public int deg { get; set; }
        public int clouds { get; set; }
        public double rain { get; set; }
    }
}
