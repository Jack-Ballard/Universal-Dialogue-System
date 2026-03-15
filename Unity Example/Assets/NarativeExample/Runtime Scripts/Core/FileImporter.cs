using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Importer
{
    public static void ImportFile()
    {
        string filePath = Application.streamingAssetsPath + "/exported_data.json";
        string input = File.ReadAllText(filePath);
        System.Object deserializedData = Newtonsoft.Json.JsonConvert.DeserializeObject(input);
        //Console.WriteLine(deserializedData);

        List<System.Object> textBoxes = ((Newtonsoft.Json.Linq.JObject)deserializedData)["TextBoxes"].ToObject<List<System.Object>>();
        //Console.WriteLine(textBoxes[0].ToString());
        foreach (System.Object textBox in textBoxes)
        {
            Globals.textBoxes.Add(new TextBoxes(textBox));
        }

        List<System.Object> connections = ((Newtonsoft.Json.Linq.JObject)deserializedData)["Connections"].ToObject<List<System.Object>>();
        foreach (System.Object connection in connections)
        {
            int fromTextBoxID = ((Newtonsoft.Json.Linq.JObject)connection)["FromTextBoxID"].ToObject<int>();
            int fromComponentID = ((Newtonsoft.Json.Linq.JObject)connection)["FromComponentID"].ToObject<int>();
            int toTextBoxID = ((Newtonsoft.Json.Linq.JObject)connection)["ToTextBoxID"].ToObject<int>();
            Globals.connections.Add((fromTextBoxID, fromComponentID, toTextBoxID));
        }
    }
}