using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform mercyObj;
    public Vector3 camOffset;

    void LateUpdate()
    {
        camOffset = new Vector3(mercyObj.transform.position.x, 2, -8);
        transform.position = mercyObj.transform.position + camOffset;
    }




    /*Point of interest - what the camera is going to follow
    public GameObject poi;

    //initial z pos of camera
    //public float camZ;
    private Vector3 camPos;

    //easing var
    public float easing = 0.05f;
    //a follow cam singleton
    static public FollowCam Instance;


    private void Awake()
    {
        Instance = this;
        camPos = new Vector3(poi.transform.position.x, poi.transform.position.y, -5);
    }

    private void FixedUpdate()
    {
        //only follow if there is something to follow
        //if (poi == null) return;

        //get positions of poi
        //Vector3 destination = poi.transform.position;
        Vector3 destination;

        if (poi == null)
        {
            destination = Vector3.zero;
        }
        else
        {
            destination = poi.transform.position;

            destination = Vector3.Lerp(transform.position, destination, easing);

        }

        //retain the destination.z of camZ
        //destination.z = camZ;

        //move the camera to the destination
        transform.position = destination;
    }

    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    public GameObject mercyObj;
    private Vector3 camPos;
    //static public FollowCam Instance;
    public Vector3 camOffset;

    
    
    
    // Start is called before the first frame update
    void Awake()
    {
        
        Instance = this;
        camPos = transform.position;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Vector3 mercyPos;
        camOffset = new Vector3(mercyObj.transform.position.x, mercyObj.transform.position.y, -3);
        camPos = mercyObj.transform.position + camOffset;


        
        if (mercyObj != null)
        {
            camPos = mercyObj.transform.position + camOffset;
        }
        else
        {

        }
        
    }
    */
}
