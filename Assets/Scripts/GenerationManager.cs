using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{

    [SerializeField] int seed;
    int old_seed;
    //private Vars

    GameObject city;

    BuildingGenerator building_generator = new BuildingGenerator();

    List<BuildingPlot> plots = new List<BuildingPlot>();

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState((int)seed);
        GenerateCity();
    }

    void GenerateCity()
    {
        city = new GameObject("City");

        //road gen

        //plot gen

        

        BuildingPlot temp_plot = new BuildingPlot();
        temp_plot.InitPlot(new Vector3(0, 0, 0), new Vector2(15, 15), Youngs_BuildingType.ROUNDBUILDING, city.transform);
        plots.Add(temp_plot);

       // temp_plot = new BuildingPlot();
      //  temp_plot.InitPlot(new Vector3(6, 0, 0), new Vector2(5, 5), Youngs_BuildingType.ROUNDBUILDING, city.transform);
       // plots.Add(temp_plot);


        //building gen
        building_generator.Generate(plots);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
