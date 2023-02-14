using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    [SerializeField]
    RigController rigSpawner;

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

    public void OnDisconnected()
    {
        objCont.ObjectsClear();
        Debug.Log("Disconnected - Launching restart procedure");
        StartCoroutine(RestartConnection());
    }

    public void OnReconnecting()
    {
        Debug.Log("Connection temporarily lost");
    }

    public void OnReconnected()
    {
        objCont.ObjectsClear();
        StartCoroutine(SyncCall());
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


        var res = GetObjectsAsync();
        while (!res.IsCompleted)
        {
            yield return null;
        }

        if (res.Result)
            Debug.Log("Finished sync call");
        else
            Debug.Log("Sync call unsuccessful");
    }

    /// <summary>
    /// Get objects from server
    /// - filter lines and display them
    /// </summary>
    private async Task<bool> GetObjectsAsync()
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

                var uph = obj.GetComponent<InputPropertiesHandler>();
                uph.objCont = objCont;
                uph.serverConnection = this;
                uph.StartPosition();
                Debug.Log("Recieved " + n);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            syncCallDone = true;
            return false;
        }

        syncCallDone = true;
        // bxSpawner.SpawnInBoxes();
        return true;
    }

    public void SpawnLocalObjects()
    {
        StartCoroutine(SpawnObjectsCorout());
    }

    IEnumerator SpawnObjectsCorout()
    {
        yield return new WaitUntil(() => syncCallDone);
        
        var t = bxSpawner.SpawnInBoxes();
        var tr = rigSpawner.SpawnRig();

        while (!t.IsCompleted || !tr.IsCompleted)
            yield return null;

        // set rig movement controlls
        
        // left hand
        var lhr = GameObject.Find("LeftHand Controller");
        var lhs = GameObject.Find(rigSpawner.handLNM);
        var mc = lhs.AddComponent<MoveController>();
        mc.parent = lhr.transform;

        // right hand
        var rhr = GameObject.Find("RightHand Controller");
        var rhs = GameObject.Find(rigSpawner.handRNM);
        mc = rhs.AddComponent<MoveController>();
        mc.parent = rhr.transform;

        // head
        var hr = GameObject.Find("Main Camera");
        var hs = GameObject.Find(rigSpawner.headNM);
        mc = hs.AddComponent<MoveController>();
        mc.parent = hr.transform;
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
        StartCoroutine(DestroyObjectCoroutine(name, obj));

    }

    IEnumerator DestroyObjectCoroutine(string name, GameObject obj)
    {
        var t = objCont.DestroyObject(name, obj);
        while (!t.IsCompleted)
            yield return null;
    }

}
