using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoadSegment : MonoBehaviour
{

    Transform object_transform;
    BoxCollider collider;
    public Transform parent;

    private void Start()
    {
        
        

    }
    public void RoadSegmentInit(Vector3 position, Vector3 scale, float rot, Transform p)
    {        
        
        object_transform = GetComponent<Transform>();
        collider = GetComponent<BoxCollider>();

        transform.rotation = Quaternion.Euler(new Vector3(0, rot, 0));
        transform.position = position;
        transform.localScale = scale;

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
    }

}
