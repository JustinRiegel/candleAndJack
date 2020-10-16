using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{

    public HealthBar healthBar;
    public int maxHealth = 1000;

    public float healingDistanceFromCandle = 4;
    public float damagingDistanceFromCandle = 5;

    public int healthGainedPerInterval = 1;
    public int damageTakenPerInteraval = 1;

    [Range(0.25f, 60f)]
    public float healthChecksPerSecond = 10f;

    private float healthCheckTime;
    private int currentHealth;

    private Transform candle;

    // Start is called before the first frame update
    void Start()
    {
        if (healthBar == null)
        {
            Debug.LogError("Jack needs a slider with the healhbar component assigned!");
        }
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        candle = GameObject.FindWithTag("Candle").transform;
        healthCheckTime = 1 / healthChecksPerSecond;
        StartCoroutine("CheckHealth");
    }

    public void HealDamage(int healing)
    {
        TakeDamage(-healing);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth);
        if (currentHealth == 0)
        {
            SceneManagerHelper.instance.ChangeScene("LossJack");
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    IEnumerator CheckHealth()
    {
        while (true)
        {
            yield return new WaitForSeconds(healthCheckTime);
            CandleDamage();
        }
    }

    public void CandleDamage()
    {
        float distanceFromCandle = Vector2.Distance(transform.position, candle.position);

        if (distanceFromCandle <= healingDistanceFromCandle)  //if we are within candle's healing light
        {
            HealDamage(healthGainedPerInterval);
        }
        else if (distanceFromCandle >= damagingDistanceFromCandle)  //we are far from candle and it hurts!
        {
            TakeDamage(damageTakenPerInteraval);
        }
    }
}
