using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using weatherApp.Models;

namespace weatherApp.Controllers.Models
{
    public class WeatherResource
    {
        public List<WeatherInfo> Result { get; set; }
        public int TotalFound { get; set; }
    }
}
