using UnityEngine;
using Pathfinding;

public class CandleAI : MonoBehaviour
{
    [SerializeField] float speed = 100f;
    [Range(0.25f, 3f)]
    [SerializeField] float nextWaypointDistance = 0.5f;
    [SerializeField] GameObject[] basePath;
    [SerializeField] float dragMultiplier = 10;
    [SerializeField] float timeToAutoCanMove = 3;
    [SerializeField] bool canWinFromPath = true;
    [SerializeField] Animator candleAnimator;

    private int currentDefaultPathPoint = 0;
    private bool canMove = true;
    private float defaultDrag = 1;

    private Rigidbody2D rb;
    private CandleStatus candleStatus;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(this.name + " nextWaypointDistance: " + nextWaypointDistance);
        rb = GetComponent<Rigidbody2D>();
        candleStatus = GetComponent<CandleStatus>();

        if (basePath == null || basePath.Length == 0)
        {
            Debug.LogWarning("Candle needs to have a path set");
        }

        defaultDrag = rb.drag;

        if (candleAnimator == null)
        {
            Debug.LogWarning("Candle needs to have an animator set, check the GFX child");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //if we can't move return immediately as well
        if (!canMove)
        {

            return;
        }

        //Adding nullcheck for length here
        if (basePath.Length != 0)
        {
            var distanceToTarget = Vector2.Distance(rb.position, basePath[currentDefaultPathPoint].transform.position);
            if (distanceToTarget < nextWaypointDistance)
            {
                currentDefaultPathPoint++;
                //if we have reached the end of the path win
                if (currentDefaultPathPoint >= basePath.Length)
                {
                    if (canWinFromPath)
                    {
                        SceneManagerHelper.instance.ChangeScene("Win");
                    }
                    //even if will win from this we still don't want to error out, so set the path back to the begining
                    currentDefaultPathPoint = 0;
                }
            }

            Vector2 direction = ((Vector2)basePath[currentDefaultPathPoint].transform.position - rb.position).normalized;
            Vector2 force = direction * speed * Time.deltaTime;

            rb.AddForce(force);

            if (rb.velocity.x >= 0.1f)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (rb.velocity.x <= -0.1f)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
    }

    public void SetCanMove(bool newCanMove)
    {
        canMove = newCanMove;
        //if the object is stopped increase the drag so it slows tf down, if its started up again remove the drag clamp
        rb.drag = canMove ? defaultDrag : defaultDrag * dragMultiplier;

        //Automatically start moving again after an amount of time (defaults to off)
        if (!canMove && timeToAutoCanMove > 0)
        {
            Invoke("AutoCanMove", timeToAutoCanMove);
        }

        candleAnimator.SetBool("isHiding", !canMove);

    }

    private void AutoCanMove()
    {

        if (candleStatus.GetIsInLight())
        {
            candleStatus.CalculateLightDamage();
            Invoke("AutoCanMove", timeToAutoCanMove);
            return;
        }

        SetCanMove(true);
    }

    public bool GetCanMove()
    {
        return canMove;
    }
}