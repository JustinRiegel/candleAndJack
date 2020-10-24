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
    }

    public void RemoveAttractor(GameObject removedAttractor)
    {
        attractorList.Remove(removedAttractor);
        updatePathfinderTargets();
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
        foreach(var npc in pathfinderAIs)
        {
            npc.SetNewTarget(null);
        }

        foreach (PathfinderAI pathfinder in pathfinderAIs)
        {
            if (pathfinder.GetCanBeDistractedByAttractor())//i dont think this check is needed anymore since candle is no longer a pathfinder
            {
                var closestAttractor = FindClosestAttractor(pathfinder.gameObject);
                closestAttractor.GetComponentInParent<DecoLight>().SetCurrentSeeker(pathfinder);

                pathfinder.SetNewTarget(closestAttractor);
            }
        }
    }
}
