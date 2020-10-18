using UnityEngine;

public class TurnOnLight : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TrickRTreater"))
        {
            DecoLight dL = transform.parent.gameObject.GetComponent<DecoLight>();
            dL.TurnOnLight();
        }
    }

}
