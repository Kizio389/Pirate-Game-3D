using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SingletonIndexPlayer
{
    private static SingletonIndexPlayer instance;

    public static SingletonIndexPlayer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SingletonIndexPlayer();
            }
            return instance;
        }
    }

    public float Max_Health { get; set; }
    public float Health { get; set; }
    public float Max_Stamina { get; set; }
    public float Stamina { get; set; }
    public int Coin { get; set; }
    public float Damage { get; set; }

    private SingletonIndexPlayer()
    {
        Max_Health = 100f;
        Health = 100f;
        Max_Stamina = 50f;
        Stamina = 50f;
        Coin = 0;
        Damage = 20;
    }

    public void ResetData()
    {
        Max_Health = 100f;
        Health = Max_Health;
        Max_Stamina = 50f;
        Stamina = Max_Stamina;
        Coin = 0;
        Damage= 20;
    }
}
