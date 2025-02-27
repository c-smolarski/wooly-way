using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

using Com.IsartDigital.WoolyWay.Data;

// Author : Clément DUCROQUET

namespace Com.IsartDigital.WoolyWay.Utils.Data
{
	public static class DataTool
	{
		static public List<UserData> ListDeserializer(string[] pDataArray)
		{
			UserData lData;

			List<UserData> lDataList = new List<UserData>();

			for (int lIndex = 1; lIndex < pDataArray.Length - 1; lIndex++)
			{
				string str = pDataArray[lIndex].EndsWith(",") ? pDataArray[lIndex].Remove(pDataArray[lIndex].Length - 1) : pDataArray[lIndex];
				//var str = "{\"username\": \"user\", \"password\": \"pass\", \"salt\": \"s\"}";

				JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
				jsonOptions.IncludeFields = true;

				lData = JsonSerializer.Deserialize<UserData>(str, jsonOptions);
				lDataList.Add(lData);

				GD.Print(str + " ", lData.username, lData.hachedPassword, lData.passwordSalt);
			}

			return lDataList;
		}
	}
}
