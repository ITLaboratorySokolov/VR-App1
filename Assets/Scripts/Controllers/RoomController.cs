using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RoomController : MonoBehaviour
{
    [SerializeField]
    WallCollisionProcessor irlRoomWalls;

    [SerializeField]
    GameObject player;

    [SerializeField]
    GameObject irlGround;

    // Start is called before the first frame update
    void Start()
    {
        // CenterRoom();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTeleport(TeleportingEventArgs args)
    {
        Debug.Log("Teleporting");
        Debug.Log("From: " + player.transform.position);
        Debug.Log("To: " + args.teleportRequest.destinationPosition);
    }

    public void SetRoomSize(float wX, float wZ)
    { 
        // set position of walls
        irlRoomWalls.SetBorderWall(wX, wZ);

        // set size of teleportation area
        float currentSizeX = irlGround.GetComponent<Renderer>().bounds.size.x;
        float currentSizeZ = irlGround.GetComponent<Renderer>().bounds.size.z;

        Vector3 scale = irlGround.transform.localScale;
        scale.x = wX * (scale.x / currentSizeX);
        scale.z = wZ * (scale.z / currentSizeZ);

        float scalingX = scale.x / irlGround.transform.localScale.x;
        float scalingZ = scale.z / irlGround.transform.localScale.z;

        irlGround.transform.localScale = scale;

        /*
        scale = scene.transform.localScale;
        scale.x = scale.x * scalingX;
        scale.z = scale.z * scalingZ;
        scene.transform.localScale = scale;
        */
    }

}
