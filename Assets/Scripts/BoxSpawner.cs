using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] boxes;
    [SerializeField]
    ObjectController objCont;

    // Start is called before the first frame update
    void Start()
    {

    }

    public async void SpawnInBoxes()
    {
        for (int i = 0; i < boxes.Length; i++)
        {
            // spawn to default position?
            GameObject o = Instantiate(boxes[i], this.transform);
            string name = o.name;

            // send/update to server

            bool val = await objCont.ContainsObject(name);
            if (val)
            {
                Destroy(o);
                objCont.UpdateProperties(name);
            }
            else
            {
                objCont.AddObjectAsync(o);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
