using TMPro;
using UnityEngine;
using ZCU.TechnologyLab.Common.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Session;

/// <summary>
/// Script controlling the menu in scsene
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Canvas objects")]
    [SerializeField]
    GameObject exitPanel;
    [SerializeField]
    GameObject controlPanel;

    [Header("Scripts")]
    [SerializeField]
    LanguageController langCont;

    [Header("Connection")]
    /// <summary> Text displaying connection status </summary>
    [SerializeField]
    TMP_Text connectionTXT;

    /// <summary>
    /// Toggle exit panel
    /// </summary>
    /// <param name="val"> True if exit panel on, false if off </param>
    public void ToggleExitPanel(bool val)
    {
        exitPanel.SetActive(val);
    }

    /// <summary>
    /// On Exit button clicked
    /// </summary>
    /// <param name="scc"> Server connection controller </param>
    public void OnQuit(ServerConectionController scc)
    {
        scc.OnExit();
    }

    /// <summary>
    /// Switch language
    /// </summary>
    public void SwitchLang()
    {
        langCont.SwapLanguages();
    }

    /// <summary>
    /// Toggle controls panel
    /// </summary>
    /// <param name="val"> True if controls panel on, false if off </param>
    public void ToggleControlsPanel(bool val)
    {
        controlPanel.SetActive(val);
    }

    /// <summary>
    /// Display connection status
    /// </summary>
    /// <param name="connected"> Is connected to server </param>
    public void SetConnection(SignalRSessionWrapper session) 
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
