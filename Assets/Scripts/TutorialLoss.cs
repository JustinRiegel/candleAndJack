using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLoss : MonoBehaviour
{
    [SerializeField] bool forceTutorialLoss = true;
    [SerializeField] Vector3 theShadowRealm;

    private void Start()
    {
        if (theShadowRealm == null)
        {
            theShadowRealm = new Vector3(-300f, 0, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (forceTutorialLoss)
        {
            if (collision.CompareTag("Player"))
            {
                GameObject.FindWithTag("Candle").transform.position = theShadowRealm;
            }
        }
    }
}
