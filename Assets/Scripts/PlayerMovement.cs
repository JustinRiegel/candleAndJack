using UnityEngine;

//Requires that the gameobject this compontent is on also has a Rigidbody2D
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 5f;
    public AbilityCooldownManager QAbility;
    public AbilityCooldownManager EAbility;

    private Vector2 movement;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (QAbility == null)
        {
            Debug.LogWarning("Jack does not have any ability set for Q");
        }

        if (EAbility == null)
        {
            Debug.LogWarning("Jack does not have any ability set for E");
        }
        sr = GetComponent<SpriteRenderer>();

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

        if (EAbility != null && Input.GetKeyDown(KeyCode.E))
        {
            EAbility.UseAbility("disableLight");
        }

        if (movement.x >= 0.1f)
        {
            sr.flipX = true;
            
        }
        else if (movement.x <= -0.1f)
        {
            sr.flipX = false;
        }

    }

    void FixedUpdate()
    {
        //called 50 times a second
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

    }
}
