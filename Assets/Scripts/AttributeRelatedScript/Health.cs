using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField]private float maxHealth;
    [SerializeField]private float currentHealth;

    [SerializeField]private float maxEnergy;
    [SerializeField]private float currentEnergy;

    private GameObject healthBarObject;
    private GameObject energyBarObject;

    private Image healthBar;
    private Image energyBar;

    public float CurrentHealth
    {
        get=>currentHealth;
        set =>currentHealth = Mathf.Clamp(value, 0f, maxHealth);// 在设置 CurrentHealth 时，确保值不超出最大生命值范围
    }
    public float CurrentEnergy
    {
        get=>currentEnergy;
        set =>currentEnergy = Mathf.Clamp(value, 0f, maxEnergy);// 在设置 CurrentHealth 时，确保值不超出最大生命值范围
    }
    private void Start()
    {
        healthBarObject = GameObject.Find("UIHealthbar");
        energyBarObject = GameObject.Find("UIManabar");
        
        healthBar = healthBarObject.GetComponent<Image>();
        energyBar = energyBarObject.GetComponent<Image>();
    }

    private void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);

        healthBar.fillAmount = currentHealth / maxHealth;
        energyBar.fillAmount = currentEnergy / maxEnergy;
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
