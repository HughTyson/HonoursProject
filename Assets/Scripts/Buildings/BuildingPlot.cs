using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlot
{


    [HideInInspector] public Vector3 plot_centre;
    [HideInInspector] public Vector2 plot_dimensions;
    [HideInInspector] public Youngs_BuildingType building_type;
    [HideInInspector] public Transform city_transform;

    //have this contain info like maybe type of building, voneit is in e.g residential, business, 

    public void InitPlot(Vector3 centre, Vector2 dimensions, Youngs_BuildingType type, Transform transform)
    {
        plot_centre = centre;
        plot_dimensions = dimensions;
        building_type = type;
        city_transform = transform;
    }

}
