using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField]
    WallCollisionProcessor wallCollisionProcessor;

    [SerializeField]
    Transform roomCenter;

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

    private void CenterRoom()
    {
        Vector3 center = roomCenter.position;

        walkableGround.transform.position = center;
        scene.transform.position = center;
        displayGround.transform.position = new Vector3(center.x, center.y - 0.05f, center.z);
        wallCollisionProcessor.MoveWallPosition(center);
    }
}
