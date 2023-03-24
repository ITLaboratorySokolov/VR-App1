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
    [SerializeField]
    Vec2FVariable roomSize;

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

    private void Start()
    {
        float wX = roomSize.Value.x;
        float wZ = roomSize.Value.y;

        if (wX != 0 && wZ != 0)
            roomController.SetRoomSize(wX, wZ);
    }

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

    public void ResetSetUp(InputAction.CallbackContext ctx)
    {
        Debug.Log("Reseting configuration!");
        ReadConfig();
        serverConnection.ResetConnection();
    }

}
