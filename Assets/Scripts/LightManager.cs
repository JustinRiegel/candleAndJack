using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private List <GameObject> attractorList = new List<GameObject>();
    private PathfinderAI[] pathfinderAIs;
    private List<GraphLine> _graphLineList = new List<GraphLine>();
    private GraphLineComparer _graphLineComparer = new GraphLineComparer();

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

    private void updatePathfinderTargets()
    {
        //clear out the graph
        _graphLineList.Clear();

        //clear out targets of pathfinders
        foreach(var npc in pathfinderAIs)
        {
            npc.SetNewTarget(null);
        }
        
        foreach(var light in attractorList)
        {
            //clear out seekers of attractors
            light.GetComponentInParent<DecoLight>().SetCurrentSeeker(null);

            foreach(var npc in pathfinderAIs)
            {
                //if the npc can be distracted by a light being off, add a line to the graph between the current light and this npc
                if (npc.GetCanBeDistractedByAttractor())
                {
                    _graphLineList.Add(new GraphLine(light, npc));
                }
            }
        }

        //sort the list by distance
        _graphLineList.Sort(_graphLineComparer);

        foreach(var graphLine in _graphLineList)
        {
            //if the light does not have a seeker and if the pathfinder does not have a target, this is the closest pathfinder for that light
            if(graphLine.DecoLightOfNode.GetCurrentSeeker() == null && graphLine.PathfinderNode.GetTarget() == null)
            {
                graphLine.DecoLightOfNode.SetCurrentSeeker(graphLine.PathfinderNode);
                graphLine.PathfinderNode.SetNewTarget(graphLine.AttractorNode);
            }
        }
    }

    private class GraphLine
    {
        public GameObject AttractorNode { get; private set; }
        public DecoLight DecoLightOfNode { get; private set; }
        public PathfinderAI PathfinderNode { get; private set; }
        public float Distance { get; private set; }

        public GraphLine(GameObject light, PathfinderAI path)
        {
            AttractorNode = light;
            DecoLightOfNode = AttractorNode.GetComponentInParent<DecoLight>();
            PathfinderNode = path;

            Vector2 lightNodeLocation = AttractorNode.transform.position;
            Vector2 pathfinderLocation = PathfinderNode.transform.position;

            Distance = Vector2.Distance(lightNodeLocation, pathfinderLocation);
        }
    }

    private class GraphLineComparer : IComparer<GraphLine>
    {
        public int Compare(GraphLine x, GraphLine y)
        {
            if(x.Distance < y.Distance)
            {
                return -1;
            }
            else if (x.Distance > y.Distance)
            {
                return 1;
            }

            return 0;
        }
    }
}
