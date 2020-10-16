using System.Collections;
using UnityEngine;

public class CandleStatus : MonoBehaviour
{

    public HealthBar healthBar;
    public int _maxHealth = 3;

    [Header("Damage Settings")]
    [Range(1, 10)]
    public int _secondsPerDamageCheck = 3;
    public int _damageTakenPerInterval = 1;

    [Header("Healing Settings")]
    public bool _canHeal = false;
    public int _healthGainedPerInterval = 1;
    [Range(1, 10)]
    public int _secondsPerHealthCheck = 3;

    private int _currentHealth;

    private bool _isInLight = false;
    private PathfinderAI _candlePathfinderAI;

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
        _candlePathfinderAI = GameObject.FindWithTag("Candle").GetComponent<PathfinderAI>();
    }

    public void HealDamage(int healing)
    {
        if (_canHeal)
        {
            TakeDamage(-healing);
        }
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        healthBar.SetHealth(_currentHealth);
        if(_currentHealth == 0)
        {
            SceneManagerHelper.instance.ChangeScene("LossCandle");
        }
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
            _candlePathfinderAI.SetCanMove(!_isInLight);
            CalculateLightDamage();
            yield return new WaitForSeconds(_secondsPerDamageCheck);
        }
    }

    IEnumerator HealLoop()
    {
        while (true)
        {
            _candlePathfinderAI.SetCanMove(!_isInLight);
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
            TakeDamage(_damageTakenPerInterval);
        }
        else
        {
            HealDamage(_healthGainedPerInterval);
        }
    }
}
