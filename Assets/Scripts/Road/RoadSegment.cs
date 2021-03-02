using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DFSSearch
{
    public bool visited;
}
public class RoadSegment : MonoBehaviour
{

    public DFSSearch dfs;

    GameObject road_object;

    Transform object_transform;
    BoxCollider collider;

    public Transform parent;
    public Vector3 direction;

    public List<GameObject> connected_points_all;
    public List<GameObject> connected_child_nodes;


    public List<GameObject> connected_segments_endpoints;
    public List<GameObject> connected_segments_midpoints;
    public List<GameObject> connected_segments_intersection;

    public int road_number;

    private void Start()
    {
        
    }
    public void RoadSegmentInit(Vector3 position, Vector3 scale, float rot, Transform p, Vector3 dir, GameObject obj)
    {        
        
        object_transform = GetComponent<Transform>();
        collider = GetComponent<BoxCollider>();

        transform.rotation = Quaternion.Euler(new Vector3(0, rot, 0));
        transform.position = position;
        transform.localScale = scale;

        parent = p;

        direction = dir;

        road_object = obj;

        connected_segments_endpoints = new List<GameObject>(); 
        connected_segments_midpoints = new List<GameObject>();
        connected_segments_intersection = new List<GameObject>();

        dfs.visited = false;

    }

    public Transform GetTransform()
    {
        return object_transform;
    }

    public void ReScaleCollisionBox()
    {
        collider.size = new Vector3(1, 1, 1);
    }

    public void AddConnectedSegmentEndPoint(GameObject obj)
    {
        connected_segments_endpoints.Add(obj);
        connected_points_all.Add(obj);
    }

    public void AddConnectedSegmentMidPoints(GameObject obj)
    {
        connected_segments_midpoints.Add(obj);
        connected_points_all.Add(obj);
    }

    public void AddConnectedIntersection(GameObject obj)
    {
        connected_segments_intersection.Add(obj);
        connected_points_all.Add(obj);
    }

    public void AddChildNode(GameObject obj)
    {
        connected_child_nodes.Add(obj);
    }

    public GameObject GetObj()
    {
        return road_object;
    }

    public void CleanUpConnection()
    {

        for(int i = connected_segments_endpoints.Count - 1; i > -1; i--)
        {
            if(connected_segments_endpoints[i] == null)
            {
                connected_segments_endpoints.RemoveAt(i);
                
            }
        }

        for (int i = connected_segments_midpoints.Count - 1; i > -1; i--)
        {
            if (connected_segments_midpoints[i] == null)
            {
                connected_segments_midpoints.RemoveAt(i);

            }
        }

        for (int i = connected_points_all.Count - 1; i > -1; i--)
        {
            if (connected_points_all[i] == null)
            {
                connected_points_all.RemoveAt(i);

            }
        }

        for (int i = connected_child_nodes.Count - 1; i > -1; i--)
        {
            if (connected_child_nodes[i] == null)
            {
                connected_child_nodes.RemoveAt(i);

            }
        }
    }

    public bool RemoveObj(GameObject obj)
    {

        if(connected_points_all.Remove(obj))
        {
            return true;
        }

        return false;
    }

    public void RoadNum(int i)
    {
        road_number = i;
    }

    public void Update()
    {

        

        foreach(GameObject obj in connected_points_all)
        {
            Debug.DrawLine(transform.position, obj.transform.position, Color.red);
        }

    }

}
