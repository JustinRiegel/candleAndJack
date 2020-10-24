using System.Collections.Generic;
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
        updatePathfinderTargets();
        //CheckCurrentTargets(newAttractor);
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

        var temp = newAttractor.GetComponentInParent<DecoLight>();

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

    public PathfinderAI FindClosestPathfinder(GameObject attractor)
    {
        Vector2 attractorLocation = attractor.transform.position;
        float shortestDistance = float.MaxValue;
        PathfinderAI closestPathfinder = null;

        foreach(var npc in pathfinderAIs)
        {
            //if the pathfinder already has a target, it has already been determined to be closest to that attractor
            //and so it should not be considered a potential seeker for an attractor
            if(!npc.GetCanBeDistractedByAttractor() || npc.GetTarget() != null)
            {
                continue;
            }

            var currentDistance = Vector2.Distance(attractorLocation, npc.transform.position);
            if(currentDistance < shortestDistance)
            {
                shortestDistance = currentDistance;
                closestPathfinder = npc;
            }
        }

        return closestPathfinder;
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
        //originally we were looping over all the pathfinders and checking against their location to the attractor
        //to determine the closest attractor to each pathfinder. with the addition of "NPC communication" that means
        //only one pathfinder needs to go to a given attractor at a time. i am also adjusting the logic to be
        //attractor-centric as all attractors need to have a pathfinder, but not all pathfinders need an attractor

        //TODO: do we need to null out targets for all pathfinders? i think so, otherwise its possible to end up with multiple PFs heading to the same attractor
        //we want to clear out current target of all the pathfinders because they're either going to be assigned a new target or they need to not have a target
        foreach(var npc in pathfinderAIs)
        {
            npc.SetNewTarget(null);
        }

        foreach(var attractor in attractorList)
        {
            var closestPathfinder = FindClosestPathfinder(attractor);
            attractor.GetComponentInParent<DecoLight>().SetCurrentSeeker(closestPathfinder);
            closestPathfinder.SetNewTarget(attractor);
            //find closest pathfinder regardless of if it has a target or not. because we are looping over attractors, we do not need to worry about
            //assign them as mutual targets
        }

        //foreach (PathfinderAI pathfinder in pathfinderAIs)
        //{
        //    if (pathfinder.GetCanBeDistractedByAttractor())
        //    {
        //        var closestAttractor = FindClosestAttractor(pathfinder.gameObject);
        //        closestAttractor.GetComponentInParent<DecoLight>().SetCurrentSeeker(pathfinder);

        //        pathfinder.SetNewTarget(closestAttractor);
        //    }
        //}
    }
}
