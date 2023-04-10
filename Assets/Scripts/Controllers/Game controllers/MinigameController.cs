using TMPro;
using UnityEngine;

/// <summary>
/// Script used to control the active minigame - collecting boxes in the scene and depositing
/// them into a dedicated area
/// </summary>
public class MinigameController : MonoBehaviour
{
    [SerializeField()]
    LanguageController langCont;
    
    /// <summary> Number of boxes that should be collected </summary>
    [SerializeField()]
    int boxesGoal;
    /// <summary> Number of boxes currently collected </summary>
    int boxesIn;

    /// <summary> Displayed text </summary>
    [SerializeField()]
    TMP_Text dispText;


    // Start is called before the first frame update
    void Start()
    {
        boxesIn = 0;
    }

    /// <summary>
    /// Reset the counter of boxes in dedicated area
    /// </summary>
    public void ResetCount()
    {
        boxesIn = 0;
        dispText.text = "0 / " + boxesGoal;
        dispText.color = Color.white;
    }

    /// <summary>
    /// If a box enters the dedicated area, add to the counter
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.StartsWith("CardboardBox_"))
        {
            boxesIn++;

            dispText.text = boxesIn + " / " + boxesGoal;

            // if goal met
            if (boxesGoal <= boxesIn)
            {
                dispText.text = langCont.GetGoalText();
                dispText.color = Color.green;
            }
        }
    }

    /// <summary>
    /// If a box exits dedicated area, decrease counter
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.StartsWith("CardboardBox_"))
        {
            boxesIn--;

            if (boxesGoal > boxesIn)
            {
                dispText.text = boxesIn + " / " + boxesGoal;
                dispText.color = Color.white;
            }
        }

    }

}
