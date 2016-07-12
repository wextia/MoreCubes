﻿using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    Color healthyColor;
    [SerializeField]
    Color hurtColor;
    [SerializeField]
    float maxHealth = 100;
    [SerializeField]
    TextMesh textMesh;
    [SerializeField]
    float healingPerEnemy = .25f;
    float currentHealth;
    MeshRenderer meshRenderer;

    float totalHealingDone = 0;

    static PlayerHealth playerHealth;

    public static PlayerHealth instance
    {
        get
        {
            if (!playerHealth)
            {
                playerHealth = FindObjectOfType<PlayerHealth>();
                if (!playerHealth)
                {
                    Debug.LogError("There needs to be one active PlayerHealth script on a GameObject in your scene.");
                }
                else
                {
                    playerHealth.Init();
                }
            }

            return playerHealth;
        }
    }

    void Init()
    {
        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
    }

    public static float MaxHealth
    {
        get
        {
            return instance.maxHealth;
        }

        set
        {
            instance.maxHealth = value;
            CurrentHealth = instance.maxHealth;
        }
    }

    public static float CurrentHealth
    {
        get
        {
            return instance.currentHealth;
        }

        set
        {
            instance.currentHealth = value;
            Color currentColor =
            Color.Lerp(instance.healthyColor, instance.hurtColor,
            (instance.maxHealth - instance.currentHealth) / instance.maxHealth);
            instance.meshRenderer.material.color = currentColor;
            instance.textMesh.text = instance.currentHealth.ToString("F2") + "%";
            if (instance.currentHealth < 0)
            {
                Destroy(instance.gameObject);
            }
        }
    }

    public static float HealingPerEnemy
    {
        get
        {
            return instance.healingPerEnemy;
        }

        set
        {
            instance.healingPerEnemy = value;
        }
    }

    //COULD BE PROBLEMATIC
    void Start() {
        CurrentHealth = maxHealth;
    }

    void OnEnable()
    {
        EventManager.StartListening(EventManager.EventType.OnUpgradesShown, OnUpgradesShown);
        EventManager.StartListening(EventManager.EventType.OnEnemyKilled, OnEnemyKilled);
    }

    void OnDisable()
    {
        EventManager.StopListening(EventManager.EventType.OnUpgradesShown, OnUpgradesShown);
        EventManager.StopListening(EventManager.EventType.OnEnemyKilled, OnEnemyKilled);
    }

    void OnEnemyKilled()
    {
        if(CurrentHealth + healingPerEnemy < MaxHealth)
        {
            CurrentHealth += healingPerEnemy;
            totalHealingDone += healingPerEnemy;
            StatisticsControl.SetStat(StatisticsControl.Stat.healedLife, Mathf.RoundToInt(totalHealingDone));
        }
    }

    void OnUpgradesShown()
    {
        CurrentHealth = maxHealth;
    }
}