using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers;

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

    [SerializeField]
    TextureProperty texProp;

    [SerializeField]
    Material lineMaterial;
    [SerializeField]
    Material boxMaterial;
    [SerializeField]
    Material generalMaterial;

    // Start is called before the first frame update
    void Start()
    {
        timeUntilUpdate = 0;

        MeshRenderer mr = GetComponent<MeshRenderer>();
        Color c = mr.material.GetColor("_Color"); ;
        Texture t = mr.material.GetTexture("_MainTex");

        tag = "worldObject";

        //texProp.textureName = "_MainTex";

        if (name.StartsWith("Line"))
        {
            mr.material = lineMaterial;
            mr.material.SetColor("_Color", c);
            if (t != null)
                mr.material.SetTexture("_MainTex", t);

            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
            int layerNum = LayerMask.NameToLayer("DrawnLine");
            gameObject.layer = layerNum;


        }
        else if (name.StartsWith("CardboardBox"))
        {
            mr.material = boxMaterial;
            mr.material.SetColor("_Color", c);
            if (t != null)
                mr.material.SetTexture("_MainTex", t);

            // no updating of texture properties!
            MeshPropertiesManager mpm = GetComponent<MeshPropertiesManager>();
            
            TextureSizeProperty tsp = GetComponent<TextureSizeProperty>();
            TextureProperty tp = GetComponent<TextureProperty>();
            
            mpm.OptionalProperties.Remove(tsp);
            Destroy(tsp);
            
            mpm.OptionalProperties.Remove(tp);
            Destroy(tp);
        }
        else
        {
            mr.material = generalMaterial;
            mr.material.SetColor("_Color", c);
            if (t != null)
                mr.material.SetTexture("_MainTex", t);
        }
    }

    public void StartPosition()
    {
        GameObject ground = GameObject.Find("Ground");
        if (ground != null)
        {
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
                transform.hasChanged = false;

                objCont.UpdateProperties(this.name);
                timeUntilUpdate = 1;
                lastRot = newRot;
                lastPos = newPos;
            }
        }
    }

    

}
