using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    [SerializeField]
    GameObject onlineHand;
    [SerializeField]
    GameObject offlineHand;

    /// <summary> Erase action </summary>
    [SerializeField]
    internal InputActionReference eraserRef = null;

    [SerializeField]
    ServerConectionController serverController;

    public void Start()
    {
        eraserRef.action.started += EraseObject;
    }

    public void OnlineHandDisplay()
    {
        offlineHand.SetActive(false);
        onlineHand.SetActive(true);
    }

    public void OfflineHandDisplay()
    {
        onlineHand.SetActive(false);
        offlineHand.SetActive(true);
    }

    void EraseObject(InputAction.CallbackContext ctx)
    {
        if (!onlineHand.activeInHierarchy)
            return;

        Collider[] cs = Physics.OverlapSphere(onlineHand.transform.position, onlineHand.transform.localScale.x);
        for (int i = 0; i < cs.Length; i++)
        {
            string name = cs[i].gameObject.name;
            // TODO funguje to?
            if (cs[i].gameObject.tag == "worldObject")
            {
                serverController.DestroyObjectOnServer(name, cs[i].gameObject);
                Debug.Log("Deleted " + name);
            }

        }
    }

}
