using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Velocidade dos carros  --------------- ")]
    public float aiDefaultSpeed = 30f;
    public float playerDefaultSpeed = 30f;
    [Header("Borracha dos carros  --------------- ")]
    public float rubberBandSpeedMod = 3.5f;
    public float rubBandAccel = .5f;

    [Header("Controle de largada  --------------- ")]
    public bool isStarting;
    public float timeBetweenStartCount = 1f;
    private float startCounter;
    public int countdownCurrent = 3;

    [Header("Posicoes Grid  --------------- ")]
    public Transform[] startPoints;
    public int playerStartPosition;
    public int aiNumberToSpawn;

    [Header("Posicoes AI Grid  --------------- ")]
    public List<CarController> carsToSpawn = new List<CarController>();

    public bool raceCompleted;

    [Header("Nome da cena")]
    public string raceCompletedScene;

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

        playerStartPosition = Random.Range(0, aiNumberToSpawn + 1);

        playerCar.transform.position = startPoints[playerStartPosition].position;
        playerCar.theRB.transform.position = startPoints[playerStartPosition].position;

        // colocar aiCars no grid
        for(int i = 0; i < aiNumberToSpawn+1; i++){
            if(i != playerStartPosition){

                int selectedCar = Random.Range(0, carsToSpawn.Count);   // escolhe um carro para respawna

                allAICars.Add(Instantiate(carsToSpawn[selectedCar], startPoints[i].position, startPoints[i].rotation));

                if(carsToSpawn.Count > aiNumberToSpawn - i){
                    carsToSpawn.RemoveAt(selectedCar);  // tirar da lista carros da lista de respawn
                }


            }
        }

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

    public void FinishRace(){
        raceCompleted = true;

        switch(playerPosition){
            case 1:
                UIManager.instance.raceResultText.text = "You Finished 1st";        
                break;
            case 2:
                UIManager.instance.raceResultText.text = "You Finished 2nd";        
                break;
            case 3:
                UIManager.instance.raceResultText.text = "You Finished 3rd";        
                break;
            default:
                UIManager.instance.raceResultText.text = "You Finished "+ playerPosition + "th";
                break;
        }

        UIManager.instance.resultScreen.SetActive(true);

    }

    public void ExitRace(){
        SceneManager.LoadScene(raceCompletedScene);
    }

}
