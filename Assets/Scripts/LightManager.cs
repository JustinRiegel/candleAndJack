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
        updatePathfinderTargets();
    }

    public void RemoveAttractor(GameObject removedAttractor)
    {
        attractorList.Remove(removedAttractor);
        updatePathfinderTargets();
    }

    public GameObject AddClosestAttractor(GameObject source)
    {
        Vector2 sourceLocation = source.transform.position;
        float lowestDistance = float.MaxValue;
        GameObject retval = null;

        foreach(GameObject attractor in attractorList)
        {
            float currentDistance = Vector2.Distance(sourceLocation, attractor.transform.position);
            if (currentDistance < lowestDistance)
            {
                retval = attractor;
            }
        }
        return retval;
    }

    private void updatePathfinderTargets()
    {
        foreach(PathfinderAI pathfinder in pathfinderAIs)
        {
            if (pathfinder.GetCanBeDistractedByAttractor())
            {
                pathfinder.SetNewTarget(AddClosestAttractor(pathfinder.gameObject));
            }
        }
    }
}
