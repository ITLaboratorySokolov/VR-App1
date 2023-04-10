using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;

/// <summary>
/// Script managing the set up of the application
/// - reads config file
/// </summary>
public class SetUpScript : MonoBehaviour
{
    [Header("Config")]
    /// <summary> Path to config file </summary>
    string pathToConfig;

    [Header("Server connection")]
    /// <summary> Server url </summary>
    [SerializeField]
    private StringVariable serverUrl;
    [SerializeField]
    private StringVariable clientName;
    /// <summary> Server connection </summary>
    [SerializeField]
    ServerConectionController serverConnection;
    /// <summary> Manual connection reset action reference </summary>
    [SerializeField]
    InputActionReference resetAction = null;

    [Header("Controllers")]
    [SerializeField]
    RoomController roomController;
    [SerializeField]
    Vec2FVariable roomSize;

    /// <summary>
    /// Set up configuration before application starts
    /// - set room size
    /// </summary>
    private void Start()
    {
        float wX = roomSize.Value.x;
        float wZ = roomSize.Value.y;

        // Set culture -> doubles are written with decimal dot
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

        if (wX != 0 && wZ != 0)
            roomController.SetRoomSize(wX, wZ);

        if (resetAction != null)
            resetAction.action.performed += ResetSetUp;
    }

    /// <summary>
    /// Read config file
    /// </summary>
    private void ReadConfig()
    {
        if (File.Exists(pathToConfig))
        {
            Debug.Log("Loading config file...");
            string[] lines = File.ReadAllLines(pathToConfig);
            if (lines.Length >= 3)
            {
                // process client name
                clientName.Value = lines[0].Trim();

                // process url
                serverUrl.Value = lines[1].Trim();

                // process room size: "wX, wZ"
                string roomSize = lines[2].Trim();
                var sizes = roomSize.Split(',');
                float wX = 0;
                float wZ = 0;
                if (sizes.Length == 2)
                {
                    float.TryParse(sizes[0].Trim(), out wX);
                    float.TryParse(sizes[1].Trim(), out wZ);
                }
                if (wX != 0 && wZ != 0)
                {
                    roomController.SetRoomSize(wX, wZ);
                }

            }
        }
    }

    /// <summary>
    /// Reset configuration
    /// </summary>
    /// <param name="ctx"></param>
    public void ResetSetUp(InputAction.CallbackContext ctx)
    {
        Debug.Log("Reseting configuration!");
        ReadConfig();
        serverConnection.ResetConnection();
    }

}
