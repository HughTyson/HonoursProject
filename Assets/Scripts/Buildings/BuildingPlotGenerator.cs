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

        foreach(CityBlock block in city_blocks)
        {
            foreach(Plot plot in block.GetLots())
            {
               
                    BuildingPlot bp = new BuildingPlot();

                    plot.vertexes = plot.vertexes.OrderBy(v => v.x).ToList();
                    float min_x = plot.vertexes[0].x;
                    float max_x = plot.vertexes[plot.vertexes.Count - 1].x;

                    plot.vertexes = plot.vertexes.OrderBy(v => v.z).ToList();
                    float min_z = plot.vertexes[0].z;
                    float max_z = plot.vertexes[plot.vertexes.Count - 1].z;

                    bp.InitPlot(new Vector3((min_x + max_x) / 2, 0.1f, (min_z + max_z) / 2), new Vector2(max_x - (min_x + max_x) / 2, max_z - (min_z + max_z) / 2) * 2, Youngs_BuildingType.BLOCKYBUILDING, GM_.Instance.config.city_transform.transform, plot.type == CityBlockType.BUILDING ? false : true);
                    plots.Add(bp);
                

            }
        }

        return plots;
    }

    public List<BuildingPlot> GetPlots()
    {
        return building_plots;
    }


}
