using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffLight : MonoBehaviour
{
    private bool isJackInLight = false;
    private DecoLight decoLight;
    private CandleStatus _candleStatus;

    private void Start()
    {
        decoLight = transform.parent.gameObject.GetComponent<DecoLight>();
        _candleStatus = GameObject.FindWithTag("Candle").GetComponent<CandleStatus>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isJackInLight = true;
            NotifyJackInLight();
        }

        //something here could be what damages poor little candle
        if (collision.CompareTag("Candle"))
        {
            NotifyCandleLightUpdate(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isJackInLight = false;
            NotifyJackInLight();
        }

        if (collision.CompareTag("Candle"))
        {
            NotifyCandleLightUpdate(false);
        }
    }

    private void Update()
    {
        if (isJackInLight && Input.GetKeyDown(KeyCode.E))
        {
            decoLight.TurnOffLight();
        }
    }
    
    private void NotifyJackInLight()
    {
        decoLight.SetJackInLight(isJackInLight);
    }

    private void NotifyCandleLightUpdate(bool candleInLight)
    {
        _candleStatus.SetInLightStatus(candleInLight);
    }
}
