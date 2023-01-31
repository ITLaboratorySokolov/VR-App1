using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BoxSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] boxes;
    [SerializeField]
    GameObject[] boxPositions;
    [SerializeField]
    ObjectController objCont;

    // Start is called before the first frame update
    void Start()
    {

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
            string name = "CardboardBox_" + i;
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
}
