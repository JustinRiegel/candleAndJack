using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffLight : MonoBehaviour
{
    private bool isJackInLight = false;
    private DecoLight decoLight;

    private void Start()
    {
        decoLight = transform.parent.gameObject.GetComponent<DecoLight>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isJackInLight = true;
            NotifyJackInLight(isJackInLight);
        }

        //something here could be what damages poor little candle
        if (collision.CompareTag("Candle"))
        {
            //nothing yet
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isJackInLight = false;
            NotifyJackInLight(isJackInLight);
        }
    }

    private void Update()
    {
        if (isJackInLight && Input.GetKeyDown(KeyCode.E))
        {
            decoLight.TurnOffLight();
        }
    }
    
    private void NotifyJackInLight(bool isJackInLight)
    {
        decoLight.SetJackInLight(isJackInLight);
    }

}
