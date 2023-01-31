using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers;

public class InputPropertiesHandler : MonoBehaviour
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
            // TODO todle je asi blbost
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

            // is it a bitmap? or no collider active?
            BitmapPropertiesManager bpm = GetComponent<BitmapPropertiesManager>();
            BoxCollider bc = GetComponent<BoxCollider>();
            MeshCollider mc = GetComponent<MeshCollider>();
            SphereCollider sc = GetComponent<SphereCollider>();

            if (bpm != null || (!bc.enabled && !mc.enabled && !sc.enabled) || bc.size == new Vector3() )
            {
                Debug.Log("Setting colider for " + name);

                Mesh m = GetComponent<MeshFilter>().mesh;
                BoxCollider col = GetComponent<BoxCollider>();
                col.center = m.bounds.center;
                col.size= m.bounds.size;
            }
        }
    }

    public void StartPosition()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

}
