using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownManager : MonoBehaviour
{
    public Image sliderFill;
    public Color startColor;
    public Color endColor;
    public Button button;
    
    public float rechargeTime = 5.0f;
    private float rechargeEnd;
    bool canUseAbility = true;

    private CandleAI candleAI;
    private PlayerStatus _playerStatus;

    public void Start()
    {
        sliderFill.color = endColor;
        candleAI = GameObject.FindWithTag("Candle").GetComponent<CandleAI>();
        _playerStatus = GameObject.FindWithTag("Player").GetComponent<PlayerStatus>();
    }

    public void StartRecharge()
    {
        rechargeEnd = Time.time + rechargeTime;
        sliderFill.fillAmount = 1.0f;
        canUseAbility = false;
        button.interactable = false;
    }

    public void EndRecharge()
    {
        sliderFill.fillAmount = 0.0f;
        canUseAbility = true;
        button.interactable = true;
    }

    public void UseAbility(string abilityName)
    {
        if (canUseAbility)
        {
            if (abilityName == "stopCandle")
            {
                StartRecharge();
                StartCoroutine(ShowCooldownAnimation());
                candleAI.ToggleCanMove();
            }
            if (abilityName == "disableLight")
            {
                if (_playerStatus.IsPlayerNearLight())
                {
                    StartRecharge();
                    StartCoroutine(ShowCooldownAnimation());
                    _playerStatus.DisableLight();
                }
            }
        }
    }

    public bool GetCanUseAbility()
    {
        return canUseAbility;
    }

    IEnumerator ShowCooldownAnimation()
    {
        for (float t = 0f; t < rechargeTime; t += Time.deltaTime)
        {
            sliderFill.fillAmount = (rechargeEnd - Time.time) / rechargeTime;
            //Debug.Log("fillAmount is " + sliderFill.fillAmount);
            sliderFill.color = Color.Lerp(startColor, endColor, t / rechargeTime);
            yield return null;
        }
        sliderFill.color = endColor;
        EndRecharge();
    }

}
