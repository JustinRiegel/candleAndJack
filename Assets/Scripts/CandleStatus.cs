using System.Collections;
using UnityEngine;

public class CandleStatus : MonoBehaviour
{

    public CandleHealthBar healthBar;
    public int _maxHealth = 3;

    [Header("Damage Settings")]
    public int _damageTakenPerInterval = 1;

    [Header("Healing Settings")]
    public bool _canHeal = false;
    public int _healthGainedPerInterval = 1;

    public bool canLoseByCandle = true;

    private int _currentHealth;

    private bool _isInLight = false;
    private bool _isInStationaryLight = false;
    private bool _isInTrickOrTreaterLight = false;
    private CandleAI _candleAI;

    // Start is called before the first frame update
    private void Start()
    {
        if (healthBar == null)
        {
            Debug.LogError("Candle needs a healhbar component assigned!"); 
        }

        _currentHealth = _maxHealth;
        _candleAI = GameObject.FindWithTag("Candle").GetComponent<CandleAI>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("light"))
        {
            _isInStationaryLight = true;
        }
        else if(collision.CompareTag("TrickOrTreaterLight"))
        {
            _isInTrickOrTreaterLight = true;
        }

        if ((_isInStationaryLight || _isInTrickOrTreaterLight) && !_isInLight)
        {
            _isInLight = true;
            CalculateLightDamage();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("light"))
        {
            _isInStationaryLight = false;
        }
        else if (collision.CompareTag("TrickOrTreaterLight"))
        {
            _isInTrickOrTreaterLight = false;
        }

        if (!_isInStationaryLight && !_isInTrickOrTreaterLight)
        {
            _isInLight = false;
        }
    }

    private void HealDamage(int healing)
    {
        if (_canHeal)
        {
            TakeDamage(-healing);
        }
    }

    private void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        healthBar.SetHealth(_currentHealth);
        if (_currentHealth == 0)
        {
            if (canLoseByCandle)
            {
                SceneManagerHelper.instance.ChangeScene("LossCandle");
            }
        }
    }

    public int GetCurrentHealth()
    {
        return _currentHealth;
    }

    public void CalculateLightDamage()
    {
        if (_candleAI.GetCanMove())
        {
            if (_isInLight)
            {
                TakeDamage(_damageTakenPerInterval);
                _candleAI.SetCanMove(!_isInLight);
            }
            else
            {
                HealDamage(_healthGainedPerInterval);
            }
        }
    }

    public void CalculateAutoDamage()
    {
        if (_isInLight)
        {
            TakeDamage(_damageTakenPerInterval);
        }
    }

    public bool GetIsInLight()
    {
        return _isInLight;
    }
}
