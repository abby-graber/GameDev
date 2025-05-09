using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public static HealthBar Instance { get; private set; }

    public Image HealthBarFill;
    public float maxHealth = 100f;
    private float currentHealth;
    public float testTime = 20.0f;

    void Awake() {}
    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log(currentHealth + " " + maxHealth);
        UpdateHealthBar();
        
    }
    public float ReturnMana()
    {
        return currentHealth;
    } 
    private void UpdateHealthBar() {

        HealthBarFill.fillAmount = (currentHealth / maxHealth); 
        
        
        Debug.Log("HealthBarFill: " + HealthBarFill.fillAmount + "-- currenet / max -->  "+ currentHealth / maxHealth);
    }
    public void TakeDamage(float amount) { 
        if (amount > currentHealth) {
            Debug.Log("Not enough mana");
        }
        currentHealth -= amount;
        UpdateHealthBar();
    }
    public void HealDamage(float amount) {
        currentHealth += amount;
        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
        UpdateHealthBar();
    }
    // Update is called once per frame
    
}
