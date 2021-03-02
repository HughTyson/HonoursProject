using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericRoadGen : MonoBehaviour
{

    protected GameObject road_segment;

    protected List<GameObject> accepted_segments;
    protected IntersectionHandler intersection_handler;

    protected int max_roads = 1000;
    protected float max_segment_length = 70;
    protected float min_segment_length = 35;
    protected float segment_width = 3f;
    protected float max_intersection_distance = 15;
    protected float intersection_distance_step = 3;
    protected bool use_city_limits = false;

    
    protected void InitHandlers()
    {
        intersection_handler = new IntersectionHandler();
    }

    protected void InitValues()
    {
        max_roads = GM_.Instance.config.road_values.amount_of_roads;
        max_segment_length = GM_.Instance.config.road_values.max_road_segment_length;
        min_segment_length = GM_.Instance.config.road_values.min_road_segment_length;
        segment_width = GM_.Instance.config.road_values.road_segment_width;

        max_intersection_distance = GM_.Instance.config.road_values.max_intersection_distance;
        intersection_distance_step = max_intersection_distance / GM_.Instance.config.road_values.intersection_distance_steps_taken;

        road_segment = GM_.Instance.config.road_values.road_segment_obj;
        use_city_limits = GM_.Instance.config.apply_city_limits;
    }

    public List<GameObject> GetRoads()
    {
        return accepted_segments;
    }

    public List<Intersection> GetIntersections()
    {
        return intersection_handler.GetIntersections();
    }
}
