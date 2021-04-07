using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class BlockGenerator : MonoBehaviour
{

    List<CityBlock> city_blocks;

    KeyValuePair<RoadSegment, RoadSegment> key;
    List<KeyValuePair<GameObject, GameObject>> pairs;
    List<KeyValuePair<int, int>> number_pairs;

    KeyValuePair<GameObject, GameObject> final_pair;

    public void Generate(List<GameObject> road_segments)
    {

        city_blocks = new List<CityBlock>();

        //cleanup segment that only attatch to one node
        //delete them from road list and also from connecting segments
        for (int i = road_segments.Count - 1; i > -1; i--)
        {
            if (road_segments[i].GetComponent<RoadSegment>().connected_points_all.Count <= 1)
            {
                foreach (GameObject temp in road_segments[i].GetComponent<RoadSegment>().connected_points_all)
                {
                    temp.GetComponent<RoadSegment>().RemoveObj(road_segments[i]);
                }

                road_segments.RemoveAt(i);
            }
        }

        FindFaces(road_segments);

        foreach (CityBlock block in city_blocks)
        {
            block.LotCreation();
        }

    }

    public List<CityBlock> GetCityBlocks()
    {
        return city_blocks;
    }
    
    private void FindFaces(List<GameObject> vertices)
    {

        Dictionary<Vector3, CityBlock> blocks = new Dictionary<Vector3, CityBlock>();

        List<GameObject> finished_verts = new List<GameObject>();

        foreach (GameObject vertex in vertices)//loop through each road
        {
            foreach (GameObject adj in vertex.GetComponent<RoadSegment>().connected_points_all) //for each road look at their connected points
            {
                List<GameObject> visit = new List<GameObject>();
                List<int> visit_num = new List<int>();

                GameObject point_A = vertex;
                GameObject point_B = adj;

                
                visit.Add(point_B);
                visit_num.Add(point_B.GetComponent<RoadSegment>().road_number);

                bool found_v = false;
                bool force_stop = false;

                while (!found_v && visit.Count < 40 && !force_stop)
                {

                    Vector2 vector_a = new Vector2(point_B.transform.position.x - point_A.transform.position.x, point_B.transform.position.z - point_A.transform.position.z);

                    List<GameObject> candidates = new List<GameObject>();

                    foreach (GameObject cand in point_B.GetComponent<RoadSegment>().connected_points_all)
                    {
                        //if the potential road is not where we jsut came form - then don't add it to the list
                        if (cand != point_A)
                        {
                            candidates.Add(cand);
                        }
                    }

                    if (candidates.Count > 0) //if there are potential candidates
                    {
                        GameObject temp = point_B;
                        
                        point_B = BestFaceCandidate(point_A ,point_B, candidates, vector_a);
                        point_A = temp; 
                    }
                    else
                    {
                        force_stop = true;
                        
                    }

                    if (point_B == vertex)
                    {
                        found_v = true;
                        visit.Add(point_A);
                        visit_num.Add(point_A.GetComponent<RoadSegment>().road_number);
                    }

                    visit.Add(point_B);
                    visit_num.Add(point_B.GetComponent<RoadSegment>().road_number);
                }

                bool ccw = IsCCW(visit);

                if(found_v && ccw)
                {
                    
                    CityBlock block = new CityBlock();
                    block.AddRoadNumbers(visit_num);
                    block.AddEncompassingRoads(visit);
                    block.CalcBlocksCentre();
                    block.CalcMeshPoints();

                    if (!blocks.ContainsKey(block.centre_object.transform.position))
                    {
                        blocks[block.centre_object.transform.position] = block;
                    }
                    else
                    {
                        block.CleanObj();
                        block = null;
                       
                    }


                }
                finished_verts.Add(vertex);
            }
            
        }

        city_blocks = blocks.Values.ToList();

    }

    GameObject BestFaceCandidate(GameObject point_A, GameObject point_B, List<GameObject> candidates, Vector2 vector_a)
    {

        if (candidates.Count > 1)
        {
            GameObject most_counter_clockwise = GetMostCC(point_A, point_B, candidates, vector_a);
            return most_counter_clockwise;
        }
        else if(candidates.Count == 1)
        {
            return candidates[0]; //this is the only possible bath we could follow - so move to it
        }

        return null;


    }

    GameObject GetMostCC(GameObject point_A, GameObject point_B, List<GameObject> candidates, Vector2 vector_a)
    {

        GameObject mostCC = null;
        float min_angle = float.MaxValue;

        GameObject collinear = null;

        GameObject leastClockwise = null;
        float max_angle = float.MinValue;

        for(int i = 0; i < candidates.Count; i++) //loop for every potential candidate
        {
            //calculate vector_b (point C - point B)
            Vector2 vector_b = new Vector2(candidates[i].transform.position.x - point_B.transform.position.x, candidates[i].transform.position.z - point_B.transform.position.z);
            Vector2 vector_c = new Vector2(candidates[i].transform.position.x - point_A.transform.position.x, candidates[i].transform.position.z - point_A.transform.position.z);
            
            float orientation = Mathf.Sign(CrossVec2(vector_a, vector_c));

            float angle = Mathf.Acos(Vector3.Dot(vector_a, vector_b) / (vector_a.magnitude * vector_b.magnitude));
            //float angle = Mathf.Rad2Deg * Vector2.Angle(vector_a.normalized, vector_b.normalized);

            if(orientation == -1 && max_angle < angle)
            {
                mostCC = candidates[i];
                max_angle = angle;
            }
            else if(orientation == 0)
            {
                collinear = candidates[i];
            }
            else if(orientation == 1 && min_angle > angle)
            {
                leastClockwise = candidates[i];
                min_angle = angle;
            }
        }

        if(mostCC != null)
        {
            return mostCC;
        }
        else if(collinear != null)
        {
            return collinear;
        }
        else
        {
            return leastClockwise;
        }

      
    }

    bool IsCCW(List<GameObject> potential_face)
    {
        bool ccw = false;

        if(potential_face.Count > 3)
        {
            float orient = 0;

            for(int i = 0; i < potential_face.Count; i++)
            {
                Vector3 pos1 = potential_face[i].transform.position;
                Vector3 pos2 = potential_face[(i + 1) % potential_face.Count].transform.position;
                orient += (pos2.x - pos1.x) * (pos2.z + pos1.z);
            }

            if(orient > 0)
            {
                ccw = true;
            }
        }

        return ccw;
    }

    float CrossVec2(Vector2 vec1, Vector2 vec2)
    {
        return (vec1.x * vec2.y) - (vec1.y * vec2.x);
    }
}