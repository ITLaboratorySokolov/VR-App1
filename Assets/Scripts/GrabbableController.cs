using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableController : MonoBehaviour
{
    bool isHeld;
    Transform heldBy;
    Quaternion startRot;
    Quaternion startRotHand;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // is being held - update position   
        if (isHeld)
        {
            transform.position = heldBy.position;

            // The difference C between quaternions A and B
            Quaternion diff = heldBy.rotation * Quaternion.Inverse(startRotHand);
            // Add the difference to D
            transform.rotation = diff * startRot;
        }

    }

    internal void StartBeingHeld(Transform transform)
    {
        isHeld = true;
        heldBy = transform;
        startRot = this.transform.rotation;
        startRotHand = transform.rotation;
    }

    internal void StopBeingHeld(Vector3 a)
    {
        Rigidbody rb = this.GetComponent<Rigidbody>();
        var f = a * rb.mass;

        isHeld = false;
        rb.AddForce(f);

        Debug.Log(f);
    }
}
