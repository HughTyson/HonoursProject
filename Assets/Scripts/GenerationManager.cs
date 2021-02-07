using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{

    [SerializeField] int seed;
    [SerializeField] GameObject road_prefab;
    int old_seed;
    //private Vars

    GameObject city;

    PMRoadGen PM_road_gen = new PMRoadGen();
    BuildingGenerator building_generator = new BuildingGenerator();

    List<BuildingPlot> plots = new List<BuildingPlot>();

    // Start is called before the first frame update
    void Start()
    {
        //seed can be changed in the editor
        Random.InitState((int)seed);
        PM_road_gen.road_segment = road_prefab;


        GenerateCity();

        GM_.Instance.config.a = 1;

        Debug.Log(GM_.Instance.config.a);
        
    }

    void GenerateCity()
    {
        city = new GameObject("City");

        //road gen
        PM_road_gen.Generate();
        //plot gen

        //set a temporary plot

        BuildingPlot temp_plot = new BuildingPlot();
        temp_plot.InitPlot(new Vector3(0, 0, 5f), new Vector2(15, 15), Youngs_BuildingType.ROUNDBUILDING, city.transform);
        plots.Add(temp_plot);

        temp_plot = new BuildingPlot();
        temp_plot.InitPlot(new Vector3(15, 0, 0), new Vector2(15, 15), Youngs_BuildingType.BLOCKYBUILDING, city.transform);
        plots.Add(temp_plot);


        //building gen
       // building_generator.Generate(plots);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
