﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Security.Cryptography;
using System.Linq;
using UnityEditor.Experimental.Rendering;
using System;
using System.Linq.Expressions;

public class PathfinderAI : MonoBehaviour
{

    //this will need to be updated with the distractions to be drawn to
    [SerializeField] Transform target;
    [SerializeField] float speed = 100f;
    [SerializeField] float nextWaypointDistance = 1.5f;
    [SerializeField] float drawDistance = 5f;
    [SerializeField] GameObject[] basePath;
    [SerializeField] bool linearPath = false;
    [SerializeField] bool canBeDistractedByLight = true;
    [SerializeField] float dragMultiplier = 10;
    [SerializeField] float timeToAutoCanMove = -1;

    private bool hasTarget = true;
    private int currentDefaultPathPoint = 0;
    private bool onDefaultPath = true;
    private Path path;
    private int currentWaypoint = 0;
    private bool isLinearPathReversed = false;
    private bool canMove = true;
    private float defaultDrag = 1;

    private Seeker seeker;
    private Rigidbody2D rb;

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
            basePath = new GameObject[] {gameObject};
        }

        defaultDrag = rb.drag;

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
                    onDefaultPath = false;
                    return;
                }
            }

            //if we don't have a target or aren't near it go to the base path
            seeker.StartPath(rb.position, basePath[currentDefaultPathPoint].transform.position, OnPathComplete);
            onDefaultPath = true;
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
        if (path == null)
        {
            return;
        }

        if (!canMove)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            if (onDefaultPath)
            {
                currentDefaultPathPoint = DetermineNextDefaultPathPoint();
                DeterminePath();
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

    int DetermineNextDefaultPathPoint()
    {
        int retVal;

        if (linearPath)
        {
            if (isLinearPathReversed)
            {
                if (currentDefaultPathPoint <= 0)
                {
                    retVal = 1;
                    isLinearPathReversed = false;
                }
                else
                {
                    retVal = currentDefaultPathPoint - 1;
                }
            }
            else
            {
                if (currentDefaultPathPoint >= basePath.Length - 1)
                {
                    isLinearPathReversed = true;
                    retVal = basePath.Length - 1;
                }
                else
                {
                    retVal = currentDefaultPathPoint + 1;
                }
            }
        }
        else
        //for cycling paths
        {
            if (currentDefaultPathPoint >= basePath.Length - 1)
            {
                retVal = 0;
            }
            else
            {
                retVal = currentDefaultPathPoint + 1;
            }
        }
        retVal = Mathf.Clamp(retVal, 0, basePath.Length - 1);
        return retVal;
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
            target = newTarget.transform;
        }
        DeterminePath();
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
    }

    private void AutoCanMove()
    {
        SetCanMove(true);
    }    

    public bool GetCanMove()
    {
        return canMove;
    }
}