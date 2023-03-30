using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;

public class ConfigController : MonoBehaviour
{
    [SerializeField]
    ConfigLanguageController langController;

    string pathToConfig;

    [SerializeField]
    string nextScene;

    [Header("Text")]
    [SerializeField]
    StringVariable serverUrl;
    [SerializeField]
    StringVariable clientName;
    [SerializeField]
    Vec2FVariable roomSizeVal;

    [Header("InputFields")]
    [SerializeField]
    TMP_InputField urlTXT;
    [SerializeField]
    TMP_InputField nameTXT;
    [SerializeField]
    TMP_InputField roomSizeXTXT;
    [SerializeField]
    TMP_InputField roomSizeZTXT;

    [Header("GameObjects")]
    [SerializeField]
    GameObject controlsPanel;

    [Header("ErrorIcons")]
    [SerializeField]
    GameObject errorNM;
    [SerializeField]
    GameObject errorURL;

    // Start is called before the first frame update
    void Start()
    {
        // read config
        pathToConfig = Directory.GetCurrentDirectory() + "\\config.txt";
        Debug.Log(pathToConfig);

        // Set culture -> doubles are written with decimal dot
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        ReadConfig();

        // display values
        DisplayValues();
    }

    private void DisplayValues()
    {
        urlTXT.text = serverUrl.Value;
        nameTXT.text = clientName.Value;

        if (roomSizeXTXT != null)
        {
            roomSizeXTXT.text = "" + roomSizeVal.Value.x;
            roomSizeZTXT.text = "" + roomSizeVal.Value.y;
        }
    }

    public void Play()
    {
        serverUrl.Value = urlTXT.text.Trim();
        clientName.Value = nameTXT.text.Trim();

        urlTXT.text = serverUrl.Value;
        nameTXT.text = clientName.Value;

        bool noClient = false;
        if (clientName.Value == null || clientName.Value.Length == 0)
            noClient = true;
        errorNM.SetActive(noClient);

        bool noUrl = false;
        if (serverUrl.Value == null || serverUrl.Value.Length == 0)
            noUrl = true;
        errorURL.SetActive(noUrl);

        if (noUrl || noClient)
            return;

        if (roomSizeXTXT != null)
        {
            float wX = 0;
            float wZ = 0;
            float.TryParse(roomSizeXTXT.text.Trim(), out wX);
            float.TryParse(roomSizeXTXT.text.Trim(), out wZ);
            roomSizeVal.Value = new Vector2(wX, wZ);
        }

        SceneManager.LoadScene(nextScene);
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
                    roomSizeVal.Value = new Vector2(wX, wZ);
                }

            }
        }
    }

    public void ToggleControlsPanel(bool val)
    {
        controlsPanel.SetActive(val);
    }

    /// <summary>
    /// Filter username
    /// - only a-zA-Z0-9_- allowed
    /// </summary>
    public void FilterName()
    {
        nameTXT.text = Regex.Replace(nameTXT.text, "[^a-zA-Z0-9_-]+", "", RegexOptions.Compiled);
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
