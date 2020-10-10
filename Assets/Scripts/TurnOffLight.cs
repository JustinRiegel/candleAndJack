using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffLight : MonoBehaviour
{
    private bool isJackInLight = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isJackInLight = true;
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
        }
    }

    private void Update()
    {
        if (isJackInLight && Input.GetKeyDown(KeyCode.E))
        {
            DecoLight dL = transform.parent.gameObject.GetComponent<DecoLight>();
            dL.TurnOffLight();
        }
    }
    
}
