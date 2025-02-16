using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace Com.IsartDigital.ProjectName
{
    public class MapData
    {
        public MapInfo tutorial { get; set; }
        public MapInfo level1 { get; set; }
        public MapInfo level2 { get; set; }
        public MapInfo level3 { get; set; }
        public MapInfo level4 { get; set; }
        public MapInfo level5 { get; set; }
        public MapInfo level6 { get; set; }
    }

    public class MapInfo
    {
        public string author { get; set; }
        public int par { get; set; }
        public List<string> sheepDirection { get; set; }
        public List<string> map { get; set; }
    }
}
