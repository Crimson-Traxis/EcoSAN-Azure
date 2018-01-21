using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcoSAN_Web.Models
{
    public class TrashPickupModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int TimeStamp { get; set; }
        public string Image { get; set; }
        public string DeviceID { get; set; }
        public string DeviceName { get; set; }
        public List<string> ConnectedDevices { get; set; }

        public TrashPickupModel()
        {
            var randPos = new Random().Next(3);
            var idList = new[] { "1111", "1112", "1113", "1114" };
            var nameList = new[] { "Trent's Iphone", "Chipp's Iphone", "Nate's Iphone", "Anthony's Iphone" };
            var rand = new Random();
            Latitude = 42.727583 + (rand.NextDouble() - .5) / 1000;
            Longitude = -84.482184 + (rand.NextDouble() - .5) / 1000;
            TimeStamp = 0;
            Image = "";
            DeviceID = idList[randPos];
            DeviceName = nameList[randPos];
            ConnectedDevices = new List<string>();
        }
    }
}