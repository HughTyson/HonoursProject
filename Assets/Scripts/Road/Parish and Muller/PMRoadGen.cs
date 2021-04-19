using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PMRoadGen : GenericRoadGen
{

    int[] angle = new int[] { 0, 90, 180, 270 };
    Vector3[] directions = new Vector3[] { new Vector3(0, 0, 1), new Vector3(0, 0, -1), new Vector3(1, 0, 0), new Vector3(-1, 0, 0) };

    Queue<GameObject> road_segment_queue;
    

    public List<GameObject> Generate()
    {

        InitValues();   //setup vlaues used in road generation
        InitHandlers(); //initilise managers used in road generation

        //define lists used to store new segments and accepted segments
        accepted_segments = new List<GameObject>();
        road_segment_queue = new Queue<GameObject>();
      

        //create the initial road segment and add it to the 
        GameObject obj = Instantiate(road_segment);
        obj.GetComponent<RoadSegment>().RoadSegmentInit(new Vector3(0, 0, 0), new Vector3(min_segment_length, 0.1f, segment_width), 0, obj.transform, new Vector3(1,0,0), obj);

        road_segment_queue.Enqueue(obj); //queue the first object

        //generate roads until max amount of roads have been created or max amount of itterations has passed or there are no segments in the queue
        while (accepted_segments.Count < max_roads)
        {
            GameObject working_obj;
            RoadSegment segment;

            //take an object form the queue
            if (road_segment_queue.Count != 0)
            {
                working_obj = road_segment_queue.Dequeue();
                segment = working_obj.GetComponent<RoadSegment>();
            }
            else
            {
                break;
            }

            //check if this object passes all local contraints
            bool accepted = LocalConstraints(working_obj);

            if (accepted) //if it passes the constraints then addd it to the list of accepted segments, if not then immeditely delte the object
            {

                working_obj.GetComponent<RoadSegment>().RoadNum(accepted_segments.Count);
                accepted_segments.Add(working_obj);
                GlobalConstraints(segment);

                
            }
            else
            {
                DestroyImmediate(working_obj);  //object is not part of the system so destory immediatley
            }
        }

        foreach (GameObject i in road_segment_queue) //delete all roads that were not accepted
        {
            DestroyImmediate(i);
        }

        road_segment_queue.Clear();

        //make sure all collision boxes encompas full shape
        for (int i = 0; i < accepted_segments.Count; i++)
        {
            accepted_segments[i].GetComponent<RoadSegment>().ReScaleCollisionBox();
            accepted_segments[i].GetComponent<RoadSegment>().CleanUpConnection();
        }

        //create intersections
        CreateIntersections();

        //  CleanUp();

        return accepted_segments;
    }



    bool LocalConstraints(GameObject working_obj)
    {
        RoadSegment working_segment = working_obj.GetComponent<RoadSegment>();

        foreach (GameObject accepted_segment in accepted_segments) //check againts every road segments that have already been created
        {
           
            Physics.SyncTransforms();

            if(use_city_limits) //check if it is within a user defined boundry
            {
                if (Mathf.Abs(accepted_segment.transform.position.x) > GM_.Instance.config.city_limits_x || Mathf.Abs(accepted_segment.transform.position.z) > GM_.Instance.config.city_limits_z)
                {
                    return false;
                }
            }

            //make sure this new road doesn't intersect iwth other roads
            if (working_obj.GetComponent<BoxCollider>().bounds.Intersects(accepted_segment.GetComponent<BoxCollider>().bounds))
            {
                return false;
            }
             //ensure these roads arnt too close ot other roads
            if(RoadTooClose(working_segment,accepted_segment))
            {
                return false;
            }

        }

        return true;
    }


    void GlobalConstraints(RoadSegment segment)
    {

        Vector3 pos;
        Vector3 scale;

        int rot = (int)Mathf.Abs(segment.transform.rotation.eulerAngles.y);

        //spawn 4 new roads off this segment
        foreach (int i in angle)
        {

            if (Mathf.Approximately(rot, i))
            {
                switch (i)
                {
                    case 0:
                        {
                            CreateFwdBckXAxis(1, segment);
                            CreateLftRgtZAxis(1, segment);
                            CreateFwdBckXAxis(-1, segment);
                            CreateLftRgtZAxis(-1, segment);
                            break;
                        }
                    case 90:
                        {
                            CreateFwdBckZAxis(1, segment);
                            

                            CreateLftRgtXAxis(1, segment);
                            CreateFwdBckZAxis(-1, segment);
                            CreateLftRgtXAxis(-1, segment);
                            break;
                        }
                    case 180:
                        {
                            CreateFwdBckXAxis(1, segment);
                            

                            CreateLftRgtZAxis(1, segment);
                            CreateFwdBckXAxis(-1, segment);
                            CreateLftRgtZAxis(-1, segment);
                            break;
                        }
                    case 270:
                        {
                            CreateFwdBckZAxis(1, segment);
                            

                            CreateLftRgtXAxis(1, segment);
                            CreateFwdBckZAxis(-1, segment);
                            CreateLftRgtXAxis(-1, segment);
                            break;
                        }
                    default:
                        {
                            Debug.Log("Rejected Angle");
                            break;
                        }
                }
            }
        }



    }

    void CreateFwdBckXAxis(int fwd_bck, RoadSegment segment)
    {
        //fwd_bck is 1 when going forward, -1 when going back
        Vector3 pos;
        Vector3 scale;

        GameObject proposed_road = Instantiate(road_segment);

        float value = GM_.Instance.procedural.GetPointCutOff(segment.transform.position.x + (fwd_bck * min_segment_length), segment.transform.position.z);

        float random_length_forward = min_segment_length + ((max_segment_length - min_segment_length) * value);

        pos = segment.transform.position;
        pos.x += fwd_bck * ((segment.transform.localScale.x / 2) + (random_length_forward / 2));
        scale = segment.transform.localScale;
        scale.x = random_length_forward;
        scale.z = segment_width;

        proposed_road.GetComponent<RoadSegment>().RoadSegmentInit(pos, scale, Mathf.Round(segment.transform.rotation.eulerAngles.y), segment.GetTransform(), new Vector3(fwd_bck, 0, 0), proposed_road);
        proposed_road.GetComponent<RoadSegment>().AddConnectedSegmentEndPoint(segment.GetObj());

        segment.AddConnectedSegmentEndPoint(proposed_road);
        segment.AddChildNode(proposed_road);

        road_segment_queue.Enqueue(proposed_road);
    }

    void CreateFwdBckZAxis(int fwd_bck, RoadSegment segment)
    {
        //fwd_bck is 1 when going forward, -1 when going back
        Vector3 pos;
        Vector3 scale;

        GameObject proposed_road = Instantiate(road_segment);

        float value = GM_.Instance.procedural.GetPointCutOff(segment.transform.position.x + (fwd_bck * min_segment_length), segment.transform.position.z);

        float random_length_forward = min_segment_length + ((max_segment_length - min_segment_length) * value);


        pos = segment.transform.position;
        pos.z += fwd_bck * ((segment.transform.localScale.x / 2) + (random_length_forward / 2));
        scale = segment.transform.localScale;
        scale.x = random_length_forward;
        scale.z = segment_width;
        proposed_road.GetComponent<RoadSegment>().RoadSegmentInit(pos, scale, Mathf.Round(segment.transform.rotation.eulerAngles.y), segment.GetTransform(), new Vector3(0,0,fwd_bck), proposed_road);
        proposed_road.GetComponent<RoadSegment>().AddConnectedSegmentEndPoint(segment.GetObj());
        segment.AddConnectedSegmentEndPoint(proposed_road);
        segment.AddChildNode(proposed_road);


        road_segment_queue.Enqueue(proposed_road);
    }

    void CreateLftRgtZAxis(int lft_rgt, RoadSegment segment)
    {

        //lft_rgt is 1 when going left, -1 when going right
        Vector3 pos;
        Vector3 scale;

        GameObject proposed_road = Instantiate(road_segment);

        float value = GM_.Instance.procedural.GetPointCutOff(segment.transform.position.x , segment.transform.position.z + (lft_rgt * min_segment_length));

        float random_length_up = min_segment_length + ((max_segment_length - min_segment_length) * value);

        pos = segment.transform.position;
        pos.z += lft_rgt * (random_length_up / 2) + (lft_rgt * segment_width / 2);

        pos.x = Random.Range(segment.transform.position.x - ((segment.transform.localScale.x / 2) - (0.3f * (segment.transform.localScale.x / 2))), segment.transform.position.x + ((segment.transform.localScale.x / 2) - (0.3f * (segment.transform.localScale.x / 2))));

        scale = segment.transform.localScale;
        scale.x = random_length_up;
        scale.z = segment_width;
        proposed_road.GetComponent<RoadSegment>().RoadSegmentInit(pos, scale, Mathf.Round(segment.transform.rotation.eulerAngles.y + (90)), segment.GetTransform(), new Vector3(0,0,lft_rgt), proposed_road);
        proposed_road.GetComponent<RoadSegment>().AddConnectedSegmentEndPoint(segment.GetObj());
        segment.AddConnectedSegmentMidPoints(proposed_road);
        segment.AddChildNode(proposed_road);


        road_segment_queue.Enqueue(proposed_road);
    }

    void CreateLftRgtXAxis(int lft_rgt, RoadSegment segment)
    {

        //lft_rgt is 1 when going left, -1 when going right
        Vector3 pos;
        Vector3 scale;

        GameObject proposed_road = Instantiate(road_segment);

        float value = GM_.Instance.procedural.GetPointCutOff(segment.transform.position.x, segment.transform.position.z + (lft_rgt * min_segment_length));

        float random_length_up = min_segment_length + ((max_segment_length - min_segment_length) * value);

        pos = segment.transform.position;
        pos.x += lft_rgt * (random_length_up / 2) + (lft_rgt * segment_width / 2);

        pos.z = Random.Range(segment.transform.position.z - ((segment.transform.localScale.x / 2) - (0.3f * (segment.transform.localScale.x / 2))), segment.transform.position.z + ((segment.transform.localScale.x / 2) - (0.3f * (segment.transform.localScale.x / 2))));

        scale = segment.transform.localScale;
        scale.x = random_length_up;
        scale.z = segment_width;
        proposed_road.GetComponent<RoadSegment>().RoadSegmentInit(pos, scale, Mathf.Round(segment.transform.rotation.eulerAngles.y + (90)),segment.GetTransform() ,new Vector3(lft_rgt, 0, 0), proposed_road);
        proposed_road.GetComponent<RoadSegment>().AddConnectedSegmentEndPoint(segment.GetObj());
        segment.AddConnectedSegmentMidPoints(proposed_road);
        segment.AddChildNode(proposed_road);


        road_segment_queue.Enqueue(proposed_road);
    }

    
    void CreateIntersections()
    {
        int layer_mask = 1 << 8;
        bool x_or_z = false; //false is 0 or 180         true is 90 or 270

        foreach (GameObject obj in accepted_segments)
        {

            float rot = Mathf.Abs(obj.transform.rotation.eulerAngles.y);

            if(rot < 1)
            {
                rot = 0;
            }

            foreach (int i in angle)
            {
                if (Mathf.Approximately(rot, i))
                {

                    directions = GetDirection(i, ref x_or_z);

                    break;
                }
            }

            foreach (Vector3 dir in directions)
            {
                Physics.SyncTransforms();
                RaycastHit hit;

                Ray ray = new Ray(obj.transform.position, dir);
                float distance = 1;

                while (distance < max_intersection_distance)
                {
                    if (Physics.Raycast(ray, out hit, RayDistance(obj.transform.localScale, distance), layer_mask))
                    {

                        if (hit.collider.gameObject == obj) //if I have hit myslef
                        {
                            break;
                        }

                        if (hit.collider.gameObject.transform == obj.GetComponent<RoadSegment>().parent || hit.collider.gameObject.GetComponent<RoadSegment>().parent == obj.transform) //if I have hit my parent
                        {
                            break;
                        }

                        //if we share the same parent
                        if (hit.collider.gameObject.GetComponent<RoadSegment>().parent == obj.GetComponent<RoadSegment>().parent)
                        {
                            break;
                        }

                        if (RotationEquals((int)hit.collider.gameObject.transform.rotation.eulerAngles.y,(int) obj.transform.rotation.eulerAngles.y))
                        {
                            break;
                        }

                        //if(RoadTooClose(obj.GetComponent<RoadSegment>(), hit.collider.gameObject))
                        //{
                        //    break;
                        //}

                        
                        obj.transform.localScale += new Vector3(CalcDistance(obj.transform, hit.collider.gameObject.transform, x_or_z, dir), 0, 0);
                        obj.transform.position += (dir * (CalcDistance(obj.transform, hit.collider.gameObject.transform, x_or_z, dir)));

                        obj.GetComponent<RoadSegment>().AddConnectedSegmentEndPoint(hit.collider.gameObject);
                        hit.collider.gameObject.GetComponent<RoadSegment>().AddConnectedIntersection(obj);

                        break;
                    }

                    distance += intersection_distance_step;
                }

            }
        }
    }

    float CalcDistance(float a, float b)
    {
        return Mathf.Abs(a - b);
    }

    float CalcDistance(Transform moving_segment, Transform stationary_segment, bool x_or_z, Vector3 direction)
    {
        float return_value = 0;
        float moving_init_pos = 0;
        float stationary_init_pos = 0;

        if (!x_or_z) //obj is rotated at 0 or 180
        {
            moving_init_pos = moving_segment.position.x + (direction.x * (moving_segment.localScale.x / 2));
            stationary_init_pos = stationary_segment.position.x + ((direction.x * -1) * (stationary_segment.localScale.z / 2));

        }
        else if (x_or_z) //if object is rotate 90 or 270
        {
            moving_init_pos = moving_segment.position.z + (direction.z * (moving_segment.localScale.x / 2));
            stationary_init_pos = stationary_segment.position.z + ((direction.z * -1) * (stationary_segment.localScale.z / 2));
        }

        return_value = CalcDistance(moving_init_pos, stationary_init_pos);


        return return_value;
    }

    float RayDistance(Vector3 scale, float multiplier)
    {
        return ((scale.x / 2) + (min_segment_length / 2)) * multiplier;
    }

    bool RotationEquals(int rot_a, int rot_b)
    {
        return Mathf.Approximately(Mathf.Abs(rot_a), Mathf.Abs(rot_b)) || Mathf.Approximately(Mathf.Abs(rot_a + 180), Mathf.Abs(rot_b)) || Mathf.Approximately(Mathf.Abs(rot_a - 180), Mathf.Abs(rot_b));
    }

    Vector3[] GetDirection(int rot)
    {
        Vector3[] dir = new Vector3[] { new Vector3(0, 0, 0) };

        switch (rot)
        {
            case 0:
                {
                    dir = new Vector3[] { new Vector3(1, 0, 0), new Vector3(-1, 0, 0) };
                    break;
                }
            case 90:
                {
                    dir = new Vector3[] { new Vector3(0, 0, 1), new Vector3(0, 0, -1) };
                    break;
                }
            case 180:
                {
                    dir = new Vector3[] { new Vector3(1, 0, 0), new Vector3(-1, 0, 0) };
                    break;
                }
            case 270:
                {
                    dir = new Vector3[] { new Vector3(0, 0, 1), new Vector3(0, 0, -1) };
                    break;
                }
            default:
                {
                    break;
                }
        }

        return dir;
    }

    Vector3[] GetDirection(int rot, ref bool x_or_z)
    {
        Vector3[] dir = new Vector3[] { new Vector3(0, 0, 0) };

        switch (rot)
        {
            case 0:
                {
                    dir = new Vector3[] { new Vector3(1, 0, 0), new Vector3(-1, 0, 0) };
                    x_or_z = false;
                    break;
                }
            case 90:
                {
                    dir = new Vector3[] { new Vector3(0, 0, 1), new Vector3(0, 0, -1) };
                    x_or_z = true;
                    break;
                }
            case 180:
                {
                    dir = new Vector3[] { new Vector3(1, 0, 0), new Vector3(-1, 0, 0) };
                    x_or_z = false;
                    break;
                }
            case 270:
                {
                    dir = new Vector3[] { new Vector3(0, 0, 1), new Vector3(0, 0, -1) };
                    x_or_z = true;
                    break;
                }
            default:
                {
                    break;
                }
        }

        return dir;
    }

    bool RoadTooClose(RoadSegment working_segment, GameObject accepeted_segment)
    {
        if (RotationEquals((int)accepeted_segment.transform.rotation.eulerAngles.y, (int)working_segment.transform.rotation.eulerAngles.y))
        {
            if (Vector3.Distance(accepeted_segment.transform.position, working_segment.transform.position) < min_segment_length)
            {
                return true;
            }

            directions = GetDirection((int)working_segment.transform.rotation.eulerAngles.y);

            foreach (Vector3 dir in directions)
            {
                Vector3 offset_working_seg = new Vector3(0, 0, 0);

                offset_working_seg = dir * working_segment.transform.localScale.x / 2;
                Vector3 offset_other_seg = (dir* -1) * accepeted_segment.transform.localScale.x/2;

                if (Vector3.Distance(working_segment.transform.position + offset_working_seg, accepeted_segment.transform.position) < min_segment_length / 2)
                {
                    return true;
                }

                if (Vector3.Distance(working_segment.transform.position, accepeted_segment.transform.position + offset_other_seg) < min_segment_length / 2)
                {
                    return true;
                }

                if (Vector3.Distance(working_segment.transform.position + offset_working_seg, accepeted_segment.transform.position + offset_other_seg) < min_segment_length / 2)
                {
                    if (working_segment.parent != accepeted_segment.transform)
                    {
                        if(accepeted_segment.GetComponent<RoadSegment>().parent != working_segment.transform && accepeted_segment.GetComponent<RoadSegment>().parent != accepeted_segment.transform && accepeted_segment.GetComponent<RoadSegment>().parent != working_segment.parent)
                            return true;
                    }

                }

            }

        }

        return false;
    }


    //the purpose of the clean up funtion is to a) remove any roads that only has one connection and b) to alter roads that make up a complicated block

    void CleanUp()
    {
        for (int i = accepted_segments.Count - 1; i > -1; i--)
        {
            if (accepted_segments[i].GetComponent<RoadSegment>().connected_points_all.Count <= 1)
            {
                foreach (GameObject temp in accepted_segments[i].GetComponent<RoadSegment>().connected_points_all)
                {
                    temp.GetComponent<RoadSegment>().RemoveObj(accepted_segments[i]);
                }

                DestroyImmediate(accepted_segments[i]);

                accepted_segments.RemoveAt(i);
            }
        }
    }
}

