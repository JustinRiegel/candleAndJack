using UnityEditor.Macros;
using UnityEngine;
using UnityEngine.UI;

//Requires that the gameobject this compontent is on also has a Rigidbody2D
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 5f;
    public AbilityCooldownManager QAbility;

    private Vector2 movement;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (QAbility == null)
        {
            Debug.LogWarning("Jack does not have any ability set for Q");
        }
    }

    // Update is called once per frame
    // bad place for physics
    void Update()
    {
        // handle input here
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        if(QAbility != null && Input.GetKeyDown(KeyCode.Q))
        {
            QAbility.UseAbility("stopCandle");
        }

    }

    void FixedUpdate()
    {
        //called 50 times a second
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
