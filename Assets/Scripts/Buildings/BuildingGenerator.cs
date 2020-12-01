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

        float building_height = 30;  //use perlin noise here

        foreach(BuildingPlot plot in plots)
        {

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


    void TowerBuilding(BuildingPlot plot, float maximum_height)
    {
        int tiers = Random.Range(3, 5);

        vertices = new Vector3[24 * tiers + 12];
        triangles = new int[36 * tiers + 12];
        uv = new Vector2[24 * tiers + 12];

        float height = Random.Range(0.6f * maximum_height, maximum_height);
        float peakHeight = Random.Range(0.25f, 0.25f * height);
        height -= peakHeight;
        height /= 2;

        Vector3 lb = new Vector3(plot.plot_centre.x, 0, plot.plot_centre.y);
        Vector3 rt = new Vector3(plot.plot_dimensions.x + 0.1f, 0, plot.plot_dimensions.y + 0.1f);

        Block block;

        for (int i = 0; i < tiers; i++)
        {
            lb = new Vector3(lb.x + 0.1f, rt.y, lb.z + 0.1f);
            rt = new Vector3(rt.x - 0.1f, rt.y + height, rt.z - 0.1f);
            block = new Block(lb, rt, window_size);
            AddVertices(block.GetVertices(), block.GetTriangles(), block.GetUV());
            height /= 2;
        }

        GameObject new_building = new GameObject("tower building");
        CreateMesh(new_building, vertices, triangles, uv);

        new_building.transform.parent = plot.city_transform;
        new_building.transform.localPosition = plot.plot_centre;
    }

    void RoundBuilding(BuildingPlot plot, float height)
    {
        int slices = 36;//amount of slices that make up the cylinder
        int slices_skipped = Random.Range(0, 11);   //the amount of slices that will be skippped
        int index_skipped = Random.Range(0, slices / 2 - slices_skipped);   //the itteration at whichthey will be skipped

        vertices = new Vector3[(slices + 1 - slices_skipped * 2) * 4 + 50];
        triangles = new int[(slices - slices_skipped * 2) * 12 + 72];
        uv = new Vector2[(slices + 1 - slices_skipped * 2) * 4 + 50];

        //generation of the cylinder
        Cylinder cylinder = new Cylinder(plot, height, slices, slices_skipped, index_skipped);
        AddVertices(cylinder.GetVertices(), cylinder.GetTriangles(), cylinder.GetUV());

        ////creating the gameobject
        GameObject new_building = new GameObject("round building");
        CreateMesh(new_building, vertices, triangles, uv);

        new_building.transform.parent = plot.city_transform;
        new_building.transform.localPosition = plot.plot_centre;
    }

    void ModernBuilding(BuildingPlot plot, float maximum_height)
    {

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
            AddVertices(block.GetVertices(), block.GetTriangles(), block.GetUV());

            GameObject building_block = new GameObject("modern building");
            //CreateMesh(building_block, block.GetVertices(), block.GetTriangles(), block.GetUV());

            building_block.transform.parent = new_building.transform;

            maximum_height = height;
        }

        //create the shape

        CreateMesh(new_building, vertices, triangles, uv);

        new_building.transform.parent = plot.city_transform;
        new_building.transform.localPosition = plot.plot_centre;

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

    void CreateMesh(GameObject obj, Vector3[] vertices, int[] triangles, Vector2[] uv)
    {
        //adding coponents for rendering the mesh
        obj.AddComponent<MeshFilter>();
        obj.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh(); //a new mesh

        //setting the vertices, tris and UV's of the mesh to the calculated ones
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        obj.GetComponent<MeshFilter>().mesh = mesh;

        Material new_mat = Resources.Load("Red Mat", typeof(Material)) as Material;
        obj.GetComponent<MeshRenderer>().material = new_mat;

        obj.GetComponent<MeshFilter>().mesh.RecalculateNormals();
    }

}
