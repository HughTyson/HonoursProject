using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IntersectionHandler
{
    List<Intersection> intersections;

    public IntersectionHandler()
    {
        intersections = new List<Intersection>();
    }

    public void CreateIntersection(Vector3 position, GameObject parent_road, bool on_init)
    {
        Intersection i = new Intersection();
        i.obj = GameObject.Instantiate(GM_.Instance.config.road_values.intersection_obj);
        i.obj.transform.position = position;
        i.SetParent(parent_road);
        i.inverse = on_init;

        //used for debugging
        if(on_init)
        {
            i.obj.GetComponent<IntersectionValueShow>().direction_of_parent = parent_road.GetComponent<RoadSegment>().direction;
        }
        else
        {
            i.obj.GetComponent<IntersectionValueShow>().direction_of_parent = parent_road.GetComponent<RoadSegment>().direction * -1;
        }

        i.obj.GetComponent<IntersectionValueShow>().parent_road = parent_road;

        intersections.Add(i);

    }

    public List<Intersection> GetIntersections()
    {
        return intersections;
    }
}

public class Intersection
{
    public GameObject obj;
    GameObject parent_road;

    public bool inverse;

    public void SetParent(GameObject object_)
    {
        parent_road = object_;
    }

    public GameObject GetParentRoad()
    {
        return parent_road;
    }

}