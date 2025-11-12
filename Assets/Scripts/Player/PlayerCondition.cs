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
    [SerializeField] private float speedMax = 100f;
    [SerializeField] private float jumpMax = 100f;
    
    private Conditions health;  
    private Conditions hunger;  
    private Conditions stamina; 
    private Conditions speed;
    private Conditions jump;

    private void Awake()
    {
        health = new Conditions();
        hunger = new Conditions();
        stamina = new Conditions();
        speed = new Conditions();
        jump = new Conditions();
        
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
        
        speed.maxValue =  speedMax;
        speed.curValue = speedMax*0.2f;
        
        jump.maxValue =  jumpMax;
        jump.curValue = jumpMax * 0.2f;
    }
    

    public void Damage(float damage)
    {
        health.curValue -= damage;
        Debug.Log("damage"+damage);
        Debug.Log($"current health {health.curValue}");
    }
    
    public void HealHealth(float heal)
    {
        float after =  health.curValue + heal;
        float newHealth = Mathf.Min(after, healthMax);
        health.curValue = newHealth;
    }

    public void HealStamina(float heal)
    {
        float after =  stamina.curValue + heal;
        float newStamina = Mathf.Min(after, staminaMax);
        stamina.curValue = newStamina;
    }

    public void HealHunger(float heal)
    {
        float after =  hunger.curValue + heal;
        float newHunger = Mathf.Min(after, hungerMax);
        hunger.curValue = newHunger;
    }

    public void SpeedUp(float s)
    {
        float after =  speed.curValue + s;
        speed.curValue = after < speedMax ? after : speedMax;
    }

    public void JumpUp(float j)
    {
        float after =  jump.curValue + j;
        jump.curValue = after < jumpMax ? after: jumpMax;
    }
    
    
    
}
