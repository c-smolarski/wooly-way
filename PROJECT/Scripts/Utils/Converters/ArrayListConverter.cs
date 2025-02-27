using Godot;
using System;
using System.Collections.Generic;

// Author : Clément DUCROQUET

namespace Com.IsartDigital.WoolyWay.Utils.Converters
{
	public static class ArrayListConverter
	{
		static public List<string> StringArrayToStringList(string[] pStringArray)
		{
			List<string> lList = new List<string>();

            foreach (string lString in pStringArray) lList.Add(lString);

            return lList;
		}
	}
}
