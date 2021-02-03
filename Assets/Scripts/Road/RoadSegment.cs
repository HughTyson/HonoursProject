using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSegment : MonoBehaviour
{

    Transform object_transform;
    BoxCollider collider;
    public int spawner = 0;

    private void Start()
    {
        object_transform = GetComponent<Transform>();
        collider = GetComponent<BoxCollider>();

    }
    public void RoadSegmentInit(Vector3 position, Vector3 scale, float rot, int s)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, rot, 0));
        transform.position = position;
        transform.localScale = scale;

        spawner = s;
    }




}
