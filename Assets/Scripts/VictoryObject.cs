using UnityEngine;

public class VictoryObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Candle"))
        {
            SceneManagerHelper.instance.ChangeScene("Win");
        }
    }
}
