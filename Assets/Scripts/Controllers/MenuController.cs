using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ZCU.TechnologyLab.Common.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Session;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    GameObject exitPanel;
    [SerializeField]
    GameObject controlPanel;
    [SerializeField]
    LanguageController langCont;

    [Header("Connection")]
    /// <summary> Text displaying connection status </summary>
    [SerializeField]
    TMP_Text connectionTXT;

    public void ToggleExitPanel(bool val)
    {
        exitPanel.SetActive(val);
    }

    public void OnQuit(ServerConectionController scc)
    {
        scc.OnExit();
    }

    public void SwitchLang()
    {
        langCont.SwapLanguages();
    }

    public void ToggleControlsPanel(bool val)
    {
        controlPanel.SetActive(val);
    }

    /// <summary>
    /// Display connection status
    /// </summary>
    /// <param name="connected"> Is connected to server </param>
    public void SetConnection(SignalRSessionWrapper session) // bool connected)
    {
        Debug.Log(session.State.ToString());

        string state = langCont.GetSessionStateString(session.State);

        if (session.State == SessionState.Connected)
            ChangeConnection(state, Color.green);
        else if (session.State == SessionState.Reconnecting)
            ChangeConnection(state, Color.yellow);
        else
            ChangeConnection(state, Color.red);
    }

    /// <summary>
    /// Change displayed connection status
    /// </summary>
    /// <param name="msg"> Message </param>
    /// <param name="c"> Colour of text </param>
    public void ChangeConnection(string msg, Color c)
    {
        connectionTXT.text = msg;
        connectionTXT.color = c;
    }
}
