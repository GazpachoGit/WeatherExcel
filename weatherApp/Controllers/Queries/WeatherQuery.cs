using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace weatherApp.Controllers.Queries
{
    public class WeatherQuery
    {
        public int Page { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
    }
}
