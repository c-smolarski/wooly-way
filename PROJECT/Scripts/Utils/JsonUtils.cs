using Godot;
using Godot.Collections;
using System;
using System.IO;

namespace Com.IsartDigital.WoolyWay.Utils
{
    public static class JsonUtils
    {
        public static Dictionary dataDico = new Dictionary();
        public static Dictionary ReadJson(string pPathJson)
        {
            //creates an object Json to analyse data
            Json jsonRead = new Json();

            //stocks the data from the json folder in the variable
            string data = File.ReadAllText(pPathJson);

            //converts the data in an exploitable data
            Error error = jsonRead.Parse(data);

            // reads the data and converts it in a dictionary
            dataDico = (Dictionary)jsonRead.Data;

            return dataDico;
        }
    }
}