using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{

    public int seed = 1;

    public bool apply_city_limits = false;
    public float city_limits_x = 700;
    public float city_limits_z = 700;

    public RoadValues road_values;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class RoadValues
{

    public GameObject road_segment_obj;

    [Tooltip("The maximum amount of roads that will be spawned")]
    public int amount_of_roads = 100;
    [Tooltip("The maximum length of a road segment")]
    public float max_road_segment_length = 35f;
    [Tooltip("The minimum length of a road segment")]
    public float min_road_segment_length = 15f;
    [Tooltip("The width of a road segment")]
    public float road_segment_width = 3f;

    [Tooltip("The maximum distance a road will check infront of itself to create a interesction")]
    public float max_intersection_distance = 15f;
    [Tooltip("The amount of steps that will be taken in order to reach the maximum distance e.g if max distance is 15 and steps is 5, the distance checked infront will incremement by 3 each step")]
    public float intersection_distance_steps_taken = 5f;

    

}