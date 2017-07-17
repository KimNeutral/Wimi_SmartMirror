using System.Collections.Generic;

namespace DSWeather
{
    public class SKWeather // 4
    {
        public Weather weather { get; set; }

        public class Weather // 3
        {
            public List<Minutely> minutely { get; set; } //2
        }

        public class Temperature
        {
            public string tc { get; set; }
        }

        public class Sky
        {
            public string name { get; set; }
        }

        public class Station
        {
            public string name { get; set; }
        }

        public class Minutely // 1
        {
            public string timeObservation { get; set; }
            public Station station { get; set; }
            public Sky sky { get; set; }
            public Temperature temperature { get; set; }
        }

    }
}
