using UnityEngine;

public class TurnOnLight : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TrickRTreater"))
        {
            var pathfinderAI = collision.gameObject.GetComponent<PathfinderAI>();
            if (pathfinderAI != null)
            {
                pathfinderAI.SetCanMove(false);
                var timeToLight = pathfinderAI.getTimeToTurnOnLight();
                Debug.Log(this.name + "Invoking TurnOnParentLight in " + timeToLight);
                Invoke("TurnOnParentLight", timeToLight);
            }
        }
    }


    private void TurnOnParentLight()
    {
        DecoLight dL = transform.parent.gameObject.GetComponent<DecoLight>();
        if (dL != null)
        {
            Debug.Log (this.name + " TurnOnParentLight Invoked");
            dL.TurnOnLight();
        }
    }
}
