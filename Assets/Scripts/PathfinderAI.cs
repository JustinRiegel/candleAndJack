﻿using UnityEngine;
using Pathfinding;

public class PathfinderAI : MonoBehaviour
{

    //this will need to be updated with the distractions to be drawn to
    [SerializeField] Transform target;
    [SerializeField] float speed = 100f;
    [Range(0.25f, 3f)]
    [SerializeField] float nextWaypointDistance = 1.5f;
    [SerializeField] float drawDistance = 5f;
    [SerializeField] GameObject[] basePath;
    [SerializeField] bool linearPath = false;
    [SerializeField] bool canBeDistractedByLight = true;
    [SerializeField] GameObject UIElement;
    [SerializeField] GameObject FixingUIElement;
    [SerializeField] Vector2 UIOffset = new Vector2(0, 1.5f);
    [Range(0f, 20f)]
    [SerializeField] float timeToTurnOnLight = 1f;
    [SerializeField] float dragMultiplier = 10;
    [SerializeField] Animator animator;

    private bool hasTarget = true;
    private int currentDefaultPathPoint = 0;
    private bool onDefaultPath = true;
    private Path path;
    private int currentWaypoint = 0;
    private bool isLinearPathReversed = false;
    private Vector3 threeDUIOffset;
    private bool canMove = true;
    private float defaultDrag = 1;

    private GameObject currentUIElement;
    private Seeker seeker;
    private Rigidbody2D rb;
    private bool originalDistractStatus;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if (target == null)
        {
            hasTarget = false;
        }

        if (basePath == null || basePath.Length == 0)
        {
            //if the base path is empty set it to itself
            basePath = new GameObject[] { gameObject };
        }

        if (UIOffset == null)
        {
            threeDUIOffset = new Vector3(0, 1.5f, 0);
        }
        else
        {
            threeDUIOffset = new Vector3(UIOffset.x, UIOffset.y, 0);
        }

        if (FixingUIElement == null)
        {
            FixingUIElement = UIElement;
        }

        originalDistractStatus = canBeDistractedByLight;

        InvokeRepeating("DeterminePath", 0f, 0.5f);
    }

    void DeterminePath()
    {
        if (seeker.IsDone())
        {
            if (hasTarget)
            {
                float distance = Vector2.Distance(rb.position, target.position);

                if (distance <= drawDistance)
                {
                    //if we are within draw distance of the closest target go towards it
                    seeker.StartPath(rb.position, target.position, OnPathComplete);
                    currentWaypoint = 0;
                    SetOnDefaultPath(false);
                    return;
                }
            }

            //if we don't have a target or aren't near it go to the base path
            seeker.StartPath(rb.position, basePath[currentDefaultPathPoint].transform.position, OnPathComplete);
            currentWaypoint = 0;
            SetOnDefaultPath(true);
        }
    }


    private void SetOnDefaultPath(bool onPath)
    {
        if (onDefaultPath != onPath)
        {
            onDefaultPath = onPath;
            if (currentUIElement != null)
            {
                GameObject.Destroy(currentUIElement);
            }
            if (!onDefaultPath)
            {
                currentUIElement = Instantiate(UIElement, transform.position + threeDUIOffset, Quaternion.identity, transform);
                AudioManager.instance.PlaySound("NPCDistract");
            }
        }
    }


    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {

        //if we don't have a path return immediately
        if (path == null)
        {

            return;
        }

        //if we can't move return immediatley
        if (!canMove)
        {
            return;
        }


        if (currentWaypoint >= path.vectorPath.Count)
        {
            if (onDefaultPath)
            {
                var distanceToTarget = Vector2.Distance(rb.position, basePath[currentDefaultPathPoint].transform.position);

                if (distanceToTarget < nextWaypointDistance)
                {
                    DetermineNextDefaultPathPoint();
                    DeterminePath();
                }
            }
            return;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distanceToNextPoint = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distanceToNextPoint < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (rb.velocity.x >= 0.1f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (rb.velocity.x <= -0.1f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

    }

    private void DetermineNextDefaultPathPoint()
    {

        //for linear paths we travel to the end of the path point by point and then travel back by reversing those points
        if (linearPath)
        {
            //this is true if we have reached the end of the path and are now traveling back
            if (isLinearPathReversed)
            {
                //reduce by 1
                currentDefaultPathPoint--;
                //if we are now below one we are at the start of the path again
                if (currentDefaultPathPoint < 0)
                {
                    currentDefaultPathPoint = 1;
                    isLinearPathReversed = false;
                }
            }
            //this is for linear paths that are not currently reversed
            else
            {
                currentDefaultPathPoint++;
                //if we are at the end of the path we want to reverse it and start going backwards
                if (currentDefaultPathPoint >= basePath.Length)
                {
                    isLinearPathReversed = true;
                    currentDefaultPathPoint = basePath.Length - 1;
                }
                //otherwise we just want to increment by 1
            }
        }
        else
        //for cycling paths increment by 1
        {
            currentDefaultPathPoint++;
            //if we are at the end of the path we want to go back to the first path point
            if (currentDefaultPathPoint >= basePath.Length)
            {
                currentDefaultPathPoint = 0;
            }
        }
        if (currentDefaultPathPoint < 0 || currentDefaultPathPoint >= basePath.Length)
        {
            Debug.LogWarning(this.name + " has an invalid Default path point, clamping" + currentDefaultPathPoint);
            currentDefaultPathPoint = Mathf.Clamp(currentDefaultPathPoint, 0, basePath.Length - 1);
        }
    }

    public bool GetCanBeDistractedByAttractor()
    {
        return canBeDistractedByLight;
    }

    public void SetNewTarget(GameObject newTarget)
    {
        if (newTarget == null)
        {
            hasTarget = false;
            target = null;
        }
        else
        {
            hasTarget = true;
            target = newTarget.transform; // put the thing here 
        }
        DeterminePath();
    }

    public Transform GetTarget()
    {
        return target;
    }

    public void SetCanMove(bool newCanMove)
    {
        if (currentUIElement != null)
        {
            GameObject.Destroy(currentUIElement);
        }

        canMove = newCanMove;
        //if the object is stopped increase the drag so it slows tf down, if its started up again remove the drag clamp
        rb.drag = canMove ? defaultDrag : defaultDrag * dragMultiplier;

        //Automatically start moving again after an amount of time (defaults to off)
        if (!canMove)
        {
            Invoke("AutoCanMove", timeToTurnOnLight);
            currentUIElement = Instantiate(FixingUIElement, transform.position + threeDUIOffset, Quaternion.identity, transform);
            canBeDistractedByLight = false;
        }
        if (canMove)
        {
            CancelInvoke("AutoCanMove");
            canBeDistractedByLight = originalDistractStatus;
        }

        animator.SetBool("CanMove", canMove);
    }

    private void AutoCanMove()
    {
        SetCanMove(true);
    }

    public float getTimeToTurnOnLight()
    {
        return timeToTurnOnLight;
    }
}