using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCondition : MonoBehaviour
{
    

    [Header("Defaults")]
    [SerializeField] private float healthMax = 100f;

    [SerializeField] private float hungerMax = 100f;

    [SerializeField] private float staminaMax = 100f;
    
    private Conditions health;  
    private Conditions hunger;  
    private Conditions stamina; 

    private void Awake()
    {
        health = new Conditions();
        hunger = new Conditions();
        stamina = new Conditions();
        
        InitConditions();
    }

    private void Start()
    {
        Debug.Log($"health + {health.curValue} \n stamina + {stamina.curValue}\n hunger +{hunger.curValue}");
    }
    

    public void InitConditions()
    {
        health.maxValue =  healthMax;
        health.curValue = healthMax;
        
        hunger.maxValue =  hungerMax;
        hunger.curValue = hungerMax;
        
        stamina.maxValue =  staminaMax;
        stamina.curValue = staminaMax;
    }
    

    public void Damage(float damage)
    {
        health.curValue -= damage;
        Debug.Log("damage"+damage);
        Debug.Log($"current health {health.curValue}");
    }
    
    
    
}
