using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CityBlock : MonoBehaviour
{


    List<Vector2> mesh_points;

    List<int> numbers;
    List<GameObject> encompassing_roads;

    float min_x = 0;
    float max_x = 0;

    float min_z = 0;
    float max_z = 0;

    Vector3 centre_point;
    GameObject centre_object;
    GameObject city_block;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

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
        }


        mesh_points = points.ToList();

        CreatMesh();

    }

    void CreatMesh()
    {
        city_block = new GameObject("CityObject");
        MeshFilter mf = city_block.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer mr = city_block.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
       

        Triangulator tr = new Triangulator(mesh_points.ToArray());
        int[] indicies = tr.Triangulate();

        Vector3[] vertices = new Vector3[mesh_points.Count];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(mesh_points[i].x, 0, mesh_points[i].y);
        }


        Mesh m = new Mesh();
        m.vertices = vertices;
        m.triangles = indicies;
        m.RecalculateNormals();
        m.RecalculateBounds();

        mf.mesh = m;

        
        BoxCollider collider = city_block.AddComponent(typeof(BoxCollider)) as BoxCollider;
        collider.size = new Vector3(collider.size.x - GM_.Instance.config.road_values.road_segment_width, collider.size.y, collider.size.z - GM_.Instance.config.road_values.road_segment_width);

        
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
        DestroyImmediate(centre_object);
        DestroyImmediate(city_block);
    }

}
