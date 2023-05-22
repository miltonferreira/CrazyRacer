using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    public TMP_Text LapCounterText, bestLapTimeText, currentLapTimeText;

    [Header("Ref UI Position")]
    public TMP_Text positionText;

    [Header("Ref UI Largada")]
    public TMP_Text countdownText;
    public TMP_Text goText;

    [Header("Ref UI Resultado Corrida")]
    public TMP_Text raceResultText;
    public GameObject resultScreen;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null){
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitRace(){
        RaceManager.instance.ExitRace();
    }
}
