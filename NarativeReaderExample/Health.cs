using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NarativeReaderExample
{
    public static class Health
    {
        public static void AddFunctionsToAPI()
        {
            Globals.AddFunction(new Action<int>(TakeDamage));
            Globals.AddFunction(new Func<string>(GetFormattedHealth));
        }
        public static string GetFormattedHealth()
        {
            int health = (int)Globals.variables["health"];
            switch (health)
            {
                case > 70:
                    return "Healthy";
                case > 30:
                    return "Injured";
                default:
                    return "Criticaly Injured";
            }
        }

        public static void TakeDamage(int damage)
        {
            int health = (int)Globals.variables["health"];
            health -= damage;
            if (health < 0) health = 0;
            Globals.variables["health"] = health;
        }
    }
}
