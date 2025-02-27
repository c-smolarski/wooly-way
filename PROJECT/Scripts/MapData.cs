using Godot;
using System;
using System.Collections.Generic;

namespace Com.IsartDigital.WoolyWay
{
    public class MapData
    {
        public MapInfo Tutorial { get; set; }
        public MapInfo Level1 { get; set; }
        public MapInfo Level2 { get; set; }
        public MapInfo Level3 { get; set; }
        public MapInfo Level4 { get; set; }
        public MapInfo Level5 { get; set; }
        public MapInfo Level6 { get; set; }
    }

    public class MapInfo
    {
        public string Author { get; set; }
        public int Par { get; set; }
        public List<string> SheepDirection { get; set; }
        public List<string> Map { get; set; }
    }
}
