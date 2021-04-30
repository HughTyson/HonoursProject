using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class BlockGenerator : MonoBehaviour
{

    List<CityBlock> city_blocks;

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

        //find each face within the graph
        FindFaces(road_segments);


        foreach (CityBlock block in city_blocks)
        {
            block.LotCreation(); //generate the lots for each building
        }

    }

    public List<CityBlock> GetCityBlocks()
    {
        return city_blocks;
    }
    
    //this function is responsible for identifying all the city blocks between the road network
    private void FindFaces(List<GameObject> vertices)
    {

        //a dictionary to hold the city blocks
        Dictionary<Vector3, CityBlock> blocks = new Dictionary<Vector3, CityBlock>();   

        List<GameObject> finished_verts = new List<GameObject>();

        foreach (GameObject vertex in vertices)//loop through each road
        {
            foreach (GameObject adj in vertex.GetComponent<RoadSegment>().connected_points_all) //for each road look at their connected points
            {
                List<GameObject> visit = new List<GameObject>();    //create a new list of visisted roads
              

                GameObject point_A = vertex;    //the initial vertex we started from
                GameObject point_B = adj;   //the adjecent vertex we are looking at

                //store the adjacent point
                visit.Add(point_B); 
               

                bool found_v = false;
                bool force_stop = false;

                //loop as long as we hav not found the starting vertex
                while (!found_v && !force_stop)
                {
                    //calculate vector a (the vector between point a and point b
                    Vector2 vector_a = new Vector2(point_B.transform.position.x - point_A.transform.position.x, point_B.transform.position.z - point_A.transform.position.z);

                    //create the list of potential candidate direction
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
                        
                        point_B = BestFaceCandidate(point_A ,point_B, candidates, vector_a); //set next point to the best candicate
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
                        
                    }

                    visit.Add(point_B);

                }

                bool ccw = IsCCW(visit);

                //if has been found and is counter clockwise then create the city block ands store it - ensuring it has not been previously created
                if(found_v && ccw)
                {
                    
                    CityBlock block = new CityBlock();
                    block.AddEncompassingRoads(visit);
                    block.CalcBlocksCentre();
                    block.CalcMeshPoints();

                    if (!blocks.ContainsKey(block.centre_point))
                    {
                        blocks[block.centre_point] = block;
                    }
                    else
                    {
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

        if (candidates.Count > 1)   //if there is more than 1 candidate then check for the most counter clockwise
        {
            GameObject most_counter_clockwise = GetMostCC(point_A, point_B, candidates, vector_a);
            return most_counter_clockwise;
        }
        else if(candidates.Count == 1) //this is the only possible path we could follow - so move to it
        {
            return candidates[0]; 
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
         
            //get the orientation an the angle
            float orientation = Mathf.Sign(CrossVec2(vector_a, vector_c));
            float angle = Mathf.Acos(Vector3.Dot(vector_a, vector_b) / (vector_a.magnitude * vector_b.magnitude));


            
            if(orientation == -1 && max_angle < angle)
            {
                // if the angle negative and the max angle is less than the calcualted angle then this is the most counter clockwise
                mostCC = candidates[i];
                max_angle = angle;
            }
            else if(orientation == 0)
            {
                //if oritnetation is 0 then this road candicate is straight ahead
                collinear = candidates[i];
            }
            else if(orientation == 1 && min_angle > angle)
            {
                //if orientation is 1 then this piece is clockwise
                //if this angle is greate than the minimum angle then this piece is the most clockwise
                leastClockwise = candidates[i];
                min_angle = angle;
            }
        }

        //choose which value to return 
        if(mostCC != null)  
        {
            return mostCC;  //if there is a most counter closwise value then return it
        }
        else if(collinear != null)
        {
            return collinear; //else if there is a collinear road segment
        }
        else
        {
            return leastClockwise; //finaly the lst choice is to return the least counterclockwise segment
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