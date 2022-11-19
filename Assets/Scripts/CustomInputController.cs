using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class CustomInputController : MonoBehaviour
{

    /// <summary> Left hand </summary>
    [SerializeField]
    GameObject leftHand;
    /// <summary> Right hand </summary>
    [SerializeField]
    GameObject rightHand;

    [Header("Grabbing")]
    /// <summary> Grab with right hand </summary>
    [SerializeField]
    InputActionReference grabR;
    /// <summary> Grab with left hand </summary>
    [SerializeField]
    InputActionReference grabL;
    /// <summary> Grabbed with right hand </summary>
    List<GameObject> grabbedR;
    /// <summary> Grabbed with left hand </summary>
    List<GameObject> grabbedL;

    Vector3 prevPos;
    Vector3 newPos;

    Vector3 prevVel;
    Vector3 newVel;

    UnityEngine.XR.InputDevice LeftControllerDevice;
    UnityEngine.XR.InputDevice RightControllerDevice;

    // Start is called before the first frame update
    void Start()
    {
        grabR.action.started += GrabObjectsR;
        grabL.action.started += GrabObjectsL;
        grabbedR = new List<GameObject>();
        grabbedL = new List<GameObject>();
        prevPos = leftHand.transform.position;
        newPos = leftHand.transform.position;

        LeftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        LeftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    /// <summary>
    /// Grab objects with left hand
    /// </summary>
    /// <param name="obj"></param>
    private void GrabObjectsL(InputAction.CallbackContext obj)
    {
        GrabObjects(leftHand, grabbedL);
    }

    /// <summary>
    /// Grab objects with right hand
    /// </summary>
    /// <param name="ctx"></param>
    private void GrabObjectsR(InputAction.CallbackContext ctx)
    {
        GrabObjects(rightHand, grabbedR);
    }

    /// <summary>
    /// Grab objects
    /// </summary>
    /// <param name="hand"> Hand object </param>
    /// <param name="gameList"> List for grabbed objects </param>
    private void GrabObjects(GameObject hand, List<GameObject> gameList)
    {
        Collider[] cs = Physics.OverlapSphere(hand.transform.position, hand.GetComponent<SphereCollider>().radius * hand.transform.localScale.x);
        for (int i = 0; i < cs.Length; i++)
        {
            string name = cs[i].gameObject.name;
            if (cs[i].gameObject.tag == "grabbable")
            {
                Debug.Log("Grab " + name + "!");

                GrabbableController gr = cs[i].gameObject.GetComponent<GrabbableController>();
                gr.StartBeingHeld(hand.transform);
                gameList.Add(cs[i].gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        prevPos = newPos;
        newPos = leftHand.transform.position;

        prevVel = newVel;
        newVel = (newPos - prevPos) / Time.deltaTime;

        // Stop drawing on released button
        if (grabR.action.WasReleasedThisFrame())
            StopHolding(grabbedR, false);

        if (grabL.action.WasReleasedThisFrame())
            StopHolding(grabbedL, true);


    }

    /// <summary>
    /// Stop holding all objects in golist
    /// </summary>
    /// <param name="golist"> List with held game objects </param>
    private void StopHolding(List<GameObject> golist, bool left)
    {
        var deltav = newVel - prevVel;
        Vector3 a = deltav / Time.deltaTime;

        LeftControllerDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out a);

        foreach (GameObject o in golist)
        {
            GrabbableController gr = o.GetComponent<GrabbableController>();
            gr.StopBeingHeld(a);
        }

        if (left)
            grabbedL = new List<GameObject>();
        else
            grabbedR = new List<GameObject>();
    }
}
