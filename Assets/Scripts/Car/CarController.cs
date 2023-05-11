using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody theRB;
    public float maxSpeed;

    public float forwardAccel = 8f, reverseAccel = 4f;
    
    private float speedInput;

    public float turnStrength = 180f;

    private float turnInput;

    [SerializeField]private bool grounded;
    
    public Transform groundRayPoint, groundRayPoint2;
    public LayerMask whatIsGround;
    public float groundRayLength = .75f;

    // arrasto nas rampas ------------------------------------------
    private float dragOnGround;
    public float gravityMod = 10f;

    // rodas do carro ----------------------------------------------
    public Transform leftFrontWheel, rightFrontWheel;
    public float maxWheelTurn = 25f;

    // particle system ----------------------------------------------
    [Header("Particle System ------------------- ")]
    public ParticleSystem[] dustTrail;
    public float maxEmission = 25f, emissionFadeSpeed = 20f;
    private float emissionRate;

    public AudioSource engineSound, skidSound;
    public float skidFadeSpeed = 2;

    // checkpoint na pista -----------------------------------------
    private int nextCheckpoint;
    [Header("Volta Atual ------------------- ")]
    public int currentLap;
    
    [Header("Contagem de tempo -------------- ")]
    public float lapTime;
    public float bestLapTime;

    [Header("AI System ---------------------- ")]
    public bool isAI;
    public int currentTarget;
    private Vector3 targetPoint;
    public float aiAccelerateSpeed = 1f;
    public float aiTurnSpeed = .8f;
    public float aiReachPointRange = 5f;
    public float aiPointVariance = 3f;
    public float aiMaxTurn = 15f;
    private float aiSpeedInput, aiSpeedMod;

    // Start is called before the first frame update
    void Start()
    {
        theRB.transform.parent = null;
        dragOnGround = theRB.drag;

        if(isAI){   // pega a posição do proximo target
            targetPoint = RaceManager.instance.allCheckpoints[currentTarget].transform.position;
            RandomiseAITarget();

            aiSpeedMod = Random.Range(.8f, 1.1f);   // adiciona velocidades diferentes ao carros
        }

        if(!isAI){
            UIManager.instance.LapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;
        }
    }

    // Update is called once per frame
    void Update()
    {

        lapTime += Time.deltaTime;

        if(!isAI){

            var ts = System.TimeSpan.FromSeconds(lapTime);
            UIManager.instance.currentLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s",ts.Minutes,ts.Seconds,ts.Milliseconds);

            speedInput = 0f;
            if(Input.GetAxis("Vertical") > 0){
                speedInput = Input.GetAxis("Vertical") * forwardAccel;
            }else if(Input.GetAxis("Vertical") < 0){
                speedInput = Input.GetAxis("Vertical") * reverseAccel;
            }

            turnInput = Input.GetAxis("Horizontal");
            // if(grounded && Input.GetAxis("Horizontal") != 0){
            //     // Mathf.Sign(speedInput) usando na marcha re invertendo o lado que o carro vai
            //     transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude / maxSpeed), 0f));
            // }

        }else{
            targetPoint.y = transform.position.y;

            if(Vector3.Distance(transform.position, targetPoint) < aiReachPointRange){
                SetNextAITarget();  // indica o proximo target da AI
            }

            // rotação do carro na direção do target ------------------------------
            Vector3 targetDir = targetPoint - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);

            Vector3 localPos = transform.InverseTransformPoint(targetPoint);
            if(localPos.x < 0f){
                angle = -angle;
            }

            turnInput = Mathf.Clamp(angle/aiMaxTurn, -1f, 1f);  // limita rotação do carro

            if(Mathf.Abs(angle) < aiMaxTurn){
                aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, 1f, aiAccelerateSpeed);
            }else{
                aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, aiTurnSpeed, aiAccelerateSpeed);
            }


            // faz as AI acelerar
            //aiSpeedInput = 1f;
            speedInput = aiSpeedInput * forwardAccel * aiSpeedMod;

        }

        // turning the wheels
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), rightFrontWheel.localRotation.eulerAngles.z);

        //transform.position = theRB.position;

        // control particle emissions
        emissionRate = Mathf.MoveTowards(emissionRate, 0f, emissionFadeSpeed * Time.deltaTime);

        // se tiver no chao e movendo, cria mais particulas
        if(grounded && (Mathf.Abs(turnInput) > .5f || (theRB.velocity.magnitude < maxSpeed * .5f && theRB.velocity.magnitude != 0))){
            emissionRate = maxEmission;
        }
        
        // não deixa fazer particulas se tiver parado
        if(theRB.velocity.magnitude <= 0.5f){
            emissionRate = 0;
        }

        for(int i = 0; i < dustTrail.Length; i++){
            var emissionModule = dustTrail[i].emission;
            emissionModule.rateOverTime = emissionRate;
        }

        if(engineSound != null){
            engineSound.pitch = 1f + (theRB.velocity.magnitude /maxSpeed) * 2f;
        }

        if(skidSound != null){
            if(Mathf.Abs(turnInput) > 0.5f){
                skidSound.volume = 1f;
            }else{
                skidSound.volume = Mathf.MoveTowards(skidSound.volume, 0f, skidFadeSpeed * Time.deltaTime);
            }
        }
    }

    private void FixedUpdate() {

        grounded = false;

        RaycastHit hit;
        Vector3 normalTarget = Vector3.zero;

        if(Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround)){
            grounded = true;

            normalTarget = hit.normal;
        }

        if(Physics.Raycast(groundRayPoint2.position, -transform.up, out hit, groundRayLength, whatIsGround)){
            grounded = true;

            normalTarget = (normalTarget + hit.normal) / 2f;
        }

        // when on ground rotate to match the normal
        // faz rotação do carro nas rampas
        if(grounded){
            transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget) * transform.rotation;
        }

        // accelerates the car
        if(grounded){
            theRB.drag = dragOnGround;
            theRB.AddForce(transform.forward * speedInput * 1000f);
        }else{
            theRB.drag = .1f;
            theRB.AddForce(-Vector3.up * gravityMod * 100f);
        }

        if(theRB.velocity.magnitude > maxSpeed){    // limita velocidade do carro pelo maxSpeed
            theRB.velocity = theRB.velocity.normalized * maxSpeed;
        }

        //Debug.Log(theRB.velocity.magnitude);  // mostra a velocidade do carro

        // movimenta para os lados o carro ----------------------
        transform.position = theRB.position;
        //if(grounded && Input.GetAxis("Horizontal") != 0){
        if(grounded && speedInput != 0){
            // Mathf.Sign(speedInput) usando na marcha re invertendo o lado que o carro vai
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude / maxSpeed), 0f));
        }
        
    }

    public void CheckpointHit(int cpNumber){
        //Debug.Log(cpNumber);
        if(cpNumber == nextCheckpoint){
            nextCheckpoint++;

            // zera checkpoint ao passar pelo ultimo
            if(nextCheckpoint == RaceManager.instance.allCheckpoints.Length){
                nextCheckpoint = 0;
                LapCompleted(); // adiciona 1 volta
            }
        }

        if(isAI){
            if(cpNumber == currentTarget){
                SetNextAITarget();
            }
        }
    }

    public void SetNextAITarget(){
        currentTarget++;
        if(currentTarget >= RaceManager.instance.allCheckpoints.Length){
            currentTarget = 0;
        }

        // pega a posição do proximo target
        targetPoint = RaceManager.instance.allCheckpoints[currentTarget].transform.position;
        RandomiseAITarget();
    }

    public void LapCompleted(){
        currentLap++;

        if(lapTime < bestLapTime || bestLapTime == 0f){
            bestLapTime = lapTime;
        }

        lapTime = 0f;

        if(!isAI){

            var ts = System.TimeSpan.FromSeconds(bestLapTime);
            UIManager.instance.bestLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s",ts.Minutes,ts.Seconds,ts.Milliseconds);

            UIManager.instance.LapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;
        }
    }

    public void RandomiseAITarget(){
        // adiciona valores xyz para evitar que todos os carros vá na mesma posição do target
        targetPoint += new Vector3(Random.Range(-aiPointVariance, aiPointVariance), 0f, Random.Range(-aiPointVariance, aiPointVariance));
    }
}
