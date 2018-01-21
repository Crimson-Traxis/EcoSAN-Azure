using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcoSAN_Web.Models.Firebase
{
    public class Point
    {
        public string DeviceID { get; set; }
        public string DeviceName { get; set; }
        public int RecievedPoint { get; set; }
        public int TimeStamp { get;set; }
    }
}