using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceInfoManager : MonoBehaviour
{
    public static RaceInfoManager instance;

    public string trackToLoad;
    public CarController racerToUse;
    public int noOfAI;  // numero de carros AI na corrida
    public int noOfLaps;
    public bool enteredRace;
    [Header("Sprites Track e Car escolhidos ----------")]
    public Sprite trackSprite;
    public Sprite racerSprite;

    private void Awake() {
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }else{
            Destroy(this.gameObject);
        }
    }

    
}
