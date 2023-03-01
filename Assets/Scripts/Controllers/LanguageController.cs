using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageController : MonoBehaviour
{
    [Header("Text")]
    [SerializeField()]
    TMP_Text quitTXT;
    [SerializeField()]
    TMP_Text instructionsTXT;

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
    string contCZ = "Ovládání";
    string contEN = "Controls";

    string quitCZ = "Ukonèit aplikaci?";
    string quitEN = "Do you want to quit?";
    string yesCZ = "Ano";
    string yesEN = "Yes";
    string noCZ = "Ne";
    string noEN = "No";

    string instructionsCZ = "Najdi všech 9 schovaných krabic!";
    string instructionsEN = "Find all 9 hidden boxes!";

    string langCZ = "EN";
    string langEN = "CZ";

    // Start is called before the first frame update
    void Start()
    {
        lang = "CZ";
        SetLabels();
        SetControls();
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
}
