using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;

/// <summary>
/// Script used to control spawning of boxes
/// </summary>
public class BoxSpawner : MonoBehaviour
{
    /// <summary> Prefabs of boxes to spawn </summary>
    [SerializeField]
    GameObject[] boxes;
    /// <summary> Where should be boxes spawned </summary>
    [SerializeField]
    GameObject[] boxPositions;
    /// <summary> Object controller </summary>
    [SerializeField]
    ObjectController objCont;
    /// <summary> Parent of spawned boxes </summary>
    [SerializeField]
    GameObject serverObjectParent;
    /// <summary> Tag of spawned boxes </summary>
    [SerializeField]
    StringVariable serverObjectTag;

    /// <summary>
    /// Spawn one test box
    /// [DEBUG METHOD]
    /// </summary>
    public void SpawnTestBox()
    {
        StartCoroutine(TestCorout());
    }

    /// <summary>
    /// Spawn test box coroutine
    /// [DEBUG METHOD]
    /// </summary>
    public IEnumerator TestCorout()
    {
        var t = SpawnTestBoxT();

        while (!t.IsCompleted)
            yield return null;

        Debug.Log("Done spawning test box");
    }

    /// <summary>
    /// Task to spawn test box
    /// [DEBUG METHOD]
    /// </summary>
    public async Task SpawnTestBoxT()
    {
        GameObject o = Instantiate(boxes[0], boxPositions[0].transform.position, boxPositions[0].transform.rotation, this.transform);
        var uph = o.GetComponent<InputPropertiesHandler>();
        uph.objCont = objCont;
        uph.StartPosition();
        string name = "CardboardBox_Test";
        o.name = name;
        o.tag = serverObjectTag.Value;
        
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

    /// <summary>
    /// Spawn boxes on specified locations in scene
    /// </summary>
    public async Task SpawnInBoxes()
    {
        for (int i = 0; i < boxPositions.Length; i++)
        {
            Debug.Log("Spawning box " + i);

            // spawn to default position?
            GameObject o = Instantiate(boxes[i % boxes.Length], boxPositions[i].transform.position, boxPositions[i].transform.rotation, this.transform);
            var uph = o.GetComponent<InputPropertiesHandler>();
            uph.objCont = objCont;
            uph.StartPosition();
            string name = "CardboardBox_" + objCont.clientName.Value + "_" + i;
            o.name = name;
            o.tag = serverObjectTag.Value;

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

    /// <summary>
    /// Delete boxes from server and local space
    /// </summary>
    /// <returns></returns>
    internal async Task DeleteBoxesFromServer()
    {
        List<GameObject> gol = new List<GameObject>();

        // find all object with specified tag
        var allGOs = GameObject.FindGameObjectsWithTag(serverObjectTag.Value);
        foreach (GameObject go in allGOs) 
        {
            if (go.name.StartsWith("CardboardBox_" + objCont.clientName.Value))
                gol.Add(go);
        }

        Debug.Log(gol.Count);
        await objCont.RemoveObjects(gol);
    }
}
