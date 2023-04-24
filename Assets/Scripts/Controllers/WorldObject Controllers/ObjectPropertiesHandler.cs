using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers;

/// <summary>
/// Script used to set properties of incoming game object from the server
/// - properties are based on the name of the game object
/// </summary>
public class ObjectPropertiesHandler : MonoBehaviour
{
    /// <summary> Object controller </summary>
    [SerializeField]
    public ObjectController objCont;

    [Header("Materials")]
    /// <summary> Line material </summary>
    [SerializeField]
    Material lineMaterial;
    /// <summary> Box material </summary>
    [SerializeField]
    Material boxMaterial;
    /// <summary> General material </summary>
    [SerializeField]
    Material generalMaterial;

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        Color c = mr.material.GetColor("_Color"); ;
        Texture t = mr.material.GetTexture("_MainTex");

        tag = "worldObject";

        // Set material for line
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
        // Set material for box
        else if (name.StartsWith("CardboardBox"))
        {
            mr.material = boxMaterial;
            mr.material.SetColor("_Color", c);
            if (t != null)
                mr.material.SetTexture("_MainTex", t);

            // GRAVITY DISABLED
            // enable gravity only for "local" objects
            // Rigidbody rb = GetComponent<Rigidbody>();
            // if (name.StartsWith("CardboardBox_" + objCont.clientName.Value))
            // {
            //    rb.useGravity = true;
            //    rb.isKinematic = false;
            // }
        }
        // Set head and hands
        else if (name.StartsWith("Head") || name.StartsWith("Hand")) //HandL or LHand??
        {
            mr.material = generalMaterial;
            mr.material.SetColor("_Color", c);
            if (t != null)
                mr.material.SetTexture("_MainTex", t);

            // remove grab interactable
            if (GetComponent<XRGrabInteractable>() != null)
                GetComponent<XRGrabInteractable>().enabled = false;
            
            // set colliders
            BoxCollider bc = GetComponent<BoxCollider>();
            MeshCollider mc = GetComponent<MeshCollider>();
            SphereCollider sc = GetComponent<SphereCollider>();

            if (bc != null)
            {
                bc.size = new Vector3();
                bc.enabled = false;
            }

            if (mc != null)
            {
                mc.enabled = false;
                mc.sharedMesh = null;
            }

            if (sc != null)
            {
                sc.radius = 0;
                sc.enabled = false;
            }

            if (GetComponent<Rigidbody>() != null)
            {
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<Rigidbody>().isKinematic = true;
            }

        }

        // Set material for general object
        else
        {
            mr.material = generalMaterial;
            mr.material.SetColor("_Color", c);
            if (t != null)
                mr.material.SetTexture("_MainTex", t);

            BitmapPropertiesManager bpm = GetComponent<BitmapPropertiesManager>();
            BoxCollider bc = GetComponent<BoxCollider>();
            MeshCollider mc = GetComponent<MeshCollider>();
            SphereCollider sc = GetComponent<SphereCollider>();

            // if there is no collider or it is a bitmap set a box collider
            if (bpm != null || (!bc.enabled && !mc.enabled && !sc.enabled) || bc.size == new Vector3() )
            {
                Mesh m = GetComponent<MeshFilter>().mesh;
                BoxCollider col = GetComponent<BoxCollider>();
                col.enabled = true;

                Debug.Log("Coliders set " + gameObject.name);

                Debug.Log("Set bounds " + m.bounds.size);

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
