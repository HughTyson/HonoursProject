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

    List<int> numbers;
    List<GameObject> encompassing_roads;
    
    float min_x = 0;
    float max_x = 0;

    float min_z = 0;
    float max_z = 0;

    Vector3 centre_point;
    public GameObject centre_object;
    public GameObject obj;
    public CityBlockType city_block_type;

    public void AddRoadNumbers(List<int> incoming_numbers)
    {
        numbers = incoming_numbers;

        numbers.Sort();
    }

    public void AddEncompassingRoads(List<GameObject> objects)
    {
        encompassing_roads = objects;

    }

    public void CalcMeshPoints()
    {

        HashSet<Vector2> points = new HashSet<Vector2>();

        foreach (GameObject current_segment in encompassing_roads)
        {
            foreach (GameObject adj in current_segment.GetComponent<RoadSegment>().connected_points_all)
            {
                if (encompassing_roads.Contains(adj))
                {
                    if (IsPerpindicular(current_segment, adj))
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

        foreach (Vector3 p in points)
        {
            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.transform.position = new Vector3(p.x,0,p.y);
            corner_positions.Add(new Vector3(p.x, 0, p.y));
            point.transform.SetParent(centre_object.transform);
            
        }

        mesh_points = points.ToList();

        CreatMesh();
    }

    Vector3[] vertices;



    void CreatMesh()
    {

        obj = new GameObject("CityObject");
        obj.transform.SetParent(centre_object.transform);
        
        mesh_points = mesh_points.OrderBy(x => Mathf.Atan2(x.x - centre_object.transform.position.x, x.y- centre_object.transform.position.z)).ToList();

        Triangulator tr = new Triangulator(mesh_points.ToArray());
        int[] indicies = tr.Triangulate();

        vertices = new Vector3[mesh_points.Count];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(mesh_points[i].x, 0, mesh_points[i].y);
        }

        Mesh m = new Mesh();
        m.vertices = vertices;
        m.triangles = indicies;
        m.RecalculateNormals();
        m.RecalculateBounds();

        DefinePolygonOrder(m);
        
        centre_object.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        
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

    public List<int> GetRoadNumbers()
    {
        return numbers;
    }

    public void CalcBlocksCentre()
    {
        SetMinMax();

        centre_point = new Vector3((min_x + max_x) /2 , 0.1f, (min_z + max_z) / 2);

        centre_object = GameObject.CreatePrimitive(PrimitiveType.Cube);
        centre_object.transform.position = centre_point;

        centre_object.AddComponent<CityBlockInfoDisplay>();
        centre_object.GetComponent<CityBlockInfoDisplay>().SetInfo( encompassing_roads, numbers);
    }

    void SetMinMax()
    {

        bool x_set = false;
        bool z_set = false;

        foreach(GameObject obj in encompassing_roads)
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

            if(x_set && z_set)
            {
                break;
            }
        }

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

    public void CleanObj()
    {
        GameObject.DestroyImmediate(centre_object);
        GameObject.DestroyImmediate(obj);
        
    }


    //split into lots
    float minimum_area = 0; 
    List<Plot> accepeted_plots = new List<Plot>();
    public void LotCreation()
    {
        minimum_area = GM_.Instance.config.building_plot_values.minimum_area;
       
        Plot initital_plot = new Plot();
        initital_plot.edges_clockwise = lookup;
        initital_plot.edges_anitclockwise = reverse_lookup;

        initital_plot.cut_x_z = false;

        initital_plot.type = CityBlockType.BUILDING;
        Vector3 offset = new Vector3();

        //scaling each vertex in to make a pavement
        for (int i = 0; i < vertices.Length; i++)
        {

            Vector3 dir = new Vector3(vertices[i].x - centre_object.transform.position.x, 0, vertices[i].z - centre_object.transform.position.z).normalized;

            offset.x = dir.x < 0 ? -0.5f : 0.5f;
            offset.z = dir.z < 0 ? -0.5f : 0.5f;

            vertices[i] = vertices[i] - (offset * GM_.Instance.config.road_values.road_segment_width);
        }

        initital_plot.vertexes = vertices.ToList();

        foreach(KeyValuePair<int, int> value in lookup)
        {
            if(Vector3.Distance(vertices[value.Key], vertices[value.Value]) < GM_.Instance.config.building_plot_values.minimum_building_dimension)
            {
                PlazaGenerator.Generate(initital_plot);
                return;
            }
        }

        
        

        Queue<Plot> subdivide_queue = new Queue<Plot>();
        subdivide_queue.Enqueue(initital_plot);

        while(subdivide_queue.Count > 0)
        {
            Plot most_recent = subdivide_queue.Dequeue();

            List<Plot> new_plots = SubdividePlot(most_recent);

            if(new_plots.Count != 2)
            {
                accepeted_plots.Add(most_recent);
            }
            else
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
            first_values = GetFirst(true, plot.vertexes, plot.edges_clockwise);

            float midpoint = (plot.vertexes[first_values.Key].z + plot.vertexes[first_values.Value].z) / 2;

            Plot p = GetPlotZAxis(plot.edges_clockwise, plot.vertexes, midpoint, first_values);

            if(p != null)
            {
                potential_new_plots.Add(p);
            }

            first_values = GetFirst(true, plot.vertexes, plot.edges_anitclockwise);

            p = GetPlotZAxis(plot.edges_anitclockwise, plot.vertexes, midpoint, first_values);

            if (p != null)
            {
                potential_new_plots.Add(p);
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

        Plot temp_plot = new Plot();
        temp_plot.type = CityBlockType.BUILDING;
        temp_plot.cut_x_z = false;
        bool colliding_vert_found = false;

        int start_vertex = first_values.Value;

        temp_plot.vertexes.Add(new Vector3(midpoint, 0.1f, vertexes[first_values.Value].z));
        //first_plot.vertexes.Add(plot.vertexes[start_vertex]);
        temp_plot.edges_clockwise.Add(0, 1);
        temp_plot.edges_anitclockwise.Add(1, 0);

        int added_value = 1;
        while (!colliding_vert_found)
        {
            int next_vertex = edges[start_vertex];

            if (IsBetween(midpoint, vertexes[start_vertex].x, vertexes[next_vertex].x))
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
                temp_plot.edges_clockwise.Add(added_value, added_value + 1);
                temp_plot.edges_anitclockwise.Add(added_value + 1, added_value);

                temp_plot.vertexes.Add(vertexes[start_vertex]);
                temp_plot.vertexes.Add(vertexes[next_vertex]);

                added_value += 1;
                start_vertex = next_vertex;
            }
        }

        if (PassesCriteria(temp_plot))
        {
            WhatAxis(ref temp_plot);
            return temp_plot;
        }

        return null;
    }

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

        foreach(KeyValuePair<int, int> it in edges)
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
        if (!TouchesRoad(new_plot.vertexes))
        {
            return false;
        }

        if (GetArea(new_plot.vertexes) < minimum_area)
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

        foreach(Vector3 vertice in new_vertices)
        {
            foreach(Vector3 outer_vertices in vertices)
            {
                if(vertice.x == outer_vertices.x || vertice.z == outer_vertices.z)
                {
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

    //function looks what side is larger is larger
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

        if(x_axis > z_axis)
        {
            if(x_axis > z_axis * 1.5f)
            {
                new_plot.cut_x_z = false;
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

public class Plot
{
    public Dictionary<int, int> edges_clockwise = new Dictionary<int, int>();
    public Dictionary<int, int> edges_anitclockwise = new Dictionary<int, int>();
    public List<Vector3> vertexes = new List<Vector3>();

    public bool cut_x_z = false;
    public CityBlockType type;
}