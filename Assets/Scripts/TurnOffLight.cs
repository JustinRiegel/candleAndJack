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
            NotifyJackInLight();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isJackInLight = false;
            NotifyJackInLight();
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
}
