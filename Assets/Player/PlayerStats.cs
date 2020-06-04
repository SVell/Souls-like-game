using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("HP")]
    public int hp;
    public int maxHp = 100;
    [Header("Stamina")]
    public float  stamina;
    public float  maxstamina = 100;

    private void Start()
    {
        stamina = maxstamina;
        hp = maxHp;
    }

    private void FixedUpdate()
    {
        print(stamina);
        if (stamina > maxstamina)
        {
            stamina = maxstamina;
        }
        if (hp > maxHp)
        {
            hp = maxHp;
        }

        if (stamina < 0)
        {
            stamina = 0;
        }
    }
}
