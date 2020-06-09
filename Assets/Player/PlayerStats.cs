using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("HP")]
    public int hp;
    public int maxHp = 100;
    public Slider hpSlider;
    public static bool dead;
    [Header("Stamina")]
    public float  stamina;
    public float  maxstamina = 100;
    public Slider staminaSlider;

    private void Start()
    {
        hpSlider.maxValue = maxHp;
        staminaSlider.maxValue = maxstamina;
        stamina = maxstamina;
        hp = maxHp;
    }

    private void Update()
    {
        UpdateUI();
        if (Input.GetKeyDown(KeyCode.K))
        {
            hp -= 20;
        }
    }

    private void FixedUpdate()
    {
        if (stamina > maxstamina)
        {
            stamina = maxstamina;
        }
        if (hp > maxHp)
        {
            hp = maxHp;
        }

        if (hp <= 0)
        {
            dead = true;
        }

        if (stamina < 0)
        {
            stamina = 0;
        }
    }

    private void UpdateUI()
    {
        hpSlider.value = hp;
        staminaSlider.value = stamina;
    }
}
