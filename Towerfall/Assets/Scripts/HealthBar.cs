using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public static HealthBar Instance { get; private set; }

    public Image HealthBarFill;
    public float maxMana = 100f;
    private float currentMana;
    public float testTime = 20.0f;
    

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

    }
    void Start()
    {
        currentMana = maxMana;
        Debug.Log(currentMana + " " + maxMana);
        UpdateHealthBar();
        
    }
    public float ReturnMana()
    {
        return currentMana;
    } 
    private void UpdateHealthBar() {

        HealthBarFill.fillAmount = (currentMana / maxMana); 
        
        
        Debug.Log("HealthBarFill: " + HealthBarFill.fillAmount + "-- currenet / max -->  "+ currentMana / maxMana);
    }
    public void UseMana(float amount) { 
        if (amount > currentMana) {
            Debug.Log("Not enough mana");
        }
        currentMana -= amount;
        UpdateHealthBar();
    }
    public void AddMana(float amount) {
        currentMana += amount;
        if (currentMana > maxMana) {
            currentMana = maxMana;
        }
        UpdateHealthBar();
    }
    // Update is called once per frame
    void Update()
    {   
        //UseMana(1.0f / testTime * Time.deltaTime);
        //UseMana(0.001f);
        
    }
}
