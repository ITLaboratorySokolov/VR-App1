using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using ZCU.TechnologyLab.Common.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Serialization.Mesh;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers;

public class ServerConectionController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField]
    ObjectController objCont;
    [SerializeField]
    BoxSpawner bxSpawner;

    [Header("Serializers")]
    /// <summary> Mesh serializer </summary>
    RawMeshSerializer serializer;

    [Header("Connection")]
    /// <summary> Session </summary>
    [SerializeField]
    SignalRSessionWrapper session;
    /// <summary> Synchronization call has been finished </summary>
    internal bool syncCallDone;
    /// <summary> Highest number of line on server </summary>
    int serverLines;

    [Header("Actions")]
    /// <summary> Action performed upon Start </summary>
    [SerializeField]
    UnityEvent actionStart = new UnityEvent();
    /// <summary Action performed upon Destroy </summary>
    [SerializeField]
    UnityEvent actionEnd = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        actionStart.Invoke();
        serializer = new RawMeshSerializer();
    }

    /// <summary>
    /// Called when automatic connection to server fails
    /// - attempts to restart connection to server
    /// </summary>
    public void ConnectionFailed()
    {
        Debug.Log("Launching restart procedure");
        StartCoroutine(RestartConnection());
    }

    public void ConnectionLost()
    {
        Debug.Log("Lost connection");
    }

    /// <summary>
    /// Restarting procedure
    /// - creates a minimum 5s delay
    /// </summary>
    /// <returns></returns>
    IEnumerator RestartConnection()
    {
        yield return new WaitForSeconds(5);
        actionStart.Invoke();
    }

    /// <summary>
    /// Reset connection to server
    /// </summary>
    public void ResetConnection()
    {
        syncCallDone = false;
    }

    /// <summary>
    /// Called when successfully connected to server
    /// </summary>
    public void ConnectedToServer()
    {
        Debug.Log("Connected to server");
        StartCoroutine(SyncCall());
    }

    /// <summary>
    /// Starting synchronization call
    /// </summary>
    /// <returns> IEnumerator </returns>
    IEnumerator SyncCall()
    {
        yield return new WaitUntil(() => session.State == SessionState.Connected);
        GetObjectsAsync();
    }

    /// <summary>
    /// Get objects from server
    /// - filter lines and display them
    /// </summary>
    private async void GetObjectsAsync()
    {
        try
        {
            // TODO update local boxes with positions from server?
            // Get all objects
            IEnumerable<GameObject> gmobjs = await objCont.ObjectRecieve();

            // If object is recognized as a line
            foreach (GameObject obj in gmobjs)
            {
                string n = obj.name;

                // Filter out lines
                // if (n.StartsWith("CardboardBox"))
                {
                    var uph = obj.GetComponent<UpdatePropertiesHandler>();
                    uph.objCont = objCont;
                    uph.serverConnection = this;
                    uph.StartPosition();
                    Debug.Log("Recieved " + n);
                }
                // else
                {
                    // objCont.RemoveObjectFromLocal(n);
                    // Debug.Log("Removing " + n);
                }

            }
        }
        catch (Exception e)
        {
            Debug.LogError("Cannot sync call");
            Debug.LogError(e.Message);
        }

        syncCallDone = true;
        Debug.Log("Sync call done");
        bxSpawner.SpawnInBoxes();
    }

    /// <summary>
    /// Action called on ending the application
    /// </summary>
    public void OnDestroy()
    {
        actionEnd.Invoke();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Remove object from server and scene
    /// </summary>
    /// <param name="name"> Object name </param>
    /// <param name="obj"> Game object </param>
    internal void  DestroyObjectOnServer(string name, GameObject obj)
    {
        objCont.DestroyObject(name, obj);
    }
}
