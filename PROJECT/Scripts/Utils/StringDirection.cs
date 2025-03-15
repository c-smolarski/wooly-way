using Com.IsartDigital.WoolyWay.Utils.TwoWayDictionnaries;
using Godot;
using System;
using System.Collections.Generic;

namespace Com.IsartDigital.WoolyWay.Utils
{
    public static class StringDirection
    {
        private const string LEFT = "Left";
        private const string RIGHT = "Right";
        private const string UP = "Up";
        private const string DOWN = "Down";

        private static readonly TwoWayDictionary<string, Vector2I> Directions = new TwoWayDictionary<string, Vector2I>{
            {LEFT, Vector2I.Left},
            {RIGHT, Vector2I.Right},
            {UP, Vector2I.Up},
            {DOWN, Vector2I.Down},
        };

        public static Vector2I GetDirection(string pDirection)
        {
            return Directions.Contains(pDirection) ? Directions[pDirection] : Vector2I.Zero;
        }
        
        
        public static string GetDirectionString(Vector2I pDirection)
        {
            return Directions.Contains(pDirection) ? Directions[pDirection].ToString() : "";
        }
    }
}
