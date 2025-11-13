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
        health.curValue = healthMax*0.6f;
        
        hunger.maxValue =  hungerMax;
        hunger.curValue = hungerMax*0.6f;
        
        stamina.maxValue =  staminaMax;
        stamina.curValue = staminaMax*0.6f;
        
        speed.maxValue =  speedMax;
        speed.curValue = speedMax*0.2f;
        
        jump.maxValue =  jumpMax;
        jump.curValue = jumpMax * 0.2f;
    }
    

    public void NotifyUI()
    {
        if (UiManager.Instance == null) return;

        float h  = health.curValue  / healthMax;
        float s  = stamina.curValue / staminaMax;
        float hu = hunger.curValue  / hungerMax;

        UiManager.Instance.UpdateStatUI(h, s, hu); // 0~1 비율 전달
    }

    public void Damage(float dmg)
    {
        health.curValue = Mathf.Clamp(health.curValue - dmg, 0, healthMax);
        NotifyUI();
    }

    public void HealHealth(float v)
    {
        health.curValue = Mathf.Clamp(health.curValue + v, 0, healthMax);
        NotifyUI();
    }
    public void HealStamina(float v)
    {
        stamina.curValue = Mathf.Clamp(stamina.curValue + v, 0, staminaMax);
        NotifyUI();
    }
    public void HealHunger(float v)
    {
        hunger.curValue = Mathf.Clamp(hunger.curValue + v, 0, hungerMax);
        NotifyUI();
    }
    public void SpeedUp(float v)
    {
        speed.curValue = Mathf.Clamp(speed.curValue + v, 0, speedMax);
        // 스피드/점프는 슬라이더 없으면 생략 가능
        NotifyUI();
    }
    public void JumpUp(float v)
    {
        jump.curValue = Mathf.Clamp(jump.curValue + v, 0, jumpMax);
        NotifyUI();
    }
    
    
    
}
