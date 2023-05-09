using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    public TMP_Text LapCounterText, bestLapTimeText, currentLapTimeText;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null){
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
