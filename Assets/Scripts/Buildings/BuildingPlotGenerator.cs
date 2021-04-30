using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class BuildingPlotGenerator : MonoBehaviour
{

    List<BuildingPlot> building_plots = new List<BuildingPlot>();
    public List<BuildingPlot> GeneratePlots(List<CityBlock> city_blocks)
    {
        List<BuildingPlot> plots = new List<BuildingPlot>();

        foreach(CityBlock block in city_blocks) //loop for each city block
        {
            foreach(Plot plot in block.GetLots()) //loop for each plot in the city block
            {
                    //create a building plot
                    BuildingPlot bp = new BuildingPlot();
                    
                    //set min and max x
                    plot.vertexes = plot.vertexes.OrderBy(v => v.x).ToList();
                    float min_x = plot.vertexes[0].x;
                    float max_x = plot.vertexes[plot.vertexes.Count - 1].x;

                    //set min and max z
                    plot.vertexes = plot.vertexes.OrderBy(v => v.z).ToList();
                    float min_z = plot.vertexes[0].z;
                    float max_z = plot.vertexes[plot.vertexes.Count - 1].z;

                    //initilise plot
                    bp.InitPlot(new Vector3((min_x + max_x) / 2, 0.1f, (min_z + max_z) / 2),
                            new Vector2(max_x - (min_x + max_x) / 2, max_z - (min_z + max_z) / 2) * 2,
                            SetType(Random.Range(0, GM_.Instance.config.building_plot_values.likelihood)),
                            GM_.Instance.config.city_transform.transform,
                            plot.type == CityBlockType.BUILDING ? false : true);

                    plots.Add(bp);  //store the plot

            }
        }

        return plots;
    }

    public List<BuildingPlot> GetPlots()
    {
        return building_plots;
    }

    //pass in a rnadom value
    Youngs_BuildingType SetType(int value)
    {

        if(value % GM_.Instance.config.building_plot_values.likelihood == 0)  //number is divisible by liklihood 
        {
            return Youngs_BuildingType.ROUNDBUILDING;   //return a round building
        }


        if (value % 2 == 0) //number is even
        {
            return Youngs_BuildingType.BLOCKYBUILDING;
        }
        else
        {
            return Youngs_BuildingType.TOWERBUILDING;
        }


        return Youngs_BuildingType.BLOCKYBUILDING;

    }


}
