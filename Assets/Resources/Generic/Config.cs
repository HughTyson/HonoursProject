using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{

    public GameObject city_transform;

    public int seed = 1;

    public bool apply_city_limits = false;
    public float city_limits_x = 700;
    public float city_limits_z = 700;

    public RoadValues road_values;
    public BuildingPlotValues building_plot_values;
    public RandomPeaks random_peaks_values;

}

[System.Serializable]
public class RoadValues
{

    public GameObject road_segment_obj;
    public GameObject intersection_obj;

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

[System.Serializable]
public class BuildingPlotValues
{
    [Tooltip("Will the building height be based on perlin noise or circular noise")]
    public bool use_perlin_noise = false;

    [Tooltip("Minimum Area a Plot can be")]
    public float minimum_area = 5;

    [Tooltip("The minimum height a building can be")]
    public float minimum_height = 25;
    [Tooltip("The maximum height a building can be")]
    public float maximum_height = 50;

    [Tooltip("Minumum Building Dimension - if less then area is a plaza")]
    public float minimum_building_dimension = 25;

    [Tooltip("likelihood of a RoundBuilding")]
    public int likelihood = 100;

}
[System.Serializable]
public class RandomPeaks
{

    [Tooltip("If using circular noise - are the peaks set by a user or are they randomly generetaed")]
    public bool randomly_generate_peaks = false;

    [Tooltip("Range of Peaks. X - Minimum. Y - Maximum")]
    public Vector2 peaks_amount = new Vector2(2,7);

    [Tooltip("Peaks Centre Position Range. X - Minimum. Y - Maximum")]
    public Vector2 peaks_CP__range = new Vector2(1500, 1500);

    [Tooltip("Range of Radius. X - Minimum. Y - Maximum")]
    public Vector2 radius_size_range = new Vector2(700, 2000);


}


