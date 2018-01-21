using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcoSAN_Web.Models.Firebase
{
    public class Pickup
    {
        public string DeviceID { get; set; }
        public string DeviceName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int TimeStamp { get; set; }
    }
}