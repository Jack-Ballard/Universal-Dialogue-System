using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;

public struct Connection
{
    public int FromTextBoxID { get; set; }
    public int FromComponentID { get; set; }
    public int FromConnectionID { get; set; }
    public int ToTextBoxID { get; set; }
    public Connection(Object serializedData)
    {
        FromTextBoxID = ((Newtonsoft.Json.Linq.JObject)serializedData)["FromTextBoxID"].ToObject<int>();
        FromComponentID = ((Newtonsoft.Json.Linq.JObject)serializedData)["FromComponentID"].ToObject<int>();
        FromConnectionID = ((Newtonsoft.Json.Linq.JObject)serializedData)["FromConnectionID"].ToObject<int>();
        ToTextBoxID = ((Newtonsoft.Json.Linq.JObject)serializedData)["ToTextBoxID"].ToObject<int>();
    }
}

public struct TextBox
{
    public int ID { get; set; }
    public string TextContent { get; set; }
    public List<Attribute> Attributes { get; set; }
    public List<Components> Components { get; set; }
    public TextBox(Object serializedData)
    {
        ID = ((Newtonsoft.Json.Linq.JObject)serializedData)["ID"].ToObject<int>();
        TextContent = ((Newtonsoft.Json.Linq.JObject)serializedData)["TextContent"].ToObject<string>();

        List<Object> attributes = ((Newtonsoft.Json.Linq.JObject)serializedData)["Attributes"].ToObject<List<Object>>();

        Attributes = new List<Attribute>();
        foreach (Object attribute in attributes)
        {
            Attributes.Add(new Attribute(attribute));
        }

        List<Object> connections = ((Newtonsoft.Json.Linq.JObject)serializedData)["Connections"].ToObject<List<Object>>();

        Components = new List<Components>();
        foreach (Object connection in connections)
        {
            Components.Add(new Components(connection));

        }
    }
}

public struct Components
{
    public int ID { get; set; }
    public List<Attribute> Attributes { get; set; }
    public List<Condition> Conditions { get; set; }
    public List<OutgoingConnection> OutgoingConnections { get; set; }
    public Components(Object serializedData = null)
    {
        if (serializedData == null)
        {
            ID = 0;
            Attributes = new List<Attribute>();
            Conditions = new List<Condition>();
            OutgoingConnections = new List<OutgoingConnection>();
            return;
        }
        ID = ((Newtonsoft.Json.Linq.JObject)serializedData)["ID"].ToObject<int>();

        List<Object> attributes = ((Newtonsoft.Json.Linq.JObject)serializedData)["Attributes"].ToObject<List<Object>>();
        Attributes = new List<Attribute>();
        foreach (Object attribute in attributes)
        {
            Attributes.Add(new Attribute(attribute));
        }

        List<Object> conditions = ((Newtonsoft.Json.Linq.JObject)serializedData)["Conditions"].ToObject<List<Object>>();
        Conditions = new List<Condition>();
        foreach (Object condition in conditions)
        {
            Conditions.Add(new Condition(condition));
        }

        List<Object> outgoingConnections = ((Newtonsoft.Json.Linq.JObject)serializedData)["OutgoingConnections"].ToObject<List<Object>>();
        OutgoingConnections = new List<OutgoingConnection>();
        foreach (Object outgoingConnection in outgoingConnections)
        {
            OutgoingConnections.Add(new OutgoingConnection(outgoingConnection));
        }
    }
}

public struct OutgoingConnection
{
    public int ID { get; set; }
    public List<Condition> Conditions { get; set; }

    public OutgoingConnection(Object serializedData)
    {
        ID = ((Newtonsoft.Json.Linq.JObject)serializedData)["_id"].ToObject<int>();

        List<Object> conditions = ((Newtonsoft.Json.Linq.JObject)serializedData)["Conditions"].ToObject<List<Object>>();
        Conditions = new List<Condition>();
        foreach (Object condition in conditions)
        {
            Conditions.Add(new Condition(condition));
        }
    }
}

public struct Attribute
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

public struct Condition
{
    public int ID { get; set; }
    public string Value { get; set; }
    public Condition(Object serializedData)
    {
        ID = ((Newtonsoft.Json.Linq.JObject)serializedData)["ID"].ToObject<int>();
        Value = ((Newtonsoft.Json.Linq.JObject)serializedData)["Value"].ToObject<string>();
    }
}