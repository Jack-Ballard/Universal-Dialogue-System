using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarativeReaderExample
{
    public class TextBoxes
    {
        public int ID { get; set; }
        public string TextContent { get; set; }
        public List<Connection> Connections { get; set; }
        public TextBoxes(Object serializedData)
        {
            ID = ((Newtonsoft.Json.Linq.JObject)serializedData)["ID"].ToObject<int>();
            TextContent = ((Newtonsoft.Json.Linq.JObject)serializedData)["TextContent"].ToObject<string>();
            List<Object> connections = ((Newtonsoft.Json.Linq.JObject)serializedData)["Connections"].ToObject<List<Object>>();

            Connections = new List<Connection>();
            foreach (Object connection in connections)
            {
                Connections.Add(new Connection(connection));

            }
        }
    }

    public class Connection
    {
        public int ID { get; set; }
        public List<Attribute> Attributes { get; set; }
        public List<Object> Conditions { get; set; }
        public Connection(Object serializedData)
        {
            ID = ((Newtonsoft.Json.Linq.JObject)serializedData)["ID"].ToObject<int>();
            List<Object> attributes = ((Newtonsoft.Json.Linq.JObject)serializedData)["Attributes"].ToObject<List<Object>>();

            Attributes = new List<Attribute>();
            foreach (Object attribute in attributes)
            {
                Attributes.Add(new Attribute(attribute));
            }

            Conditions = new List<Object>();
            Conditions = ((Newtonsoft.Json.Linq.JObject)serializedData)["Conditions"].ToObject<List<Object>>();
        }
    }

    public class Attribute
    {
        public int ID { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public Attribute(Object serializedData)
        {
            ID = ((Newtonsoft.Json.Linq.JObject)serializedData)["ID"].ToObject<int>();
            Value = ((Newtonsoft.Json.Linq.JObject)serializedData)["Value"].ToObject<string>();
            Name = ((Newtonsoft.Json.Linq.JObject)serializedData)["Name"].ToObject<string>();
        }
    }
}
