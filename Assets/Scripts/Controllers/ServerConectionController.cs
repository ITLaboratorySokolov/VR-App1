using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Serialization.Mesh;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Session;

/// <summary>
/// Script used to controll connection to server
/// - handles connecting, disconnecting, reconnecting
/// </summary>
public class ServerConectionController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField]
    ObjectController objCont;
    [SerializeField]
    BoxSpawner bxSpawner;
    [SerializeField]
    RigController rigSpawner;
    [SerializeField]
    MinigameController minigame;

    [Header("Serializers")]
    RawMeshSerializer serializer;

    [Header("Connection")]
    /// <summary> Session </summary>
    [SerializeField]
    SignalRSessionWrapper session;
    /// <summary> Synchronization call has been finished </summary>
    internal bool syncCallDone;

    [Header("Actions")]
    /// <summary> Action performed upon Start </summary>
    [SerializeField]
    UnityEvent actionStart = new UnityEvent();
    /// <summary> Action performed upon Destroy </summary>
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

    /// <summary>
    /// On disconnected from server
    /// - clear local objects
    /// - start reconnect procedure
    /// </summary>
    public void OnDisconnected()
    {
        objCont.ObjectsClear();
        rigSpawner.SpawnRig();

        Debug.Log("Disconnected - Launching restart procedure");

        StartCoroutine(RestartConnection());
    }

    /// <summary>
    /// On reconnecting to server
    /// </summary>
    public void OnReconnecting()
    {
        syncCallDone = false;

        Debug.Log("Connection temporarily lost");
    }

    /// <summary>
    /// On reconnected to server
    /// </summary>
    public void OnReconnected()
    {
        objCont.ObjectsClear();

        StartCoroutine(SyncCall());
        SpawnLocalObjects();
    }

    /// <summary>
    /// Restarting procedure
    /// - creates a 5s delay betweem attempts
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
            // Get all objects
            IEnumerable<GameObject> gmobjs = await objCont.ObjectRecieve();

            Debug.Log("Got objects");

            foreach (GameObject obj in gmobjs)
            {
                string n = obj.name;

                Debug.Log(n);

                var uph = obj.GetComponent<ObjectPropertiesHandler>();
                uph.objCont = objCont;
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

    /// <summary>
    /// Spawn local objects
    /// </summary>
    public void SpawnLocalObjects()
    {
        StartCoroutine(SpawnBoxesCorout());
        StartCoroutine(SpawnRigCorout());
    }

    /// <summary>
    /// Spawn local boxes
    /// </summary>
    IEnumerator SpawnBoxesCorout()
    {
        yield return new WaitUntil(() => syncCallDone);
        
        var t = bxSpawner.SpawnInBoxes();

        while (!t.IsCompleted)
            yield return null;

        Debug.Log("Spawned local boxes");
    }

    /// <summary>
    /// Spawn rig and send it to server
    /// </summary>
    IEnumerator SpawnRigCorout()
    {
        yield return new WaitUntil(() => syncCallDone);

        var tr = rigSpawner.AddRigToServer();

        while (!tr.IsCompleted)
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

        rigSpawner.SwapColor(true);
    }

    /// <summary>
    /// Action called on ending the application
    /// </summary>
    public void OnDestroy()
    {
        actionEnd.Invoke();
    }

    /// <summary>
    /// On application exit
    /// </summary>
    internal void OnExit()
    {
        StartCoroutine(ExitCorout());
    }

    /// <summary>
    /// Exit coroutine
    /// - remove rig from server and exit application
    /// </summary>
    private IEnumerator ExitCorout()
    {
        yield return StartCoroutine(RemoveRigCorout());
        Application.Quit();
    }

    /// <summary>
    /// Remove rig from server
    /// </summary>
    IEnumerator RemoveRigCorout()
    {
        var tr = rigSpawner.RemoveRigFromServer();

        while (!tr.IsCompleted)
            yield return null;
    }

    /// <summary>
    /// Reset local boxes
    /// - delete all boxes
    /// - spawn them again at default locations
    /// </summary>
    public void RespawnBoxes()
    {
        Debug.Log("Respawning");

        if (session.State != SessionState.Connected)
            return;

        syncCallDone = false;
        minigame.ResetCount();
        StartCoroutine(DeleteBoxesFromServer());
        StartCoroutine(SpawnBoxesCorout());

        Debug.Log("Reset local boxes");
    }
    
    /// <summary>
    /// Delete local boxes
    /// </summary>
    public void ClearBoxes()
    {
        Debug.Log("Clearing");

        if (session.State != SessionState.Connected)
            return;

        minigame.ResetCount();
        StartCoroutine(DeleteBoxesFromServer());
    }

    /// <summary>
    /// Delete boxes from server
    /// </summary>
    private IEnumerator DeleteBoxesFromServer()
    {
        Task t = bxSpawner.DeleteBoxesFromServer();

        while (!t.IsCompleted)
            yield return null;

        Debug.Log("Deleted boxes from server");
        syncCallDone = true;
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

    /// <summary>
    /// Destroy object coroutine
    /// </summary>
    /// <param name="name"> Name of object </param>
    /// <param name="obj"> Game object </param>
    IEnumerator DestroyObjectCoroutine(string name, GameObject obj)
    {
        var t = objCont.DestroyObject(name, obj);
        while (!t.IsCompleted)
            yield return null;
    }

}
