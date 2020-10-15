using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private List <GameObject> attractorList = new List<GameObject>();
    private PathfinderAI[] pathfinderAIs;

    void Start()
    {
        pathfinderAIs = FindObjectsOfType<PathfinderAI>();
    }

    public void AddAttractor(GameObject newAttractor)
    {
        attractorList.Add(newAttractor);
        CheckCurrentTargets(newAttractor);
    }

    public void RemoveAttractor(GameObject removedAttractor)
    {
        attractorList.Remove(removedAttractor);
        updatePathfinderTargets();
    }

    //i added this function for slight optimization that we don't truly need, but there's no reason to loop over every attractor
    //every time a new one is added. simply comparing the current target of each pathfinder with the new attractor should suffice.
    //this function will hold up for now, but i think we'll eventually some sort of way for pathfinders to "communicate"
    //or any in range are going to head for the same attractors. but maybe that is behaviour that we want? worth discussing
    public void CheckCurrentTargets(GameObject newAttractor)
    {
        Vector2 sourceLocation;
        float currentTargetDistance;
        float newTargetDistance;

        foreach(var pathfinder in pathfinderAIs)
        {
            if (pathfinder.GetCanBeDistractedByAttractor())
            {
                sourceLocation = pathfinder.transform.position;
                if (pathfinder.GetTarget() == null)
                {
                    currentTargetDistance = float.MaxValue;
                }
                else
                {
                    currentTargetDistance = Vector2.Distance(sourceLocation, pathfinder.GetTarget().position);
                }
                newTargetDistance = Vector2.Distance(sourceLocation, newAttractor.transform.position);

                if (newTargetDistance < currentTargetDistance)
                {
                    pathfinder.SetNewTarget(newAttractor);
                }
            }
        }
    }

    public GameObject FindClosestAttractor(GameObject source)
    {
        Vector2 sourceLocation = source.transform.position;
        float lowestDistance = float.MaxValue;
        GameObject retval = null;

        foreach(GameObject attractor in attractorList)
        {
            float currentDistance = Vector2.Distance(sourceLocation, attractor.transform.position);
            if (currentDistance < lowestDistance)
            {
                lowestDistance = currentDistance;
                retval = attractor;
            }
        }
        return retval;
    }

    private void updatePathfinderTargets()
    {
        foreach (PathfinderAI pathfinder in pathfinderAIs)
        {
            if (pathfinder.GetCanBeDistractedByAttractor())
            {
                pathfinder.SetNewTarget(FindClosestAttractor(pathfinder.gameObject));
            }
        }
    }
}
