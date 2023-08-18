using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    public float maxEnergy = 100f;
    public float currentEnergy = 100f;

    public GameObject healthBarObject;
    public GameObject energyBarObject;

    private Image healthBar;
    private Image energyBar;

    private void Start()
    {
        healthBar = healthBarObject.GetComponent<Image>();
        energyBar = energyBarObject.GetComponent<Image>();
        currentHealth = 100f;
        currentEnergy = 100f;
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
        currentHealth -= damage;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
    }

    public void ConsumeEnergy(float amount)
    {
        currentEnergy -= amount;
    }

    public void RestoreEnergy(float amount)
    {
        currentEnergy += amount;
    }
}
