using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{


    
    //private Vars

    GameObject city;

    PMRoadGen PM_road_gen = new PMRoadGen();
    BuildingGenerator building_generator = new BuildingGenerator();
    BlockGenerator block_generator = new BlockGenerator();
    BuildingPlotGenerator plot_generator = new BuildingPlotGenerator();

    List<BuildingPlot> plots = new List<BuildingPlot>();

    // Start is called before the first frame update
    void Start()
    {
        //seed can be changed in the editor
        Random.InitState(GM_.Instance.config.seed);

        if (GM_.Instance.config.random_peaks_values.randomly_generate_peaks)
        {
            GM_.Instance.procedural.RandomlyCreatePeaks();
        }
        Random.InitState(GM_.Instance.config.seed);

        GenerateCity();
    }

    void GenerateCity()
    {
        city = new GameObject("City");

        //road gen
        PM_road_gen.Generate();

        //city block generation
        block_generator.Generate(PM_road_gen.GetRoads());

        //generate the biuilding plots in each city block
        plots = plot_generator.GeneratePlots(block_generator.GetCityBlocks());

        //building gen
        building_generator.Generate(plots);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
