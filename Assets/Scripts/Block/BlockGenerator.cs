using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

enum Direction
{UP, LEFT, RIGHT, DOWN }

enum Status
{
    SEARCHING, NOT_FOUND, FOUND
}


public class BlockGenerator : MonoBehaviour
{


    List<Direction> directions = new List<Direction> { Direction.UP, Direction.LEFT, Direction.DOWN, Direction.RIGHT };
    int min_direction = 0;
    int max_direction = 4;


    List<CityBlock> city_blocks;

    Vector3 starting_pos;

    GameObject search_for;


    int direction_counter;

    private void Start()
    {

    }

    Stack<GameObject> stack_gameobject = new Stack<GameObject>();
    Stack<int> stack_numbers = new Stack<int>();
    bool[] visited;

    public List<CityBlock> Generate(List<Intersection> intersection_points, List<GameObject> road_segments)
    {

        //cleanup segment that only attatch to one node
        //delete them from road list and also from connecting segments

        //for (int i = road_segments.Count - 1; i > -1; i--)
        //{
        //    if (road_segments[i].GetComponent<RoadSegment>().connected_points_all.Count <= 1)
        //    {
        //        foreach (GameObject temp in road_segments[i].GetComponent<RoadSegment>().connected_points_all)
        //        {
        //            temp.GetComponent<RoadSegment>().RemoveObj(road_segments[i]);
        //        }

        //        road_segments.RemoveAt(i);
        //    }
        //}


        city_blocks = new List<CityBlock>();

        stack_gameobject.Clear();
        stack_numbers.Clear();

        // DFS(road_segments[0].GetComponent<RoadSegment>(), road_segments[0].GetComponent<RoadSegment>());


        //foreach (Intersection intersection in intersection_points)
        //{
        //    BFS(intersection.GetParentRoad());
        //}

        foreach (GameObject intersection in road_segments)
        {
            if (intersection.GetComponent<RoadSegment>().road_number == 150)
            {
                Debug.Log("Hello");
            }
            if(intersection.GetComponent<RoadSegment>().direction.x != 0)
                BFS(intersection);

            foreach (GameObject obj in road_segments)
            {
                obj.GetComponent<RoadSegment>().dfs.visited = false;
            }
        }

        //foreach(CityBlock c in city_blocks)
        //{
        //    c.CalcMeshPoints();
        //}


        Debug.Log(city_blocks.Count);

        return city_blocks;
    }

    
    void DFS(RoadSegment road_segment, RoadSegment last_obj)
    {

        road_segment.dfs.visited = true;
        stack_numbers.Push(road_segment.road_number);
        stack_gameobject.Push(road_segment.GetObj());

        foreach (GameObject obj in road_segment.connected_points_all)
        {
            if (!obj.GetComponent<RoadSegment>().dfs.visited && obj.transform != last_obj.transform)
            {
                DFS(obj.GetComponent<RoadSegment>(), road_segment);
            }
            else
            {

                if (obj.transform != last_obj.transform)
                {
                    int[] copy_numbers = stack_numbers.ToArray();
                    GameObject[] copy_gameobjects = stack_gameobject.ToArray();

                    if (copy_numbers.Length > 4)
                    {
                        List<int> final_numbers = new List<int>();
                        List<GameObject> final_pieces = new List<GameObject>();
                        final_numbers.Add(obj.GetComponent<RoadSegment>().road_number);
                        final_pieces.Add(obj);

                        for (int i = 0; i < copy_numbers.Length; i++)
                        {
                            if (copy_numbers[i] != obj.GetComponent<RoadSegment>().road_number)
                            {
                                final_numbers.Add(copy_numbers[i]);
                                final_pieces.Add(copy_gameobjects[i]);
                            }
                            else
                            {
                                break;
                            }
                        }

                        CityBlock block = new CityBlock();
                        block.AddRoadNumbers(final_numbers);
                        city_blocks.Add(block);
                    }

                    Debug.Log(copy_numbers.Length);
                }

            }
        }
        //road_segment.dfs.visited = false;
        stack_numbers.Pop();
    }



    KeyValuePair<RoadSegment, RoadSegment> key;
    List<KeyValuePair<GameObject, GameObject>> pairs;
    List<KeyValuePair<int, int>> number_pairs;

    KeyValuePair<GameObject, GameObject> final_pair;
    

    void BFS(GameObject root_obj)
    {
        List<RoadSegment> visited_segments = new List<RoadSegment>();

        foreach (GameObject connect_obj in root_obj.GetComponent<RoadSegment>().connected_points_all)
        {

            bool obj_found = false;
            visited_segments.Clear();
            Queue<GameObject> queue = new Queue<GameObject>();


            root_obj.GetComponent<RoadSegment>().dfs.visited = true;
            queue.Enqueue(root_obj);

            
            pairs = new List<KeyValuePair<GameObject, GameObject>>();
            number_pairs = new List<KeyValuePair<int, int>>();


            while (queue.Count >-1 && obj_found == false)
            {

                int num;
                GameObject current_obj;
                if (queue.Count != 0)
                {
                    current_obj = queue.Dequeue();
                }
                else
                {
                    break;
                }

                
                List<GameObject> ordered_list = current_obj.GetComponent<RoadSegment>().connected_points_all.OrderBy(x => Mathf.Atan2(x.transform.position.z - current_obj.transform.position.z, x.transform.position.x - current_obj.transform.position.x)).ToList();
                ordered_list.Reverse();

                foreach (GameObject child_obj in ordered_list/*current_obj.GetComponent<RoadSegment>().connected_points_all*/)
                {

                    if (current_obj == root_obj)
                    {
                        if (child_obj == connect_obj)
                        {
                            continue;
                        }
                    }

                    if (child_obj != connect_obj)
                    {
                        if (!child_obj.GetComponent<RoadSegment>().dfs.visited)
                        {
                            current_obj.GetComponent<RoadSegment>().dfs.visited = true;
                            queue.Enqueue(child_obj);


                            visited_segments.Add(child_obj.GetComponent<RoadSegment>());
                            pairs.Add(new KeyValuePair<GameObject, GameObject>(current_obj, child_obj));
                            number_pairs.Add(new KeyValuePair<int, int>(current_obj.GetComponent<RoadSegment>().road_number, child_obj.GetComponent<RoadSegment>().road_number));
                        }
                    }
                    else
                    {

                        final_pair = new KeyValuePair<GameObject, GameObject>(current_obj, child_obj);
                        CreateBlock(final_pair, ref obj_found);


                        break;
                        
                        
                    }
                }
            }

            foreach(RoadSegment a in visited_segments)
            {

                a.dfs.visited = false;
            }

            //if(obj_found)
            //{
            //    break;
            //}

        }


        return;


    }

    void CreateBlock(KeyValuePair<GameObject, GameObject> final_pair, ref bool found)
    {
        List<GameObject> object_list = new List<GameObject>();
        List<int> number_list = new List<int>();

        object_list.Add(final_pair.Value);
        object_list.Add(final_pair.Key);

        number_list.Add(final_pair.Value.GetComponent<RoadSegment>().road_number);
        number_list.Add(final_pair.Key.GetComponent<RoadSegment>().road_number);

        GameObject search_for = final_pair.Key;

        for (int i = pairs.Count - 1; i > -1; i--)
        {
            if(pairs[i].Value == search_for)
            {
                object_list.Add(pairs[i].Key);
                
                search_for = pairs[i].Key;
                number_list.Add(search_for.GetComponent<RoadSegment>().road_number);
            }
        }


        number_list.Sort();

        if(number_list[0] == 4)
        {
            Debug.Log("HEllo");
        }

        if(!DuplicateRoad(number_list, object_list))
        {
            found = true;
            CityBlock block = new CityBlock();
            block.AddRoadNumbers(number_list);
            block.AddEncompassingRoads(object_list);
            block.CalcBlocksCentre();
            block.CalcMeshPoints();
            city_blocks.Add(block);
        }
    }

    bool DuplicateRoad(List<int> new_numbers, List<GameObject> obj)
    {
        bool duplicate = false;

        if(new_numbers[0] == 119)
        {
            Debug.Log("Hello");
        }

        foreach(CityBlock block in city_blocks)
        {

            List<int> blocks_numbers = block.GetRoadNumbers();
            List<int> common_items = new List<int>();
            
            foreach(int element in blocks_numbers.Intersect(new_numbers))
            {
                common_items.Add(element);
            }

            if(common_items.Count != 0)
            {
                float percentage_similarity = 0;
                percentage_similarity = common_items.Count / (float) blocks_numbers.Count;

                if(new_numbers.Count >= blocks_numbers.Count)
                {
                    if(percentage_similarity > 0.62)
                    {

                        //condition
                        duplicate = true;
                        break;
                    }
                }
                else
                {
                    if (percentage_similarity > 0.51)
                    {
                        block.AddEncompassingRoads(obj);
                        block.AddRoadNumbers(new_numbers);
                        block.CleanObj();
                        block.CalcBlocksCentre();
                        block.CalcMeshPoints();
                        duplicate = true;
                        break;
                    }
                }
            }
        }

        return duplicate;
    }


}
