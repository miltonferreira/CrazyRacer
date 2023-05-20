using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{

    public static RaceManager instance;

    public Checkpoint[] allCheckpoints;

    [Header("Total de Voltas --------------- ")]
    public int totalLaps;
    [Header("CarController --------------- ")]
    public CarController playerCar;
    [Header("Total IA Cars --------------- ")]
    public List<CarController> allAICars = new List<CarController>();
    public int playerPosition;

    public float timeBetweenPosCheck = .2f;
    private float posChkCounter;

    [Header("Velocidade dos carros")]
    public float aiDefaultSpeed = 30f;
    public float playerDefaultSpeed = 30f;
    [Header("Borracha dos carros")]
    public float rubberBandSpeedMod = 3.5f;
    public float rubBandAccel = .5f;

    [Header("Controle de largada")]
    public bool isStarting;
    public float timeBetweenStartCount = 1f;
    private float startCounter;
    public int countdownCurrent = 3;

    private void Awake() {
        if(instance == null){
            instance = this;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < allCheckpoints.Length; i++){
            allCheckpoints[i].cpNumber = i;
        }

        isStarting = true;
        startCounter = timeBetweenStartCount;   // tempo inicial de contagem regressiva

        UIManager.instance.countdownText.text = countdownCurrent+"!";
    }

    // Update is called once per frame
    void Update(){

        if(isStarting){

            // contagem para largada
            startCounter -= Time.deltaTime;
            if(startCounter < 0){

                countdownCurrent--;
                startCounter = timeBetweenStartCount;

                UIManager.instance.countdownText.text = countdownCurrent+"!";

                if(countdownCurrent == 0){
                    isStarting = false;
                    UIManager.instance.countdownText.gameObject.SetActive(false);   // desativa contagem
                    UIManager.instance.goText.gameObject.SetActive(true);           // ativa GO
                }
            }

        }else{
        
            posChkCounter -= Time.deltaTime;

            if(posChkCounter <= 0){

                playerPosition = 1; // posicao player na corrida

                foreach(CarController aiCar in allAICars){
                    if(aiCar.currentLap > playerCar.currentLap){                // compara total de voltas
                        playerPosition++;
                    }else if(aiCar.currentLap == playerCar.currentLap){
                        if(aiCar.nextCheckpoint > playerCar.nextCheckpoint){    // compara o checkpoint da volta
                            playerPosition++;
                        } else if(aiCar.nextCheckpoint == playerCar.nextCheckpoint){
                                                                                // compara quem está mais proximo do proximo checkpoint
                            if(Vector3.Distance(aiCar.transform.position, allCheckpoints[aiCar.nextCheckpoint].transform.position) <
                                Vector3.Distance(playerCar.transform.position, allCheckpoints[aiCar.nextCheckpoint].transform.position)){
                                    playerPosition++;
                                }
                        }
                    }
                }

                posChkCounter = timeBetweenPosCheck;

                // mostra posicao do player na corrida
                UIManager.instance.positionText.text = playerPosition+"/"+(allAICars.Count+1); // +1 é o player

            }

            // manage rubber banding **Boost para o player ou AI**
            if(playerPosition == 1){

                foreach(CarController aiCar in allAICars){
                    aiCar.maxSpeed = Mathf.MoveTowards(aiCar.maxSpeed,
                        aiDefaultSpeed + rubberBandSpeedMod, rubBandAccel * Time.deltaTime);
                }

                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed,
                    playerDefaultSpeed - (rubberBandSpeedMod * ((float)playerPosition / ((float)allAICars.Count+1))), rubBandAccel * Time.deltaTime);
            
            }else{
                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed,
                    playerDefaultSpeed + (rubberBandSpeedMod * ((float)playerPosition / ((float)allAICars.Count+1))), rubBandAccel * Time.deltaTime);            
            }
        }


    }
}
