// See https://aka.ms/new-console-template for more information
using NarativeReaderExample;
using Attribute = NarativeReaderExample.Attribute;

Console.WriteLine("Hello, World!");
Health.AddFunctionsToAPI();

Importer.ImportFile();
Globals.InitaliseVariables();

int currentTextBoxID = 0;

while(Globals.connections.Any(connection => connection.Item1 == currentTextBoxID))
{
    string text = LuaLogic.FormatByLua(Globals.textBoxes.First(textBox => textBox.ID == currentTextBoxID).TextContent);
    //Console.WriteLine(Globals.textBoxes.First(textBox => textBox.ID == currentTextBoxID).TextContent);
    Console.WriteLine(text);
    foreach (Connection connection in Globals.textBoxes[currentTextBoxID].Connections)
    {
        Console.Write("["+(connection.ID+1)+"]: " );
        foreach (Attribute attribute in connection.Attributes)
        {
            if (attribute.Name == "ChoiceName")
            {
                Console.Write(attribute.Value);
                break;
            }
        }
        Console.WriteLine();
    }
    int responce = -1;
    while (responce < 0 || responce >= Globals.textBoxes[currentTextBoxID].Connections.Count + 1)
    {
        Console.Write("Please select an option: ");
        string input = Console.ReadLine();
        if (!int.TryParse(input, out responce) || responce < 0 || responce >= Globals.textBoxes[currentTextBoxID].Connections.Count+1)
        {
            Console.WriteLine("Invalid input. Please enter a number corresponding to the options above.");
        }
    }
    currentTextBoxID = Globals.connections.First(connection => connection.Item1 == currentTextBoxID && connection.Item2 == Globals.textBoxes[currentTextBoxID].Connections[(responce-1)].ID).Item3;
}