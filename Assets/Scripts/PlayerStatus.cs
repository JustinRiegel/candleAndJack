using System.Collections;
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

    [Range(0f, 100f)]
    public float lowHealthPercent = 70f;
    [Range(0f, 100f)]
    public float criticalHealthPercent = 50f;

    [Range(0.01f, 1f)]
    public float lowHealthPitch = 0.9f;
    [Range(0.01f, 1f)]
    public float critcalHealthPitch = 0.8f;

    [Range(0.01f, 1f)]
    public float lowHealthMusicVolume = 0.8f;
    [Range(0.01f, 1f)]
    public float critcalHealthMusicVolume = 0.6f;

    public bool canLoseByJack = true;
    public bool isTutorial = false;

    private float healthCheckTime;
    private int currentHealth;

    private Transform candle;
    private bool _isNearLight;
    private DecoLight _nearbyLight;
    private PlayerMovement _playerMovement;

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
        _playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        healthCheckTime = 1 / healthChecksPerSecond;
        StartCoroutine("CheckHealth");
    }

    private void Update()
    {
        if(_isNearLight && _nearbyLight != null)
        {
            _nearbyLight.SetDisableAbilityIsReady(_playerMovement.EAbility.GetCanUseAbility());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("light"))
        {
            _isNearLight = true;
            _nearbyLight = collision.transform.parent.gameObject.GetComponent<DecoLight>();
            _nearbyLight.SetJackInLight(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("light"))
        {
            _isNearLight = false;
            if (_nearbyLight != null)
            {
                _nearbyLight.SetJackInLight(false);//important to call this BEFORE we null it out
            }
            _nearbyLight = null;
        }
    }

    public bool IsPlayerNearLight()
    {
        return _isNearLight;
    }

    public void DisableLight()
    {
        if(_isNearLight && _nearbyLight != null)
        {
            _nearbyLight.TurnOffLight();
            AudioManager.instance.PlaySound("InteractLightOff");
        }
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
            if (canLoseByJack)
            {
                SceneManagerHelper.instance.ChangeScene("LossJack");
            }
            if (isTutorial)
            {
                SceneManagerHelper.instance.ChangeScene("Game");
            }    
        }
        Heartbeat();
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

    private void Heartbeat()
    {

        float currentPercentHealth = ((float)currentHealth / maxHealth);

        //if we have no health we are actively changing scenes so we should stop the sounds
        if (currentHealth == 0)
        {
            AudioManager.instance.StopSound("HeartbeatFast");
            AudioManager.instance.StopSound("HeartbeatSlow");
        }
        //critical health
        else if (currentPercentHealth < (criticalHealthPercent / 100f))
        {
            AudioManager.instance.StopSound("HeartbeatSlow");
            AudioManager.instance.PlaySound("HeartbeatFast");
            AudioManager.instance.ChangePitch("GameMusic", critcalHealthPitch);
            AudioManager.instance.ChangeVolume("GameMusic", critcalHealthMusicVolume);
        }
        //bad health
        else if (currentPercentHealth < (lowHealthPercent / 100f))
        {
            AudioManager.instance.StopSound("HeartbeatFast");
            AudioManager.instance.PlaySound("HeartbeatSlow");
            AudioManager.instance.ChangePitch("GameMusic", lowHealthPitch);
            AudioManager.instance.ChangeVolume("GameMusic", lowHealthMusicVolume);
        }
        //vibing
        else
        {
            AudioManager.instance.StopSound("HeartbeatFast");
            AudioManager.instance.StopSound("HeartbeatSlow");
            AudioManager.instance.ChangePitch("GameMusic", 1f);
            AudioManager.instance.ChangeVolume("GameMusic", 1f);
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
