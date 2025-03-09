using Godot;
using System;
using System.Collections.Generic;

namespace Com.IsartDigital.WoolyWay.Utils
{
    public class MapData
    {
        public Dictionary<string,Dictionary<string, MapInfo>> Worlds { get; set; }
    }

    public class MapInfo
    {   
        public string Author { get; set; }
        public int Par { get; set; }
        public List<string> SheepDirection { get; set; }
        public List<string> DogDirection { get; set; }
        public List<string> Map { get; set; }
    }
}
