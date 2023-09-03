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
                UpdateEnergyUI();
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

    // 更新生命值UI
    private void UpdateHealthUI()
    {
        healthBar.fillAmount = GetNormalizedHealth();
    }

    // 更新能量值UI
    private void UpdateEnergyUI()
    {
        energyBar.fillAmount = GetNormalizedEnergy();
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
