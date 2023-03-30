using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZCU.TechnologyLab.Common.Connections.Client.Session;

public class LanguageController : MonoBehaviour
{
    [Header("Text")]
    [SerializeField()]
    TMP_Text quitTXT;
    [SerializeField()]
    TMP_Text instructionsTXT;
    [SerializeField()]
    TMP_Text minigameTXT;

    [Header("Buttons")]
    [SerializeField()]
    Button deleteBoxesBT;
    [SerializeField()]
    Button refreshBoxesBT;
    [SerializeField()]
    Button yesBT;
    [SerializeField()]
    Button noBT;
    [SerializeField()]
    Button langBT;

    [Header("Controls")]
    [SerializeField()]
    GameObject controlsCZ;
    [SerializeField()]
    GameObject controlsEN;
    [SerializeField()]
    Button controlsBT;

    [Header("Language")]
    internal string lang;

    [Header("Strings")]
    string deleteBoxesCZ = "Smazat krabice";
    string deleteBoxesEN = "Delete boxes";
    string resetCZ = "Reset krabic";

    string resetEN = "Reset boxes";
    string contCZ = "Ovl·d·nÌ";
    string contEN = "Controls";

    string quitCZ = "UkonËit aplikaci?";
    string quitEN = "Do you want to quit?";
    string yesCZ = "Ano";
    string yesEN = "Yes";
    string noCZ = "Ne";
    string noEN = "No";

    string instructionsCZ = "HLED¡ SE POMOC";
    string instructionsEN = "HELP WANTED";

    string wonCZ = "⁄KOL SPLNÃN";
    string wonEN = "GOAL REACHED";

    string plotCZ = @"P¯Ìstroj zajiöùujÌcÌ umÏlou gravitaci v dÛmu se rozbil!

Automechanik za mÏstem tvrdÌ, ûe ho zvl·dne opravit, ale nÏkdo ukradl krabice s pot¯ebn˝mi souË·stami a rozh·zel je po celÈm mÏstÏ...

Pom˘ûeö n·m najÌt 6 krabic se souË·stkami a doruËÌö je automechanikovi?
";
    string plotEN = @"The artificial gravity machine has broken in the dome!

The automechanic behind town claims he can fix it, but somebody has stolen boxes with needed parts and scattered them through the town...

Can you help us find 6 boxes with parts and deliver them to the automechanic?
";

    string langCZ = "EN";
    string langEN = "CZ";

    // Start is called before the first frame update
    void Start()
    {
        lang = "CZ";
        SetLabels();
        SetControls();
    }


    internal string GetGoalText()
    {
        if (lang == "CZ")
            return wonCZ;
        else if (lang == "EN")
            return wonEN;

        return "!!!";
    }

    private void SetControls()
    {
        if (lang == "CZ")
        {
            controlsCZ.SetActive(true);
            controlsEN.SetActive(false);
        }
        else if (lang == "EN")
        {
            controlsEN.SetActive(true);
            controlsCZ.SetActive(false);
        }
    }

    /// <summary>
    /// Set labels to czech or english texts
    /// </summary>
    private void SetLabels()
    {
        if (lang == "CZ")
        {
            quitTXT.text = quitCZ;
            instructionsTXT.text = instructionsCZ;

            deleteBoxesBT.GetComponentInChildren<TMP_Text>().text = deleteBoxesCZ;
            refreshBoxesBT.GetComponentInChildren<TMP_Text>().text = resetCZ;
            langBT.GetComponentInChildren<TMP_Text>().text = langCZ;
            controlsBT.GetComponentInChildren<TMP_Text>().text = contCZ;
            yesBT.GetComponentInChildren<TMP_Text>().text = yesCZ;
            noBT.GetComponentInChildren<TMP_Text>().text = noCZ;

            if (minigameTXT.text == wonEN)
                minigameTXT.text = wonCZ;

        }

        else if (langCZ == "EN")
        {
            quitTXT.text = quitEN;
            instructionsTXT.text = instructionsEN;

            deleteBoxesBT.GetComponentInChildren<TMP_Text>().text = deleteBoxesEN;
            refreshBoxesBT.GetComponentInChildren<TMP_Text>().text = resetEN;
            langBT.GetComponentInChildren<TMP_Text>().text = langEN;
            controlsBT.GetComponentInChildren<TMP_Text>().text = contEN;
            yesBT.GetComponentInChildren<TMP_Text>().text = yesEN;
            noBT.GetComponentInChildren<TMP_Text>().text = noEN;

            if (minigameTXT.text == wonCZ)
                minigameTXT.text = wonEN;
        }
    }

    /// <summary>
    /// Swap languages between CZ and EN
    /// </summary>
    internal void SwapLanguages()
    {
        if (lang == "CZ")
            lang = "EN";
        else if (lang == "EN")
            lang = "CZ";

        SetLabels();
        SetControls();
    }

    internal string GetSessionStateString(SessionState state)
    {
        if (lang == "CZ")
        {
            if (state == SessionState.Closed)
                return "Odpojeno";
            if (state == SessionState.Connected)
                return "P¯ipojeno";
            if (state == SessionState.Reconnecting)
                return "P¯ipojov·nÌ";
            if (state == SessionState.Closing)
                return "Odpojov·nÌ";
            if (state == SessionState.Starting)
                return "ZaËÌn·";
        }

        return state.ToString();
    }
}
