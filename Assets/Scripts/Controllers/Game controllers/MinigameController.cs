using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MinigameController : MonoBehaviour
{
    [SerializeField()]
    LanguageController langCont;
    
    [SerializeField()]
    int boxesGoal;

    int boxesIn;

    [SerializeField()]
    TMP_Text dispText;


    // Start is called before the first frame update
    void Start()
    {
        boxesIn = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetCount()
    {
        boxesIn = 0;
        dispText.text = "0 / " + boxesGoal;
        dispText.color = Color.white;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.StartsWith("CardboardBox_"))
        {
            boxesIn++;

            dispText.text = boxesIn + " / " + boxesGoal;

            if (boxesGoal <= boxesIn)
            {
                dispText.text = langCont.GetGoalText();
                dispText.color = Color.green;
            }
        }
    }

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
