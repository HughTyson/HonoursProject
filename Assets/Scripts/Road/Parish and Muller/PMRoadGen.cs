using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMRoadGen : MonoBehaviour
{

    public GameObject road_segment;

    [SerializeField] int max_itterations = 100;
    [SerializeField] int max_roads = 700;


    [SerializeField] float max_segment_length = 35;
    [SerializeField] float min_segment_length = 15;


    [SerializeField] float segment_width = 3f;


    Queue<GameObject> priority_queue;
    List<GameObject> accepted_segments;
    public List<GameObject> Generate()
    {

       accepted_segments = new List<GameObject>();
        priority_queue = new Queue<GameObject>();

        GameObject obj = Instantiate(road_segment);
        obj.GetComponent<RoadSegment>().RoadSegmentInit(new Vector3(0, 0, 0), new Vector3(min_segment_length, 0.1f, segment_width), 0, 0);
        priority_queue.Enqueue(obj);


        int itterations = 0;

        //generate roads until max amount of roads have been created or max amount of itterations has passed or there are no segments in the queue
        while (/*accepted_segments.Count < max_roads ||*/  itterations < max_itterations)
        {
            itterations++;

            GameObject working_obj = priority_queue.Dequeue();
            RoadSegment segment = working_obj.GetComponent<RoadSegment>();

            bool accepted = LocalConstraints(working_obj);

            if (accepted)
            {
                accepted_segments.Add(working_obj);

                if(itterations < max_itterations)
                    GlobalConstraints(segment);
            }
            else
            {
                Destroy(working_obj);
            }
        }

        foreach(GameObject i in priority_queue)
        {
            Destroy(i);
        }

        return accepted_segments;
    }



    bool LocalConstraints(GameObject working_obj)
    {
        RoadSegment segment = working_obj.GetComponent<RoadSegment>();

        foreach (GameObject s in accepted_segments)
        {

                if(s.transform.rotation == segment.transform.rotation)
                {
                    if(Vector3.Distance(s.transform.position, segment.transform.position) < 8)
                    {
                        return false;
                    }
                }

                Physics.SyncTransforms();
                if (working_obj.GetComponent<BoxCollider>().bounds.Intersects(s.GetComponent<BoxCollider>().bounds))
                {
                    return false;
                }
            

        }

        //local constaints


        return true;
    }

    void GlobalConstraints(RoadSegment segment)
    {

        Vector3 pos;
        Vector3 scale;


        


        switch (segment.transform.rotation.eulerAngles.y)
        {
            case 0:
                {
                    CreateFwdBckXAxis(1, segment);
                    CreateFwdBckXAxis(-1, segment);

                    CreateLftRgtZAxis(1, segment);
                    CreateLftRgtZAxis(-1, segment);
                    break;
                }
            case 90:
                {
                    CreateFwdBckZAxis(1, segment);
                    CreateFwdBckZAxis(-1, segment);

                    CreateLftRgtXAxis(1, segment);
                    CreateLftRgtXAxis(-1, segment);
                    break;
                }
            case 180:
                {
                    CreateFwdBckXAxis(1, segment);
                    CreateFwdBckXAxis(-1, segment);

                    CreateLftRgtZAxis(1, segment);
                    CreateLftRgtZAxis(-1, segment);
                    break;
                }
            case 270:
                {
                    CreateFwdBckZAxis(1, segment);
                    CreateFwdBckZAxis(-1, segment);

                    CreateLftRgtXAxis(1, segment);
                    CreateLftRgtXAxis(-1, segment);
                    break;
                }
        }

    }
    
    void CreateFwdBckXAxis(int fwd_bck, RoadSegment segment)
    {
        //fwd_bck is 1 when going forward, -1 when going back
        Vector3 pos;
        Vector3 scale;

        GameObject proposed_road = Instantiate(road_segment);

        float random_length_forward = Random.Range(min_segment_length, max_segment_length);

        pos = segment.transform.position;
        pos.x += fwd_bck * (segment.transform.localScale.x / 2) + (random_length_forward / 2);
        scale = segment.transform.localScale;
        scale.x = random_length_forward;
        scale.z = segment_width;
        proposed_road.GetComponent<RoadSegment>().RoadSegmentInit(pos, scale, segment.transform.rotation.eulerAngles.y, 1);

        priority_queue.Enqueue(proposed_road);
    }

    void CreateFwdBckZAxis(int fwd_bck, RoadSegment segment)
    {
        //fwd_bck is 1 when going forward, -1 when going back
        Vector3 pos;
        Vector3 scale;

        GameObject proposed_road = Instantiate(road_segment);

        float random_length_forward = Random.Range(min_segment_length, max_segment_length);

        pos = segment.transform.position;
        pos.z += fwd_bck * (segment.transform.localScale.z / 2) + (random_length_forward / 2);
        scale = segment.transform.localScale;
        scale.x = random_length_forward;
        scale.z = segment_width;
        proposed_road.GetComponent<RoadSegment>().RoadSegmentInit(pos, scale, segment.transform.rotation.eulerAngles.y, 2);

        priority_queue.Enqueue(proposed_road);
    }

    void CreateLftRgtZAxis(int lft_rgt, RoadSegment segment)
    {

        //lft_rgt is 1 when going left, -1 when going right
        Vector3 pos;
        Vector3 scale;

        GameObject proposed_road = Instantiate(road_segment);

        float random_length_up = Random.Range(min_segment_length, max_segment_length);

        pos = segment.transform.position;
        pos.z += lft_rgt * (random_length_up / 2) + (lft_rgt * segment_width / 2);
        scale = segment.transform.localScale;
        scale.x = random_length_up;
        scale.z = segment_width;
        proposed_road.GetComponent<RoadSegment>().RoadSegmentInit(pos, scale, segment.transform.rotation.eulerAngles.y + (90), 3);

        priority_queue.Enqueue(proposed_road);
    }

    void CreateLftRgtXAxis(int lft_rgt, RoadSegment segment)
    {

        //lft_rgt is 1 when going left, -1 when going right
        Vector3 pos;
        Vector3 scale;

        GameObject proposed_road = Instantiate(road_segment);

        float random_length_up = Random.Range(min_segment_length, max_segment_length);

        pos = segment.transform.position;
        pos.x += lft_rgt * (random_length_up / 2) + (lft_rgt * segment_width / 2);
        scale = segment.transform.localScale;
        scale.x = random_length_up;
        scale.z = segment_width;
        proposed_road.GetComponent<RoadSegment>().RoadSegmentInit(pos, scale, segment.transform.rotation.eulerAngles.y + (90), 4);

        priority_queue.Enqueue(proposed_road);
    }
}

