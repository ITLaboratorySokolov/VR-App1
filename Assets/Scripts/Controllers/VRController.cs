using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class VRController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StopXR();
        // XRGeneralSettings.Instance.Manager.StopSubsystems();
        //XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        // StartCoroutine(StopCardboard());
    }

    IEnumerator StartXR()
    {
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Initializing XR Failed. Check Editor or Player log for details.");
        }
        else
        {
            Debug.Log("Starting XR...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            yield return null;
        }
    }

    void StopXR()
    {
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            Camera.main.ResetAspect();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        //XRGeneralSettings.Instance.Manager.InitializeLoader();
        // XRSettings.enabled = true;
        // XRGeneralSettings.Instance.Manager.StartSubsystems();
    }
}
