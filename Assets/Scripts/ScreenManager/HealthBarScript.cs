using CombatSystem;
using GameManagerScripts;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Slider healthBar;
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthBar.value = currentHealth / maxHealth;
    }
}
