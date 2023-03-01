using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;

public class BoxSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] boxes;
    [SerializeField]
    GameObject[] boxPositions;
    [SerializeField]
    ObjectController objCont;
    [SerializeField]
    GameObject serverObjectParent;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SpawnTestBox()
    {
        StartCoroutine(TestCorout());
    }

    public IEnumerator TestCorout()
    {
        var t = SpawnTestBoxT();

        while (!t.IsCompleted)
            yield return null;

        Debug.Log("Done spawning test box");
    }

    public async Task SpawnTestBoxT()
    {
        GameObject o = Instantiate(boxes[0], boxPositions[0].transform.position, boxPositions[0].transform.rotation, this.transform);
        var uph = o.GetComponent<InputPropertiesHandler>();
        uph.objCont = objCont;
        uph.StartPosition();
        string name = "CardboardBox_Test";
        o.name = name;
        
        bool val = await objCont.ContainsObject(name);
        if (val)
        {
            Destroy(o);
            await objCont.UpdateProperties(name);
        }
        else
        {
            await objCont.AddObjectAsync(o);
        }
    }

    public async Task SpawnInBoxes()
    {
        for (int i = 0; i < boxPositions.Length; i++)
        {
            // spawn to default position?
            GameObject o = Instantiate(boxes[i % boxes.Length], boxPositions[i].transform.position, boxPositions[i].transform.rotation, this.transform);
            var uph = o.GetComponent<InputPropertiesHandler>();
            uph.objCont = objCont;
            uph.StartPosition();
            string name = "CardboardBox_" + objCont.clientName.Value + "_" + i;
            o.name = name;

            // send/update to server

            bool val = await objCont.ContainsObject(name);
            if (val)
            {
                Destroy(o);
                await objCont.UpdateProperties(name);
            }
            else
            {
                await objCont.AddObjectAsync(o);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal async Task DeleteBoxesFromServer()
    {
        List<GameObject> gol = new List<GameObject>();

        foreach (Transform child in serverObjectParent.transform)
        {
            if (child.gameObject.name.StartsWith("CardboardBox_" + objCont.clientName.Value))
                gol.Add(child.gameObject);
        }

        Debug.Log(gol.Count);

        await objCont.RemoveObjects(gol);
    }
}
