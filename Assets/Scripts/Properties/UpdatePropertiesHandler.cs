using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class UpdatePropertiesHandler : MonoBehaviour
{
    float timeUntilUpdate;

    [SerializeField]
    public ObjectController objCont;
    [SerializeField]
    public ServerConectionController serverConnection;

    Vector3 newPos;
    Vector3 lastPos;

    Quaternion newRot;
    Quaternion lastRot;

    // Start is called before the first frame update
    void Start()
    {
        timeUntilUpdate = 0;
    }

    public void StartPosition()
    {
        GameObject ground = GameObject.Find("Ground");
        if (ground != null)
        {
            Debug.Log(name + " " + transform.position.y);
            float height = ground.transform.position.y;
            if (transform.position.y < height)
                transform.position = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);
        }

        transform.position = new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        newPos = transform.position;
        newRot = transform.rotation;

        timeUntilUpdate -= Time.deltaTime;

        if (newPos != lastPos && newRot != lastRot && timeUntilUpdate < 0.1f)
        {
            UpdateServerTransform();
        }
    }

    private async void UpdateServerTransform()
    {
        if (objCont != null && serverConnection != null && serverConnection.syncCallDone)
        {
            bool val = await objCont.ContainsObject(name);
            if (val)
            {

                print("The transform has changed!");
                transform.hasChanged = false;

                objCont.UpdateProperties(this.name);
                timeUntilUpdate = 1;
                lastRot = newRot;
                lastPos = newPos;
            }
        }
    }

    

}
