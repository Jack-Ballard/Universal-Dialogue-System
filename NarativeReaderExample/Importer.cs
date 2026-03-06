using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarativeReaderExample
{
    public static class Importer
    {
        public static void ImportFile()
        {
            string filePath = @"C:\Users\Jack\source\repos\Honours Stage Project\Honours Stage Project\bin\Debug\exported_data.json";
            string input = File.ReadAllText(filePath);
            Object deserializedData = Newtonsoft.Json.JsonConvert.DeserializeObject(input);
            //Console.WriteLine(deserializedData);
            
            List<Object> textBoxes = ((Newtonsoft.Json.Linq.JObject)deserializedData)["TextBoxes"].ToObject<List<Object>>();
            //Console.WriteLine(textBoxes[0].ToString());
            foreach (Object textBox in textBoxes)
            {
                Globals.textBoxes.Add(new TextBoxes(textBox));
            }

            List<Object> connections = ((Newtonsoft.Json.Linq.JObject)deserializedData)["Connections"].ToObject<List<Object>>();
            foreach (Object connection in connections)
            {
                int fromTextBoxID = ((Newtonsoft.Json.Linq.JObject)connection)["FromTextBoxID"].ToObject<int>();
                int fromComponentID = ((Newtonsoft.Json.Linq.JObject)connection)["FromComponentID"].ToObject<int>();
                int toTextBoxID = ((Newtonsoft.Json.Linq.JObject)connection)["ToTextBoxID"].ToObject<int>();
                Globals.connections.Add((fromTextBoxID, fromComponentID, toTextBoxID));
            }
        }
    }
}
