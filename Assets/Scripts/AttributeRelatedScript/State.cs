using System;
using UI;
using UnityEngine;
using UnityEngine.UI;


public class State : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float addHealthOnUpdate;
    [SerializeField] private float addMaxHealthOnUpdate;
    private Image healthBar;
    private GameObject healthBarObject;
    private Color fullHealthColor = Color.red;
    private Color halfHealthColor = Color.magenta;
    private Color lowHealthColor = Color.black;
    private Color emptyHealthColor = new Color(0, 0, 0.5f, 1); // 黑红色

    [Header("Energy")]
    [SerializeField] private float maxEnergy;
    [SerializeField] private float currentEnergy;
    [SerializeField] private float addEnergyOnUpdate;
    [SerializeField] private float addMaxEnergyOnUpdate;
    private Image energyBar;
    private GameObject energyBarObject;
    private Color fullEnergyColor = new Color(0.5f, 0, 0.5f, 1); // 深紫色
    private Color halfEnergyColor = Color.blue;
    private Color emptyEnergyColor = Color.white;

    [Header("UI Flags")]
    private bool isHealthUpdated;
    private bool isEnergyUpdated;

    [Header("Level and Experience")]
    [SerializeField] private int currentExperience;
    [SerializeField] private float damageReduction = 0.005f;
    [SerializeField] private int maxLevel = 100;
    public delegate void LevelChangedEventHandler(int newLevel);
    public event LevelChangedEventHandler OnLevelChanged;
    private int currentLevel = 1; 
    private int[] experienceThresholds; // 存储升级所需经验值的数组
    
    [Header("CombatJudge")]
    private bool isInCombat = false;
    private float combatEndTime = 0f;
    [SerializeField]private float combatCooldownDuration = 1.8f;//脱战延时
    
    [Header("Regeneration Rates")]
    [SerializeField] private float healthRegenerationRate = 0.005f;
    [SerializeField] private float energyRegenerationRate = 0.008f;
    [SerializeField] private float healthRegenAddition = 0.02f;
    [SerializeField] private float energyRegenAddition = 0.08f;
    private float regenerationTimer;

    [Header("Damage")]
    [SerializeField] private float damage = 8f;
    [SerializeField] private float addDamageOnUpdate = 2;
    // [SerializeField] internal float attackAngle = 70f;
    // [SerializeField] internal float attackRange = 0.9f;
    [SerializeField] public float HurricaneKickDamage = 8;
    [SerializeField] public float hurricaneKickKnockbackForce = 70;
    [SerializeField] public float hurricaneKickRange = 1.2f;
    [SerializeField] internal float criticalDmgRate = 4f;
    private AttackCooldownCurve _AttcooldownCurve;
    internal float attackSpeedRate;
    


    public bool IsInCombat()
    {
        return isInCombat;
    }
    public float CurrentHealth
    {
        get => currentHealth;
        set
        {
            if (value != currentHealth) // 仅当值发生变化时更新UI
            {
                currentHealth = Mathf.Clamp(value, 0f, maxHealth);
                isHealthUpdated = true;
            }
        }
    }

    public float CurrentEnergy
    {
        get => currentEnergy;
        set
        {
            if (value != currentEnergy) // 仅当值发生变化时更新UI
            {
                currentEnergy = Mathf.Clamp(value, 0f, maxEnergy);
                isEnergyUpdated = true;
            }
        }
    }

    // 添加用于查询满、空生命、满、空能量或死亡的方法

    // 新增方法返回normalized的生命值（0到1之间的值）
    public float GetNormalizedHealth()
    {
        return currentHealth / maxHealth;
    }

    // 新增方法返回normalized的能量值（0到1之间的值）
    public float GetNormalizedEnergy()
    {
        return currentEnergy / maxEnergy;
    }

    public bool IsFullHealth()
    {
        return Mathf.Approximately(currentHealth, maxHealth);
    }

    public bool IsEmptyHealth()
    {
        return Mathf.Approximately(currentHealth, 0f);
    }

    public bool IsFullEnergy()
    {
        return Mathf.Approximately(currentEnergy, maxEnergy);
    }

    public bool IsEmptyEnergy()
    {
        return Mathf.Approximately(currentEnergy, 0f);
    }

    private void Start()
    {
        healthBarObject = GameObject.Find("UIHealthbar");
        energyBarObject = GameObject.Find("UIManabar");

        healthBar = healthBarObject.GetComponent<Image>();
        energyBar = energyBarObject.GetComponent<Image>();

        // 初始时更新UI
        UpdateHealthUI();
        UpdateEnergyUI();
        
        currentExperience = 0;
        InitializeExperienceThresholds();

        _AttcooldownCurve = GetComponent<AttackCooldownCurve>();
        if(!_AttcooldownCurve) Debug.LogError("AttackCooldownCurve NotFound");
        UpdateAttackCooldown();
    }

    // 初始化升级所需经验值数组
    private void InitializeExperienceThresholds()
    {
        experienceThresholds = new int[maxLevel];
        int baseExperience = 100; // 初始等级所需经验值
        experienceThresholds[0] = baseExperience;
        float experienceGrowthFactor = 1.25f; // 经验值增长因子

        for (int level = 2; level <= maxLevel; level++)
        {
            experienceThresholds[level - 1] = Mathf.FloorToInt(experienceThresholds[level - 2] + baseExperience);
            baseExperience = Mathf.FloorToInt(baseExperience * experienceGrowthFactor);
        }
    }




    private void Update()
    {
        // 只有在需要更新时才执行UI更新
        if (isHealthUpdated)
        {
            UpdateHealthUI();
            isHealthUpdated = false;
        }

        if (isEnergyUpdated)
        {
            UpdateEnergyUI();
            isEnergyUpdated = false;
        }

        // CheckInCombat();
        // if(!isInCombat) RegenerateHealthAndEnergy();
        CheckInCombat();
        if (!isInCombat)
        {
            // 更新回复计时器
            regenerationTimer += Time.deltaTime;

            // 当计时器超过一秒时进行回复
            if (regenerationTimer >= 1.0f)
            {
                RegenerateHealthAndEnergy();
                // 重置计时器
                regenerationTimer = 0f;
            }
        }
        
    }
    // 更新生命值UI
    private void UpdateHealthUI()
    {
        float healthNormalized = GetNormalizedHealth();
        healthBar.fillAmount = healthNormalized;

        if (healthNormalized >= 0.5f)
        {
            healthBar.color = Color.Lerp(halfHealthColor, fullHealthColor, (healthNormalized - 0.5f) * 2);
        }
        else
        {
            healthBar.color = Color.Lerp(lowHealthColor, halfHealthColor, healthNormalized * 2);
        }

        if (Mathf.Approximately(healthNormalized, 0f))
        {
            healthBar.color = emptyHealthColor;
        }
    }

    // 更新能量值UI
    // 更新能量值UI
    private void UpdateEnergyUI()
    {
        float energyNormalized = GetNormalizedEnergy();
        energyBar.fillAmount = energyNormalized;

        if (energyNormalized >= 0.5f)
        {
            energyBar.color = Color.Lerp(halfEnergyColor, fullEnergyColor, (energyNormalized - 0.5f) * 2);
        }
        else
        {
            energyBar.color = Color.Lerp(emptyEnergyColor, halfEnergyColor, energyNormalized * 2);
        }
    }

    public void TakeDamage(float damage)
    {
        if (damage <= maxHealth)
        {
            CurrentHealth -= damage * (1 - damageReduction);
        }
        else
        {
            CurrentHealth = 0f;
        }
        isInCombat = true;
        combatEndTime = Time.time + combatCooldownDuration;
    }

    public void Heal(float amount)
    {
        CurrentHealth += amount;
    }

    /// <summary>
    /// ConsumeEnergy check
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>False => No energy for next spelling</returns>
    public bool ConsumeEnergy(float amount)
    {
        if (CurrentEnergy >= amount)
        {
            CurrentEnergy -= amount;
            return true; 
        }
        else
        {
            UIManager.Instance.ShowMessage1("Insufficient Energy!");
            return false; 
        }
    }

    public void RestoreEnergy(float amount)
    {
        CurrentEnergy += amount;
    }
    
    // 获取当前等级
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    public void CheatLevelUp()
    {
        if(currentLevel < maxLevel) currentLevel += 1;
        LevelUpAction();
    }

    // 获取当前经验值
    public int GetCurrentExperience()
    {
        return currentExperience;
    }

    // 增加经验值并检查是否升级
    public void AddExperience(int experience)
    {
        if(GetComponent<PlayerController>().cheatMode) return;
        currentExperience += experience;

        // 检查是否升级
        int previousLevel = currentLevel;
        for (int i = 0; i < experienceThresholds.Length; i++)
        {
            if (currentExperience >= experienceThresholds[i])
            {
                currentLevel = i + 1;
            }
            else
            {
                break;
            }
        }

        // 如果升级了，执行相应的升级操作
        if (currentLevel > previousLevel)
        {
            // 在这里执行升级后的操作，例如增加伤害减免比例
            LevelUpAction();
        }
    }

    // 更新伤害减免比例
    private void LevelUpAction()
    {
        maxHealth += addMaxHealthOnUpdate;
        maxEnergy += addMaxEnergyOnUpdate;
        CurrentHealth += addHealthOnUpdate;
        CurrentEnergy += addEnergyOnUpdate;
        CurrentDamage += addDamageOnUpdate;
        UpdateAttackCooldown();
        damageReduction = currentLevel * 0.005f; // 每级增加 5%
        if (OnLevelChanged != null)
        {
            OnLevelChanged(currentLevel);
        }
        // 将新的伤害减免比例应用到角色
    }
    
    private void CheckInCombat()
    {
        // Check if the player has taken damage recently
        isInCombat = Time.time < combatEndTime; // Player is considered in combat
    }
    
    private void RegenerateHealthAndEnergy()
    {
            // Regenerate health and energy based on regeneration rates and current level
           Heal(maxHealth * healthRegenerationRate * (1.0f + (currentLevel - 1) * healthRegenAddition));
            RestoreEnergy(maxEnergy * energyRegenerationRate * (1.0f + (currentLevel - 1) * energyRegenAddition));
    }
    
    
    public void IncreaseDamage(float idmg)
    {
        damage += idmg;
            
    }

    public void UpdateAttackCooldown()
    {
        if (currentLevel <= maxLevel)
        {
            AttackCooldown = _AttcooldownCurve.CalculateAttackCooldown(currentLevel);
            attackSpeedRate = _AttcooldownCurve.curvePoints[0].cooldown / AttackCooldown;
            //通知player controller更新动画时间参数
            PlayerController playerController = GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.UpdateAttackAnimationTime(attackSpeedRate);
            }
            Debug.Log(currentLevel + "级攻速" + AttackCooldown +"秒，动画倍速 " + attackSpeedRate);
        }
    }
    public float CurrentDamage
    {
        get=>damage;
        set =>damage = value;
    }

    public float AttackCooldown { get; set; }
}
