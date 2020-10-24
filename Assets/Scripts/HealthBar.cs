using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{

    private Slider slider;
    public CanvasGroup screenEffect;
    [Range(1f, 100f)]
    public float maxScreenEffectHealthPercent = 70f;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        maxScreenEffectHealthPercent /= 100;
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        if (screenEffect != null)
        {
            float currentHealthPercent = (float)health / (float)slider.maxValue;
            if (currentHealthPercent <= maxScreenEffectHealthPercent)
            {
                var currentLerpVal = currentHealthPercent / maxScreenEffectHealthPercent;
                screenEffect.alpha = Mathf.Lerp(1f, 0f, currentLerpVal);
            }
            else
            {
                screenEffect.alpha = 0;
            }
        }
        else
        {
            Debug.Log("Please set a screen effect canvas in jack's healthbar in the UI");
        }
    }
}
