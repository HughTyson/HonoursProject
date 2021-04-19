using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum Youngs_BuildingType
{
    ROUNDBUILDING,
    BLOCKYBUILDING,
    TOWERBUILDING
}

public class BuildingGenerator
{
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uv;
    int indice_vertices = 0,
    indice_triangles = 0,
    indice_UV = 0;
    float window_size = .05f;

    public void Generate(List<BuildingPlot> plots) //change this into a list of plots???
    {

        float height_limits = GM_.Instance.config.building_plot_values.maximum_height - GM_.Instance.config.building_plot_values.minimum_height;
        
        foreach(BuildingPlot plot in plots)
        {

            if(!plot.empty) //the plot has a building
            {
                //float building_height = GM_.Instance.config.building_plot_values.minimum_height + (height_limits * GM_.Instance.perlin_noise.GetPoint(plot.plot_centre.x, plot.plot_centre.z));  //useing perlin noise to decide heights of buildings

                float building_height = GM_.Instance.procedural.GetPointCircle(plot.plot_centre.x, plot.plot_centre.z);

                //reset verticies, tris and UV's
                indice_triangles = 0;
                indice_vertices = 0;
                indice_UV = 0;


                switch (plot.building_type) //what type of building is in the plot
                {
                    case Youngs_BuildingType.ROUNDBUILDING:
                    {
                            RoundBuilding(plot, building_height); //generate a cylindrical building
                            break;    
                    }
                    case Youngs_BuildingType.BLOCKYBUILDING:
                    {

                            ModernBuilding(plot, building_height); //generate a blocky building
                            break;
                    }
                    case Youngs_BuildingType.TOWERBUILDING:
                    {
                            TowerBuilding(plot, building_height);
                        break;
                    }

                }
            }
        }

    }

    public void Generate(BuildingPlot plots) //change this into a list of plots???
    {

        float height_limits = GM_.Instance.config.building_plot_values.maximum_height - GM_.Instance.config.building_plot_values.minimum_height;


            float building_height = GM_.Instance.config.building_plot_values.minimum_height + (height_limits * GM_.Instance.procedural.GetPoint(plots.plot_centre.x, plots.plot_centre.y));  //useing perlin noise to decide heights of buildings

        //reset verticies, tris and UV's
        indice_triangles = 0;
        indice_vertices = 0;
        indice_UV = 0;


        switch (plots.building_type) //what type of building is in the plot
        {
            case Youngs_BuildingType.ROUNDBUILDING:
                {
                    RoundBuilding(plots, building_height); //generate a cylindrical building
                    break;
                }
            case Youngs_BuildingType.BLOCKYBUILDING:
                {

                    ModernBuilding(plots, building_height); //generate a blocky building
                    break;
                }
            case Youngs_BuildingType.TOWERBUILDING:
                {
                    TowerBuilding(plots, building_height);
                    break;
                }

        }


    }
    void TowerBuilding(BuildingPlot plot, float maximum_height)
    {
        int tiers = Random.Range(3, 6);

        Color colour = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);

        vertices = new Vector3[24 * tiers + 12];
        triangles = new int[36 * tiers + 12];
        uv = new Vector2[24 * tiers + 12];

        float random_distribution = Random.Range(0.5f, 0.8f);

        float height = Random.Range(random_distribution * maximum_height, maximum_height);
        float difference = height - random_distribution * maximum_height;

        float last_height = random_distribution * maximum_height;


        Vector3 lb = new Vector3(0, 0, 0);
        Vector3 rt = new Vector3(plot.plot_dimensions.x , last_height, plot.plot_dimensions.y );

        GameObject new_building = new GameObject("tower building");
        new_building.transform.position = plot.plot_centre;

        Block block = new Block(lb,rt, window_size);
        GameObject building_block = new GameObject("block");
        building_block.AddComponent<MeshCollider>();


        CreateMesh(building_block, block.GetVertices(), block.GetTriangles(), block.GetUV(), colour);

        new_building.transform.parent = plot.city_transform;
        building_block.transform.parent = new_building.transform;
        building_block.transform.localPosition = new Vector3(0, 0, 0);
        new_building.transform.localPosition += new Vector3(-plot.plot_dimensions.x / 2, 0, -plot.plot_dimensions.y / 2);

        for (int i = 1; i < 3; i++)
        {
            lb = new Vector3(lb.x + (1 * i), last_height, lb.z + (1 * i));
            rt = new Vector3(rt.x - (1 * i), last_height + (difference / i), rt.z - (1 * i));
            block = new Block(lb, rt, window_size);

            AddVertices(block.GetVertices(), block.GetTriangles(), block.GetUV());
            last_height += (difference / i);

            building_block = new GameObject("block");
            building_block.AddComponent<MeshCollider>();

            CreateMesh(building_block, block.GetVertices(), block.GetTriangles(), block.GetUV(), colour);

            building_block.transform.parent = new_building.transform;
            building_block.transform.localPosition = new Vector3(0, 0, 0);
        }


        // CreateMesh(new_building, vertices, triangles, uv, colour);


        //new_building.transform.localPosition = plot.plot_centre ;
    }

    void RoundBuilding(BuildingPlot plot, float height)
    {

        Color colour = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);

        GameObject base_ = new GameObject("base");
        Vector3 lb = new Vector3(0, 0, 0);
        Vector3 rt = new Vector3(plot.plot_dimensions.x, 0.05f, plot.plot_dimensions.y);

        vertices = new Vector3[24];
        triangles = new int[36];
        uv = new Vector2[24];

        Block block = new Block(lb, rt, window_size);
        AddVertices(block.GetVertices(), block.GetTriangles(), block.GetUV());
        CreateMesh(base_, vertices, triangles, uv, colour);




        int slices = 36;//amount of slices that make up the cylinder
        int slices_skipped = Random.Range(0, 12);   //the amount of slices that will be skippped
        int index_skipped = Random.Range(3, slices / 2 - slices_skipped - 3);   //the itteration at whichthey will be skipped

        vertices = new Vector3[(slices + 1 - slices_skipped * 2) * 4 + 50];
        triangles = new int[(slices - slices_skipped * 2) * 12 + 72];
        uv = new Vector2[(slices + 1 - slices_skipped * 2) * 4 + 50];

        //generation of the cylinder
        Cylinder cylinder = new Cylinder(plot, height, slices, slices_skipped, index_skipped);
        AddVertices(cylinder.GetVertices(), cylinder.GetTriangles(), cylinder.GetUV());

        ////creating the gameobject
        GameObject new_building = new GameObject("round building");
        CreateMesh(new_building, vertices, triangles, uv, colour);

        new_building.transform.parent = plot.city_transform;
        new_building.transform.position = plot.plot_centre;

        //create the base

        
        base_.transform.parent = new_building.transform;
        base_.transform.localPosition = new Vector3(0, 0, 0);
        base_.transform.localPosition += new Vector3(-plot.plot_dimensions.x / 2, 0, - plot.plot_dimensions.y/2);


    }

    void ModernBuilding(BuildingPlot plot, float maximum_height)
    {

        Color colour = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);

   

        //init variables 
        float minHeight;
        Vector3 lbMain, rtMain;
        Vector3 lb, rt;
        
        List<int> directions = new List<int>();
        directions.Add(0); directions.Add(1); directions.Add(2); directions.Add(3);

        int wings_limit = UnityEngine.Random.Range(1, 5);

        vertices = new Vector3[24 * (wings_limit + 2)];
        triangles = new int[36 * (wings_limit + 2)];
        uv = new Vector2[24 * (wings_limit + 2)];

        //create the base

        minHeight = Random.Range(3f, 7f);

        lb = new Vector3(0, 0, 0);
        rt = new Vector3(plot.plot_dimensions.x, minHeight,  plot.plot_dimensions.y);

        Block block = new Block(lb, rt, window_size);
        AddVertices(block.GetVertices(), block.GetTriangles(), block.GetUV());

        //create the main tower

        float height = UnityEngine.Random.Range(maximum_height * 0.7f, maximum_height);

        lbMain = new Vector3(UnityEngine.Random.Range(0f, plot.plot_dimensions.x * 0.4f), minHeight, UnityEngine.Random.Range(0f, plot.plot_dimensions.y * 0.4f));
        rtMain = new Vector3(plot.plot_dimensions.x * 0.6f + UnityEngine.Random.Range(0f, plot.plot_dimensions.x * 0.4f), height, plot.plot_dimensions.y * 0.6f + UnityEngine.Random.Range(0f, plot.plot_dimensions.y * 0.4f));

        block = new Block(lbMain, rtMain, window_size);
        AddVertices(block.GetVertices(), block.GetTriangles(), block.GetUV());

        maximum_height = height;

        //create the wings
        GameObject new_building = new GameObject("modern building");

        for (int i = 0; i < 3; i++)
        {
            height = UnityEngine.Random.Range(maximum_height * 0.75f, maximum_height);

            int direction = directions[UnityEngine.Random.Range(0, directions.Count)];
            directions.Remove(direction);

            switch (direction)
            {
                case 0:
                    lb = new Vector3(UnityEngine.Random.Range(lbMain.x, 0.5f * plot.plot_dimensions.x), minHeight, UnityEngine.Random.Range(0.5f * plot.plot_dimensions.y, lbMain.z));
                    rt = new Vector3(UnityEngine.Random.Range(0.5f * plot.plot_dimensions.x, rtMain.x), height, plot.plot_dimensions.y);
                    break;
                case 1:
                    lb = new Vector3(UnityEngine.Random.Range(lbMain.x, 0.5f * plot.plot_dimensions.x), minHeight, UnityEngine.Random.Range(0.5f * plot.plot_dimensions.y, lbMain.z));
                    rt = new Vector3(plot.plot_dimensions.x, height, UnityEngine.Random.Range(rtMain.z, plot.plot_dimensions.y * 0.5f));
                    break;
                case 2:
                    lb = new Vector3(0f, minHeight, UnityEngine.Random.Range(0.5f * plot.plot_dimensions.y, lbMain.z));
                    rt = new Vector3(UnityEngine.Random.Range(0.5f * plot.plot_dimensions.x, rtMain.x), height, UnityEngine.Random.Range(rtMain.z, plot.plot_dimensions.y * 0.5f));
                    break;
                case 3:
                    lb = new Vector3(UnityEngine.Random.Range(lbMain.x, 0.5f * plot.plot_dimensions.x), minHeight, 0f);
                    rt = new Vector3(UnityEngine.Random.Range(0.5f * plot.plot_dimensions.x, rtMain.x), height, UnityEngine.Random.Range(rtMain.z, plot.plot_dimensions.y * 0.5f));
                    break;
            }

            block = new Block(lb, rt, window_size);
            //AddVertices(block.GetVertices(), block.GetTriangles(), block.GetUV());

            GameObject building_block = new GameObject("modern building");
            building_block.AddComponent<MeshCollider>();
            
            CreateMesh(building_block, block.GetVertices(), block.GetTriangles(), block.GetUV(), colour);

            building_block.transform.parent = new_building.transform;

            maximum_height = height;
        }

        //create the shape

        CreateMesh(new_building, vertices, triangles, uv, colour);



        new_building.transform.parent = plot.city_transform;
        new_building.transform.localPosition = plot.plot_centre - new Vector3(plot.plot_dimensions.x/2, 0, plot.plot_dimensions.y/ 2);
        

    }

    void AddVertices(Vector3[] vertices, int[] triangles, Vector2[] uv)
    {



        foreach (int i in triangles)
        {
            this.triangles[indice_triangles++] = i + indice_vertices;
        }
        foreach (Vector3 v in vertices)
        {
            this.vertices[indice_vertices++] = v;
        }
        foreach (Vector2 u in uv)
        {
            this.uv[indice_UV++] = u;
        }
    }

    void CreateMesh(GameObject obj, Vector3[] vertices, int[] triangles, Vector2[] uv, Color base_colour)
    {
        //adding coponents for rendering the mesh
        obj.AddComponent<MeshFilter>();
        obj.AddComponent<MeshRenderer>();
        obj.AddComponent<MeshCollider>();

        Mesh mesh = new Mesh(); //a new mesh

        //setting the vertices, tris and UV's of the mesh to the calculated ones
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        obj.GetComponent<MeshFilter>().mesh = mesh;


        obj.GetComponent<MeshRenderer>().material.color = base_colour;

        obj.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        obj.GetComponent<MeshCollider>().sharedMesh = mesh;

        obj.layer = 8;
    }

}
