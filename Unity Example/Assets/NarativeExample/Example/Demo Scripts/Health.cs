using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public void Start()
    {
        AddFunctionsToAPI();
    }
    public void AddFunctionsToAPI()
    {
        Globals.AddFunction(new Action<int>(TakeDamage));
        Globals.AddFunction(new Action<int>(RestoreHealth));
        Globals.AddFunction(new Func<string>(GetFormattedHealth));
        Globals.AddVariables("health", 100);

    }
    public string GetFormattedHealth()
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

    public void TakeDamage(int damage)
    {
        int health = (int)Globals.variables["health"];
        health -= damage;
        if (health < 0) health = 0;
        Globals.variables["health"] = health;
    }

    public void RestoreHealth(int amount)
    {
        int health = (int)Globals.variables["health"];
        health += amount;
        if (health > 100) health = 100;
        Globals.variables["health"] = health;
    }
}