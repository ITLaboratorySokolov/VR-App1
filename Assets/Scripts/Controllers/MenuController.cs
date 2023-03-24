using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    GameObject exitPanel;
    [SerializeField]
    GameObject controlPanel;
    [SerializeField]
    LanguageController langCont;

    public void ToggleExitPanel(bool val)
    {
        exitPanel.SetActive(val);
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    public void SwitchLang()
    {
        langCont.SwapLanguages();
    }

    public void ToggleControlsPanel(bool val)
    {
        controlPanel.SetActive(val);
    }
}
