using System.Collections.Generic;
using System.Linq;

namespace Honours_Stage_Project.Services
{
    public class LuaValidationResult
    {
        public List<string> Errors { get; } = new List<string>();

        public bool IsValid
        {
            get { return !Errors.Any(); }
        }
    }
}