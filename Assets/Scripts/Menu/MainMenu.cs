using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public static MainMenu instance;

    [Header("Paineis de Selecao ---------")]
    public GameObject raceSetupPanel;
    public GameObject trackSelectPanel;
    public GameObject racerSelectPanel;

    [Header("Images ---------")]
    public Image trackSelectImage;
    public Image racerSelectImage;
    
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null){
            instance = this;
        }
    }

    private void Start() {
        if(RaceInfoManager.instance.enteredRace){
            trackSelectImage.sprite = RaceInfoManager.instance.trackSprite;
            racerSelectImage.sprite = RaceInfoManager.instance.racerSprite;

            OpenRaceSetup();
        }

        PlayerPrefs.SetInt(RaceInfoManager.instance.trackToLoad+"_unlocked", 1);
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.P)){
            PlayerPrefs.DeleteAll();
            print("Keys deleted");
            //PlayerPrefs.SetInt(RaceInfoManager.instance.trackToLoad+"_unlocked", 1);
        }
        #endif
    }

    public void StartGame(){
        RaceInfoManager.instance.enteredRace = true;
        SceneManager.LoadScene(RaceInfoManager.instance.trackToLoad);
    }

    public void QuitGame(){
        Application.Quit();
    }

    // RaceSetup --------------------------------------
    public void OpenRaceSetup(){
        raceSetupPanel.SetActive(true);
    }

    public void CloseRaceSetup(){
        raceSetupPanel.SetActive(false);
    }

    // TrackSelect -------------------------------------
    public void OpenTrackSelect(){
        trackSelectPanel.SetActive(true);
        CloseRaceSetup();
    }

    public void CloseTrackSelect(){
        trackSelectPanel.SetActive(false);
        OpenRaceSetup();
    }

    // RacerSelect -------------------------------------
    public void OpenRacerSelect(){
        racerSelectPanel.SetActive(true);
        CloseRaceSetup();
    }

    public void CloseRacerSelect(){
        racerSelectPanel.SetActive(false);
        OpenRaceSetup();
    }
}
