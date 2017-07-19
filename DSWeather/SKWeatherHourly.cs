using System.Collections.Generic;

namespace DSWeather
{
    public class SKWeatherHourly // 4
    {
        public Weather weather { get; set; }

        public class Weather // 3
        {
            public List<Hourly> hourly { get; set; } //2
        }

        public class Temperature
        {
            public string tc { get; set; }
        }

        public class Sky
        {
            public string name { get; set; }
            public string code { get; set; }
        }

        public class Station
        {
            public string name { get; set; }
        }

        public class Grid
        {
            public string village { get; set; }
        }

        public class Hourly // 1
        {
            public string timeObservation { get; set; }
            public Station station { get; set; }
            public Sky sky { get; set; }
            public Temperature temperature { get; set; }
            public string humidity { get; set; }
            public Grid grid { get; set; }
        }

    }
}
