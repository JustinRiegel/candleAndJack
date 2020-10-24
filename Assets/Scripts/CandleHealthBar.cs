using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CandleHealthBar : MonoBehaviour
{
    [SerializeField] Sprite[] healthSprites;

    private Image image;
    private int maxHealthForUI;
    private int currentHealth;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        maxHealthForUI = healthSprites.Length;
        SetHealth(maxHealthForUI);
    }

    public void SetHealth(int newHealth)
    {
        if (newHealth > maxHealthForUI)
        {
            Debug.LogError("Candle health attempted to be set to " + newHealth + " The UI only supports a max health of " + maxHealthForUI);
        }

        currentHealth = Mathf.Clamp(newHealth, 1, maxHealthForUI);
        image.sprite = healthSprites[currentHealth - 1];
    }

}
