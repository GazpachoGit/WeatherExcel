using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace weatherApp.Models
{
    public class WeatherInfo
    {
        public int ID { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Temperature { get; set; }
        public string Humidity { get; set; }
        public string DewPoint { get; set; }
        public string WindDirection { get; set; }
        public string Pressure { get; set; }
        public string WindSpeed { get; set; }
        public string Cloudy { get; set; }
        public string CloudyLowBorder { get; set; }
        public string Visibility { get; set; }
        public string WeatherPhenomenon { get; set; }

    }
}
