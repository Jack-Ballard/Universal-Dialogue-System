using System.Collections.Generic;

namespace Honours_Stage_Project.Services
{
    public class LuaStubDefinition
    {
        public Dictionary<string, LuaStubVariable> Variables { get; set; } = new Dictionary<string, LuaStubVariable>();
        public List<LuaStubMember> Functions { get; set; } = new List<LuaStubMember>();
        public List<LuaStubMember> Attributes { get; set; } = new List<LuaStubMember>();
    }

    public class LuaStubVariable
    {
        public string Type { get; set; }
        public object Value { get; set; }
    }

    public class LuaStubMember
    {
        public string Name { get; set; }
        public string DeclaringType { get; set; }
        public string ReturnType { get; set; }
        public List<LuaStubParameter> Parameters { get; set; } = new List<LuaStubParameter>();
    }

    public class LuaStubParameter
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}