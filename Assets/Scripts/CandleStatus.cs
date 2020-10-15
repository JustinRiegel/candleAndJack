using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleStatus : MonoBehaviour
{

    public HealthBar healthBar;
    public int _maxHealth = 3;

    public int _healthGainedPerInterval = 1;
    public int _damageTakenPerInteraval = 1;

    [Range(1, 10)]
    public int _secondsPerHealthCheck = 3;

    private float _healthCheckTime;
    private int _currentHealth;

    private GameObject _candle;
    private bool _isInLight = false;
    //private Transform _candleTransform;

    // Start is called before the first frame update
    void Start()
    {
        if (healthBar == null)
        {
            Debug.LogError("Candle needs a slider with the healhbar component assigned!");
        }
        else
        {
            healthBar.SetMaxHealth(_maxHealth);
        }
        _currentHealth = _maxHealth;
        _candle = GameObject.FindWithTag("Candle");
    }

    public void HealDamage(int healing)
    {
        TakeDamage(-healing);
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        healthBar.SetHealth(_currentHealth);
    }

    public int GetCurrentHealth()
    {
        return _currentHealth;
    }

    //the difference in these 2 loops is that i wanted the damage to kick in immediately upon entering light
    //and for the healing to take some time to kick in after leaving the light
    //both need to affect the movement immediately and in every loop (to prevent the auto-start-move from kicking in)
    IEnumerator DamageLoop()
    {
        while (true)
        {
            _candle.GetComponent<PathfinderAI>().SetCanMove(!_isInLight);
            CalculateLightDamage();
            yield return new WaitForSeconds(_secondsPerHealthCheck);
        }
    }

    IEnumerator HealLoop()
    {
        while (true)
        {
            _candle.GetComponent<PathfinderAI>().SetCanMove(!_isInLight);
            yield return new WaitForSeconds(_secondsPerHealthCheck);
            CalculateLightDamage();
        }
    }

    public void SetInLightStatus(bool inLightStatus)
    {
        _isInLight = inLightStatus;

        if (_isInLight)
        {
            StopCoroutine("HealLoop");
            StartCoroutine("DamageLoop");
        }
        else
        {
            StopCoroutine("DamageLoop");
            StartCoroutine("HealLoop");
        }
    }

    public void CalculateLightDamage()
    {
        if(_isInLight)
        {
            TakeDamage(_damageTakenPerInteraval);
        }
        else
        {
            HealDamage(_damageTakenPerInteraval);
        }
    }
}
