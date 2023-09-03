using UnityEngine;
using UnityEngine.UI;

public class State : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    [SerializeField] private float maxEnergy;
    [SerializeField] private float currentEnergy;

    private GameObject healthBarObject;
    private GameObject energyBarObject;

    private Image healthBar;
    private Image energyBar;
    
    private Color fullHealthColor = Color.green;
    private Color halfHealthColor = Color.yellow;
    private Color lowHealthColor = Color.red;
    private Color emptyHealthColor = new Color(0, 0, 0.5f, 1); // 黑红色

    private Color fullEnergyColor = new Color(0.5f, 0, 0.5f, 1); // 深紫色
    private Color halfEnergyColor = Color.blue;
    private Color emptyEnergyColor = Color.white;

    private bool isHealthUIUpdated = false;
    private bool isEnergyUIUpdated = false;

    public float CurrentHealth
    {
        get => currentHealth;
        set
        {
            if (value != currentHealth) // 仅当值发生变化时更新UI
            {
                currentHealth = Mathf.Clamp(value, 0f, maxHealth);
                UpdateHealthUI();
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
                isEnergyUIUpdated = true;
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
    }

    private void Update()
    {
        // 只有在需要更新时才执行UI更新
        if (isHealthUIUpdated)
        {
            UpdateHealthUI();
            isHealthUIUpdated = false;
        }

        if (isEnergyUIUpdated)
        {
            UpdateEnergyUI();
            isEnergyUIUpdated = false;
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
        CurrentHealth -= damage;
    }

    public void Heal(float amount)
    {
        CurrentHealth += amount;
    }

    public void ConsumeEnergy(float amount)
    {
        CurrentEnergy -= amount;
    }

    public void RestoreEnergy(float amount)
    {
        CurrentEnergy += amount;
    }
}
