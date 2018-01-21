using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcoSAN_Web.Models
{
    public class Status
    {
        public int code { get; set; }
        public string description { get; set; }
    }

    public class Status2
    {
        public int code { get; set; }
        public string description { get; set; }
    }

    public class OutputInfo
    {
        public string message { get; set; }
        public string type { get; set; }
    }

    public class Status3
    {
        public int code { get; set; }
        public string description { get; set; }
    }

    public class ModelVersion
    {
        public string id { get; set; }
        public DateTime created_at { get; set; }
        public Status3 status { get; set; }
    }

    public class Model
    {
        public string name { get; set; }
        public string id { get; set; }
        public DateTime created_at { get; set; }
        public object app_id { get; set; }
        public OutputInfo output_info { get; set; }
        public ModelVersion model_version { get; set; }
    }

    public class Image
    {
        public string url { get; set; }
    }

    public class Data
    {
        public Image image { get; set; }
    }

    public class Input
    {
        public string id { get; set; }
        public Data data { get; set; }
    }

    public class Concept
    {
        public string id { get; set; }
        public string name { get; set; }
        public object app_id { get; set; }
        public double value { get; set; }
    }

    public class Data2
    {
        public List<Concept> concepts { get; set; }
    }

    public class Output
    {
        public string id { get; set; }
        public Status2 status { get; set; }
        public DateTime created_at { get; set; }
        public Model model { get; set; }
        public Input input { get; set; }
        public Data2 data { get; set; }
    }

    public class RootObject
    {
        public Status status { get; set; }
        public List<Output> outputs { get; set; }
    }
}