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
        XRGeneralSettings.Instance.Manager.StopSubsystems();
        //XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        // StartCoroutine(StopCardboard());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        //XRGeneralSettings.Instance.Manager.InitializeLoader();
        // XRSettings.enabled = true;
        XRGeneralSettings.Instance.Manager.StartSubsystems();
    }
}
