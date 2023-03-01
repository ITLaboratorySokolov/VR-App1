using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RoomController : MonoBehaviour
{
    [SerializeField]
    WallCollisionProcessor wallCollisionProcessor;

    [SerializeField]
    GameObject player;

    [SerializeField]
    GameObject walkableGround;
    [SerializeField]
    GameObject displayGround;
    [SerializeField]
    GameObject scene;

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
        wallCollisionProcessor.SetBorderWall(wX, wZ);

        // set size of teleportation area
        float currentSizeX = walkableGround.GetComponent<Renderer>().bounds.size.x;
        float currentSizeZ = walkableGround.GetComponent<Renderer>().bounds.size.z;

        Vector3 scale = walkableGround.transform.localScale;
        scale.x = wX * (scale.x / currentSizeX);
        scale.z = wZ * (scale.z / currentSizeZ);

        float scalingX = scale.x / walkableGround.transform.localScale.x;
        float scalingZ = scale.z / walkableGround.transform.localScale.z;

        walkableGround.transform.localScale = scale;

        scale = scene.transform.localScale;
        scale.x = scale.x * scalingX;
        scale.z = scale.z * scalingZ;
        scene.transform.localScale = scale;
    }

}
