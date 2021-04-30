using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{

    [SerializeField] Rigidbody rigidbody;
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    private float maxDistance = 100f;
    private SpringJoint joint;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();  //line renderer
    }

    void Update()
    {
        //if the left mouse button is down 
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }

        if (joint != null && Input.GetMouseButton(1))
        {
            //if the joint is active and the right mouse button is down move towards the grapple point
            MoveTo();
        }

        TooCloseToGround();
    }

    //Called after Update
    void LateUpdate()
    {
       
        DrawRope();
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))  //cast a ray alongthe camera's forward, over a mximum distacne, looking for only grappable objects
        {
            grapplePoint = hit.point;   //if we find a hit save the point we hit
            joint = player.gameObject.AddComponent<SpringJoint>(); //add a joint to the player
            joint.autoConfigureConnectedAnchor = false; 
            joint.connectedAnchor = grapplePoint;   

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = camera.position;
        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;

    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    void MoveTo()
    {
        Vector3 grapple_dir = (grapplePoint - transform.position).normalized;
        float thrust_value = 5;
        rigidbody.AddForce(grapple_dir * thrust_value, ForceMode.Impulse);

    }

    float min_accepeted_distance = 50;
    void TooCloseToGround()
    {

        if (!joint) return; 

        RaycastHit hit;

        if(Physics.Raycast(camera.position, new Vector3(0,-1,0), out hit))
        {
            if(hit.transform.tag == "RoadSegment")
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                
                if(distance < min_accepeted_distance)
                {
                    joint.maxDistance = joint.maxDistance * 0.3f;
                    joint.minDistance = joint.minDistance * 0.25f;
                    Debug.Log("HELLO");
                }

                
            }
        }

    }

}
