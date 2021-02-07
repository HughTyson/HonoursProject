using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoadDirection
{
    NORTH,SOUTH,EAST,WEST
}


public class RoadSegment : MonoBehaviour
{

    Transform object_transform;
    BoxCollider collider;
    public int spawner = 0;
    public int segment_number = 0;
    public Transform parent;

    public RoadDirection direction;

    private void Start()
    {
        
        

    }
    public void RoadSegmentInit(Vector3 position, Vector3 scale, float rot,int s, int segment_number_, Transform p)
    {        
        
        object_transform = GetComponent<Transform>();
        collider = GetComponent<BoxCollider>();

        transform.rotation = Quaternion.Euler(new Vector3(0, rot, 0));
        transform.position = position;
        transform.localScale = scale;

        spawner = s;
        segment_number_ = segment_number;
        parent = p;
        

    }

    public Transform GetTransform()
    {
        return object_transform;
    }

    public void ReScaleCollisionBox()
    {
        collider.size = new Vector3(1, 1, 1);
    }

    private void Update()
    {
        //Debug.DrawLine(transform.position, transform.position + new Vector3((transform.localScale.x / 2) + (15 / 2), 0,0));
        Debug.DrawRay(transform.position, new Vector3(1, 0, 0) * ((transform.localScale.x / 2) + (15 / 2)));
       Debug.DrawRay(transform.position, (new Vector3(-1,0,0)) - new Vector3((transform.localScale.x / 2) + (15 / 2), 0, 0));
    }

}
