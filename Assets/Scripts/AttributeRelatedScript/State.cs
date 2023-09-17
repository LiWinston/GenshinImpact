using System;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
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

    [Header("Power")] 
    [SerializeField] private float maxPower = 100;
    [SerializeField] private float currentPower = 100;
    [SerializeField] private float powerRegenerationRate = 0.05f; // 每秒恢复5%
    private Image powerBar;
    private GameObject powerBarObject;
    private Color fullPowerColor = Color.green;
    private Color emptyPowerColor = Color.gray;

    [Header("UI Flags")] 
    private bool isHealthUpdated = true;
    private bool isEnergyUpdated = true;
    private bool isPowerUpdated = true;

    [Header("Level and Experience")] 
    [SerializeField]private int currentExperience;
    [SerializeField] private float damageReduction = 0.005f;
    [SerializeField] private int maxLevel = 100;

    public delegate void LevelChangedEventHandler(int newLevel);

    public event LevelChangedEventHandler OnLevelChanged;
    private int currentLevel = 1;
    private int[] experienceThresholds; // 存储升级所需经验值的数组
    [SerializeField] private Transform UpdEffectTransform;

    [Header("CombatJudge")] 
    [SerializeField] private float combatCooldownDuration = 1.8f; //脱战延时
    private bool isInCombat = false;
    private float combatEndTime = 0f;
    

    [Header("Regeneration Rates")] 
    [SerializeField] private float healthRegenerationRate = 0.005f;
    [SerializeField] private float energyRegenerationRate = 0.008f;
    [SerializeField] private float healthRegenAddition = 0.02f;
    [SerializeField] private float energyRegenAddition = 0.08f;
    private float regenerationTimer;

    [Header("Damage")] 
    [SerializeField] private float curDamage = 8f;
    [SerializeField] private float addDamageOnUpdate = 2;
    // [SerializeField] internal float attackAngle = 70f;
    // [SerializeField] internal float attackRange = 0.9f;
    [SerializeField] public float HurricaneKickDamage = 8;
    [SerializeField] public float hurricaneKickKnockbackForce = 70;
    [SerializeField] public float hurricaneKickRange = 1.2f;
    [SerializeField] internal float criticalDmgRate = 4f;
    private AttackCooldownCurve _AttcooldownCurve;
    internal float attackSpeedRate;

    [Header("ZenMode(Recover)")] 
    [SerializeField]private float zenModeP2EConversionEfficiency = 0.6f; // 禅模式下的体力转化率
    private bool isCrouchingCooldown; // 用于记录下蹲后的冷却状态
    private float _shakeBeforeZenMode = 1.5f; // 下蹲冷却时长施法前摇
    private bool isInZenMode; // 是否处于禅模式
    private float zenModeHealthModifier = 1.35f; // 禅模式下的生命值修改器
    private float zenModeHealthRegenModifier = 1.5f; // 禅模式下的生命值恢复速度修改器
    private float zenModeP2EConversionSpeed; // 禅模式下的体力转化率
    private float temporaryHealthRegenRate; // 临时的生命值恢复速度
    private PlayerController plyctl;
    public delegate void ExitZenModeEventHandler();
    public static event ExitZenModeEventHandler OnExitZenMode;


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

    public float GetNormalizedPower()
    {
        return currentPower / maxPower;
    }

// 新增方法检查是否体力已满
    public bool IsFullPower()
    {
        return Mathf.Approximately(currentPower, maxPower);
    }

// 新增方法检查是否体力为空
    public bool IsEmptyPower()
    {
        return Mathf.Approximately(currentPower, 0f);
    }

    public float CurrentPower
    {
        get => currentPower;
        set
        {
            if (value != currentPower)
            {
                currentPower = Mathf.Clamp(value, 0f, maxPower);
                isPowerUpdated = true;
            }
        }
    }


    private void Start()
    {
        plyctl = GetComponent<PlayerController>();
        healthBarObject = GameObject.Find("UIHealthbar");
        energyBarObject = GameObject.Find("UIManabar");
        powerBarObject = GameObject.Find("UIPowerbar");


        healthBar = healthBarObject.GetComponent<Image>();
        energyBar = energyBarObject.GetComponent<Image>();
        powerBar = powerBarObject.GetComponent<Image>();

        // 初始时更新UI
        UpdateHealthUI();
        UpdateEnergyUI();
        UpdatePowerUI();

        currentExperience = 0;
        InitializeExperienceThresholds();

        _AttcooldownCurve = GetComponent<AttackCooldownCurve>();
        if (!_AttcooldownCurve) Debug.LogError("AttackCooldownCurve NotFound");
        UpdateAttackCooldown();
        if (!UpdEffectTransform) UpdEffectTransform = SpellCast.FindDeepChild(transform, "spine_01");
        // if(0 !=_shakeBeforeZenMode) GetComponentInChildren<Animator>().SetFloat("_shakeBeforeZenMode",_shakeBeforeZenMode);
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

        if (isPowerUpdated)
        {
            UpdatePowerUI();
            isPowerUpdated = false;
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

        if (plyctl.isCrouching && !plyctl.isMoving)//蹲且不走
        {
            if (!isCrouchingCooldown && !isInZenMode)//
            {
                isInZenMode = true;
                isCrouchingCooldown = true;
                StartCoroutine(EnterZenMode());// 若见诸相非相，即见如来
            }
        }
        else
        {
            if(isInZenMode) ExitZenMode();
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

    private void UpdatePowerUI()
    {
        float powerNormalized = GetNormalizedPower();
        powerBar.fillAmount = powerNormalized;
        // 定义一个接近于1的阈值
        float fullPowerThreshold = 0.99f;
        powerBar.gameObject.SetActive(powerNormalized <= fullPowerThreshold);
        powerBar.color = Color.Lerp(emptyPowerColor, fullPowerColor, powerNormalized);
       
    }



    public void TakeDamage(float damage)
    {
        var actualDamage = isInZenMode ? damage * (1 - damageReduction) : damage * 1.5f;
        CurrentHealth -= actualDamage <= maxHealth ? actualDamage : CurrentHealth; //vulnerable during zenMode

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

    public void RestorePower(float amount)
    {
        CurrentPower += amount;
    }

// 新增方法用于消耗Power
    public bool ConsumePower(float amount)
    {
        if (CurrentPower >= amount)
        {
            CurrentPower -= amount;
            return true;
        }
        else
        {
            UIManager.Instance.ShowMessage1("Insufficient Stamina, ESCAPE BATTLE!");
            return false;
        }
    }

    // 获取当前等级
    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void CheatLevelUp()
    {
        if (currentLevel < maxLevel) currentLevel += 1;
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
        if (GetComponent<PlayerController>().cheatMode) return;
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
        CurrentDamage += addDamageOnUpdate;
        maxHealth += addMaxHealthOnUpdate;
        // maxEnergy += addMaxEnergyOnUpdate;
        maxEnergy += CurrentDamage * 3;
        CurrentHealth += addHealthOnUpdate;
        // CurrentEnergy += addEnergyOnUpdate;
        CurrentEnergy += CurrentDamage * 2.5f;
        ParticleEffectManager.Instance.PlayParticleEffect("UpLevel", UpdEffectTransform.gameObject, Quaternion.identity,
            Color.clear, Color.clear, 3f);
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
        RestorePower(maxPower * powerRegenerationRate);
    }


    public void IncreaseDamage(float idmg)
    {
        curDamage += idmg;
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

            Debug.Log(currentLevel + "级攻速" + AttackCooldown + "秒，动画倍速 " + attackSpeedRate);
        }
    }

    public float CurrentDamage
    {
        get => curDamage;
        set => curDamage = value;
    }

    public float AttackCooldown { get; set; }

    private IEnumerator EnterZenMode()
    {
        // 等待一秒，模拟下蹲后进入禅模式
        yield return new WaitForSeconds(_shakeBeforeZenMode);

        // 计算禅模式下的体力转化速率，根据玩家等级
        const float maxConversionSpeed = 0.8f; // 最大的转化速度，即满级时的速度
        const float minConversionSpeed = 0.10f; // 起始时的速度，你可以根据需要调整
        const float levelRange = 100f; // 等级范围，即从1级到100级
        zenModeP2EConversionSpeed = minConversionSpeed + (maxConversionSpeed - minConversionSpeed) *
            Mathf.Log(1f + currentLevel) / Mathf.Log(1f + levelRange);

        // 保存当前的生命值恢复速度，以便禅模式结束后还原
        temporaryHealthRegenRate = healthRegenerationRate;

        // 修改生命值恢复速度
        healthRegenerationRate *= zenModeHealthRegenModifier;

        // 开始消耗体力并恢复能量
        StartCoroutine(P2EConvert_ZenMode());
    }


    private void ExitZenMode()
    {
        // 退出禅模式
        isInZenMode = false;

        // 还原生命值恢复速度
        healthRegenerationRate = temporaryHealthRegenRate;

        // 重置体力转化率
        zenModeP2EConversionSpeed = 0f;

        // 重置下蹲冷却状态
        isCrouchingCooldown = false;
        
        OnExitZenMode?.Invoke();//粒子系统停播
    }

    private IEnumerator P2EConvert_ZenMode()
    {
        StartCoroutine(ParticleEffectManager.Instance.PlayParticleEffectUntilEndCoroutine("Zen",
            UpdEffectTransform.gameObject, Quaternion.identity, Color.clear, Color.cyan, ExitZenMode));
        while (isInZenMode)
        {
            // ParticleEffectManager.Instance.PlayParticleEffectUntilEnd("Zen", UpdEffectTransform.gameObject, 
            //     Quaternion.identity, Color.clear, Color.cyan, ExitZenMode);
            if (IsFullEnergy())
            {
                // 如果能量已满，显示提示信息
                UIManager.Instance.ShowMessage1("Full Energy!");
                yield return null;
                continue; // 能量已满，不需要继续处理
            }

            // 消耗体力，根据体力转化率
            float deltaPowerPercent = zenModeP2EConversionSpeed * Time.deltaTime;
            if (ConsumePower(deltaPowerPercent * maxPower))
            {
                // 如果成功消耗体力，就恢复相应比例的能量
                float energyToRestore = deltaPowerPercent * zenModeP2EConversionEfficiency * maxEnergy;
                RestoreEnergy(energyToRestore);
            }
            // 每帧等待
            yield return null;
        }
    }

}