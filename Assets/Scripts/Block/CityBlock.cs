using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum CityBlockType
{
    BUILDING, PLAZA
}

public class CityBlock
{


    List<Vector2> mesh_points;
    public List<Vector3> corner_positions = new List<Vector3>();


    List<GameObject> encompassing_roads;
    
    float min_x = 0;
    float max_x = 0;

    float min_z = 0;
    float max_z = 0;

    public Vector3 centre_point;


    public CityBlockType city_block_type;

    public void AddEncompassingRoads(List<GameObject> objects)
    {
        encompassing_roads = objects;

    }

    public void CalcMeshPoints()
    {

        HashSet<Vector2> points = new HashSet<Vector2>();

        foreach (GameObject current_segment in encompassing_roads)  //loop for all roads which surround the city block
        {
            foreach (GameObject adj in current_segment.GetComponent<RoadSegment>().connected_points_all) //loop for all sconnected roads to this road segment 
            {
                if (encompassing_roads.Contains(adj))   //if this adj segment is included in the encompassing roads
                {
                    if (IsPerpindicular(current_segment, adj))  //check if this road is perpindicular to the current road - if this is true then a mesh point is created at the intersection of the two roads and added to the list
                    {

                        Vector3 new_point = Vector3.zero;

                        if (current_segment.GetComponent<RoadSegment>().direction.x != 0)
                        {
                            new_point = new Vector2(adj.transform.position.x, current_segment.transform.position.z);
                        }
                        else
                        {
                            new_point = new Vector2(current_segment.transform.position.x, adj.transform.position.z);
                        }

                        points.Add(new_point);
                    }
                }
            }
        }

        //these are translated into 3D space
        foreach (Vector3 p in points)
        {
            corner_positions.Add(new Vector3(p.x, 0, p.y));
        }

        mesh_points = points.ToList();  //the mesh points array is set


        CreatMesh();
    }

    Vector3[] vertices;



    void CreatMesh()
    {
        //if there are 4 mesh points (4 corners)
        if(mesh_points.Count == 4)
            mesh_points = mesh_points.OrderBy(x => Mathf.Atan2(x.x - centre_point.x, x.y - centre_point.z)).ToList(); //order these points ub clockwise order around the centre point
        

        Triangulator tr = new Triangulator(mesh_points.ToArray());  //use the traingulator to help create the mesh 
        int[] indicies = tr.Triangulate();  //get the indices from the triangulator 

        vertices = new Vector3[mesh_points.Count]; //set the vertices array length

        //set al verticies
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(mesh_points[i].x, 0, mesh_points[i].y);
        }

        //create the mesh
        Mesh m = new Mesh();
        m.vertices = vertices;
        m.triangles = indicies;
        m.RecalculateNormals();
        m.RecalculateBounds();

        //define the order of the meshpoints in the polygon
        DefinePolygonOrder(m);
        
        
        
    }

    Dictionary<int, int> lookup = new Dictionary<int, int>();
    Dictionary<int, int> reverse_lookup = new Dictionary<int, int>();
    void DefinePolygonOrder(Mesh mesh)
    {
        // Get triangles and vertices from mesh
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;

        // Get just the outer edges from the mesh's triangles (ignore or remove any shared edges)
        Dictionary<string, KeyValuePair<int, int>> edges = new Dictionary<string, KeyValuePair<int, int>>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            for (int e = 0; e < 3; e++)
            {
                int vert1 = triangles[i + e];
                int vert2 = triangles[i + e + 1 > i + 2 ? i : i + e + 1];
                string edge = Mathf.Min(vert1, vert2) + ":" + Mathf.Max(vert1, vert2);
                if (edges.ContainsKey(edge))
                {
                    edges.Remove(edge);
                }
                else
                {
                    edges.Add(edge, new KeyValuePair<int, int>(vert1, vert2));
                }
            }
        }

        // Create edge lookup (Key is first vertex, Value is second vertex, of each edge)
        lookup = new Dictionary<int, int>();
        reverse_lookup = new Dictionary<int, int>();

        foreach (KeyValuePair<int, int> edge in edges.Values)
        {
            if (lookup.ContainsKey(edge.Key) == false)
            {
                lookup.Add(edge.Key, edge.Value);
                if (!reverse_lookup.ContainsKey(edge.Value))
                    reverse_lookup.Add(edge.Value, edge.Key);
            }
        }
    }

    

    bool IsPerpindicular(GameObject current_segment, GameObject adj)
    {
        if(Vector3.Cross(current_segment.GetComponent<RoadSegment>().direction, adj.GetComponent<RoadSegment>().direction).magnitude == 0)
        {
            return false;
        }

        return true;
    }

    public void CalcBlocksCentre()
    {
        //gets the minimum and maximum points of the city block
        SetMinMax();

        //claculate the centre point of the city block
        centre_point = new Vector3((min_x + max_x) /2 , 0.1f, (min_z + max_z) / 2);

    }

    void SetMinMax()
    {

        bool x_set = false;
        bool z_set = false;

        //loop for all encompassing roads and set the minimum and maximum x and z positions
        foreach (GameObject obj in encompassing_roads)
        {
            if (obj.GetComponent<RoadSegment>().direction.z != 0 && x_set == false)
            {
                x_set = true;
                min_x = obj.transform.position.x;
                max_x = obj.transform.position.x;
            }

            if (obj.GetComponent<RoadSegment>().direction.x != 0 && z_set == false)
            {
                z_set = true;
                min_z = obj.transform.position.z;
                max_z = obj.transform.position.z;
            }

            if (x_set && z_set)
            {
                break;
            }
        }

        //loop for all surrounding roads and get the minimum and maximum x and z 
        foreach (GameObject obj in encompassing_roads)
        {

            if (obj.GetComponent<RoadSegment>().direction.z != 0)
            {
                if (obj.transform.position.x < min_x) min_x = obj.transform.position.x;

                if (obj.transform.position.x > max_x) max_x = obj.transform.position.x;
            }

            if (obj.GetComponent<RoadSegment>().direction.x != 0)
            {
                if (obj.transform.position.z < min_z) min_z = obj.transform.position.z;

                if (obj.transform.position.z > max_z) max_z = obj.transform.position.z;
            }
        }


    }


    
    float minimum_area = 0; 
    List<Plot> accepeted_plots = new List<Plot>();



    //this function is responsible for splitting a city block into smaller plots that buildings can occupy
    public void LotCreation()
    {
        minimum_area = GM_.Instance.config.building_plot_values.minimum_area; //get the minimum area a plot can be

        //initially set the whole block as a plot
        Plot initital_plot = new Plot();
        initital_plot.edges_clockwise = lookup;
        initital_plot.edges_anitclockwise = reverse_lookup;

        initital_plot.cut_x_z = false;  //initially cut in the z axis, this should probable be changed to be random

        initital_plot.type = CityBlockType.BUILDING;
        Vector3 offset = new Vector3();

        //scaling each vertex in to ensure it doesnt overlap with the road
        for (int i = 0; i < vertices.Length; i++)
        {

            Vector3 dir = new Vector3(vertices[i].x - centre_point.x, 0, vertices[i].z - centre_point.z).normalized;

            offset.x = dir.x < 0 ? -0.5f : 0.5f;
            offset.z = dir.z < 0 ? -0.5f : 0.5f;

            vertices[i] = vertices[i] - (offset * GM_.Instance.config.road_values.road_segment_width);
        }
        
        initital_plot.vertexes = vertices.ToList();

        //if there are more than 4 vertexes then make the block a plaza
        if (lookup.Count > 4)
        {
            PlazaGenerator.Generate(initital_plot);
            return;
        }

        //look through each edge and check the distance - if a edge is too small make it a plaza
        foreach(KeyValuePair<int, int> value in lookup)
        {
            if(Vector3.Distance(vertices[value.Key], vertices[value.Value]) < GM_.Instance.config.building_plot_values.minimum_building_dimension)
            {
                PlazaGenerator.Generate(initital_plot);
                return;
            }
        }

        //create a queue of plaots that will be subdivided
        Queue<Plot> subdivide_queue = new Queue<Plot>();
        subdivide_queue.Enqueue(initital_plot); //queue the initial plot

        while(subdivide_queue.Count > 0) //loop until there are no more plots left
        {
            Plot most_recent = subdivide_queue.Dequeue();   //get the first plot

            List<Plot> new_plots = SubdividePlot(most_recent);  //return a list of potential new plots

            if(new_plots.Count != 2)    //if the plot has not been split, then add the most recent plot to the list of accepted plots
            {
                accepeted_plots.Add(most_recent);
            }
            else //if there plots then add them to the queue to be evaluated
            {
                foreach(Plot potential in new_plots)
                {
                    subdivide_queue.Enqueue(potential);
                }
            }
        }
    }

    List<Plot> SubdividePlot(Plot plot)
    {
        List<Plot> potential_new_plots = new List<Plot>();
        KeyValuePair<int, int> first_values;


        if (!plot.cut_x_z) //cut in the z axis
        {
            first_values = GetFirst(true, plot.vertexes, plot.edges_clockwise); //get the indicis that make up the first edge, which is in the x-axis, goign clockwise

            float midpoint = (plot.vertexes[first_values.Key].z + plot.vertexes[first_values.Value].z) / 2; // calulate the midpoint of this edge

            Plot p = GetPlotZAxis(plot.edges_clockwise, plot.vertexes, midpoint, first_values); //get a plot looking around clockwise

            if(p != null)   //if this is not null then a plot exists, hence store it
            {
                potential_new_plots.Add(p);
            }

            first_values = GetFirst(true, plot.vertexes, plot.edges_anitclockwise); //get the indicis that make up the first edge, which is in the x-axis, goign counterclockwise

            midpoint = (plot.vertexes[first_values.Key].z + plot.vertexes[first_values.Value].z) / 2; // calulate the midpoint of this edge

            p = GetPlotZAxis(plot.edges_anitclockwise, plot.vertexes, midpoint, first_values); //get a plot looking around clockwise

            if (p != null)
            {
                potential_new_plots.Add(p); //store plot if it exists
            }
        }
        else
        {
            first_values = GetFirst(false, plot.vertexes, plot.edges_clockwise);

            float midpoint = (plot.vertexes[first_values.Key].x + plot.vertexes[first_values.Value].x) / 2;

            Plot p = GetPlotXAxis(plot.edges_clockwise, plot.vertexes, midpoint, first_values);

            if (p != null)
            {
                potential_new_plots.Add(p);
            }

            first_values = GetFirst(false, plot.vertexes, plot.edges_anitclockwise);

            midpoint = (plot.vertexes[first_values.Key].x + plot.vertexes[first_values.Value].x) / 2;

            p = GetPlotXAxis(plot.edges_anitclockwise, plot.vertexes, midpoint, first_values);

            if (p != null)
            {
                potential_new_plots.Add(p);
            }
        }

        return potential_new_plots;
    }

    Plot GetPlotXAxis(Dictionary<int, int> edges, List<Vector3> vertexes, float midpoint, KeyValuePair<int, int> first_values)
    {
        //create a temporary plot
        Plot temp_plot = new Plot();
        temp_plot.type = CityBlockType.BUILDING;
        temp_plot.cut_x_z = false; //as we have cut in the x-axis to generate this plot next time we will most likely cut in the z (potentialy changed later)
        bool colliding_vert_found = false;

        int start_vertex = first_values.Value;  //get the starting vertex

        temp_plot.vertexes.Add(new Vector3(midpoint, 0.1f, vertexes[first_values.Value].z));    //add a new vertex going from the midpoint that is passed in, to the value of the first edge

        temp_plot.edges_clockwise.Add(0, 1);    //set up the indices for the clockwise and anticlockwise
        temp_plot.edges_anitclockwise.Add(1, 0);

        int added_value = 1;
        while (!colliding_vert_found)   //keep going until we have found the first vertex
        {
            int next_vertex = edges[start_vertex];  //get next vertex

            if (IsBetween(midpoint, vertexes[start_vertex].x, vertexes[next_vertex].x)) //check if next vertex falls between the range
            {
                //midpoint is between these two vertices, hence found

                
                temp_plot.edges_clockwise.Add(added_value, added_value + 1);
                temp_plot.edges_anitclockwise.Add(added_value + 1, added_value);

                temp_plot.vertexes.Add(new Vector3(midpoint, 0.1f, vertexes[start_vertex].z));

                temp_plot.edges_clockwise.Add(added_value + 1, 0);
                temp_plot.edges_anitclockwise.Add(0, added_value + 1);

                colliding_vert_found = true;
            }
            else
            {

                //is not between the range, so add the full edge to the list

                temp_plot.edges_clockwise.Add(added_value, added_value + 1);
                temp_plot.edges_anitclockwise.Add(added_value + 1, added_value);

                temp_plot.vertexes.Add(vertexes[start_vertex]);
                temp_plot.vertexes.Add(vertexes[next_vertex]);

                added_value += 1;
                start_vertex = next_vertex;
            }
        }

        //once we have a potentail plot check if it passes the criteria
        if (PassesCriteria(temp_plot))
        {
            WhatAxis(ref temp_plot);    //if it passes the criteria then determine which axis it shoudl be cut on
            return temp_plot;
        }

        return null;
    }


    //fucntion completes the same as abover, however it is being cut on the z axis
    Plot GetPlotZAxis(Dictionary<int, int> edges, List<Vector3> vertexes, float midpoint, KeyValuePair<int, int> first_values)
    {
        Plot temp_plot = new Plot();
        temp_plot.type = CityBlockType.BUILDING;
        temp_plot.cut_x_z = true;
        bool colliding_vert_found = false;

        int start_vertex = first_values.Value;

        temp_plot.vertexes.Add(new Vector3(vertexes[first_values.Value].x, 0.1f, midpoint));
        temp_plot.edges_clockwise.Add(0, 1);
        temp_plot.edges_anitclockwise.Add(1, 0);

        int added_value = 1;
        while (!colliding_vert_found)
        {
            int next_vertex = edges[start_vertex];

            if (IsBetween(midpoint, vertexes[start_vertex].z, vertexes[next_vertex].z))
            {
                //midpoint is between these two vertices, hence found

                temp_plot.edges_clockwise.Add(added_value, added_value + 1);
                temp_plot.edges_anitclockwise.Add(added_value + 1, added_value);

                temp_plot.vertexes.Add(new Vector3(vertexes[start_vertex].x, 0.1f, midpoint));

                temp_plot.edges_clockwise.Add(added_value + 1, 0);
                temp_plot.edges_anitclockwise.Add(0, added_value + 1);

                colliding_vert_found = true;
            }
            else
            {
                temp_plot.edges_clockwise.Add(added_value, added_value + 1);
                temp_plot.edges_anitclockwise.Add(added_value + 1, added_value);

                temp_plot.vertexes.Add(vertexes[start_vertex]);
                temp_plot.vertexes.Add(vertexes[next_vertex]);

                added_value += 1;
                start_vertex = next_vertex;
            }
        }

        if(PassesCriteria(temp_plot))
        {
            //if the block passes criteria then decide on what axis the segment shoudl be cut (x or z)

            WhatAxis(ref temp_plot);
            return temp_plot;
        }

        return null;
    }

    KeyValuePair<int, int> GetFirst(bool x_or_z, List<Vector3> vertexes, Dictionary<int,int> edges)
    {
        KeyValuePair<int, int> return_value;

        foreach(KeyValuePair<int, int> it in edges) //loop through all edges 
        {
            if(!x_or_z) //check for z being the same
            {
                if(vertexes[it.Key].z == vertexes[it.Value].z) 
                {
                    return it;
                }
            }
            else
            {
                if (vertexes[it.Key].x == vertexes[it.Value].x)
                {
                    return it;
                }
            }
        }

        return return_value;

    }
    bool PassesCriteria(Plot new_plot)
    {
        if (!TouchesRoad(new_plot.vertexes)) //ensure the building woould have a t least one edge which touches the roads
        {
            return false;
        }

        if (GetArea(new_plot.vertexes) < minimum_area)  //ensure the buildings footprint is larger than a minimum area
        {
            return false;
        }

        return true;

    }

    float GetArea(List<Vector3> vertexes)
    {
        float area = 0;

        for(int i = 0; i < vertexes.Count; i++)
        {
            if (i != vertexes.Count - 1)
            {
                float mulA = vertexes[i].x * vertexes[i + 1].z;
                float mulB = vertexes[i + 1].x * vertexes[i].z;
                area = area + (mulA - mulB);
            }
            else
            {
                float mulA = vertexes[i].x * vertexes[0].z;
                float mulB = vertexes[0].x * vertexes[i].z;
                area = area + (mulA - mulB);
            }
        }

        area = Mathf.Abs(area * 0.5f);

        return area;
    }

    bool TouchesRoad(List<Vector3> new_vertices)
    {

        bool touch_found = false;

        foreach(Vector3 vertice in new_vertices)    //loop for all vertices in the new list of vertices
        {
            foreach(Vector3 outer_vertices in vertices) //loop through all vertices in the initial city block
            {
                if(vertice.x == outer_vertices.x || vertice.z == outer_vertices.z)  //determine if new vertice lies on an old vertice
                {
                    //if they do touch then return
                    touch_found = true;
                    break;
                }
            }

            if(touch_found)
            {
                break;
            }
        }

        return touch_found;
    }

    //function looks what side is larger
    void  WhatAxis(ref Plot new_plot)
    {

        float x_axis, z_axis;
        
        if(new_plot.cut_x_z) //latest cut is true - therefore we have just on the x-axis
        {
            x_axis = Vector3.Distance(new_plot.vertexes[0], new_plot.vertexes[1]);
            z_axis = Vector3.Distance(new_plot.vertexes[1], new_plot.vertexes[2]);
        }
        else//latest cut is true - therefore we have just on the z-axis
        {
            z_axis = Vector3.Distance(new_plot.vertexes[0], new_plot.vertexes[1]);
            x_axis = Vector3.Distance(new_plot.vertexes[1], new_plot.vertexes[2]);
        }

        //if the x is greater than the z
        if(x_axis > z_axis)
        {
            //if the x is greater than the z multiplied by 1.5
            if(x_axis > z_axis * 1.5f)
            {
                new_plot.cut_x_z = false;   //cut in the z
            }
        }
        else
        {
            if (z_axis > x_axis * 1.5f)
            {
                new_plot.cut_x_z = true;
            }
        }
    }

    public bool IsBetween(float test_val, float bound1, float bound2)
    {
        return (test_val >= Mathf.Min(bound1, bound2) && test_val <= Mathf.Max(bound1, bound2));
    }

    public List<Plot> GetLots()
    {
        return accepeted_plots;
    }
}

//class to store plot information before it is turned into a building plot
public class Plot
{
    public Dictionary<int, int> edges_clockwise = new Dictionary<int, int>();
    public Dictionary<int, int> edges_anitclockwise = new Dictionary<int, int>();
    public List<Vector3> vertexes = new List<Vector3>();

    public bool cut_x_z = false;
    public CityBlockType type;
}