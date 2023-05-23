using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{

    public static CameraSwitcher instance;

    public GameObject[] cameras;
    private int currentCam;

    public CameraController topDownCam;
    public Cinemachine.CinemachineVirtualCamera cineCam;

    private void Awake() {
        if(instance == null){
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C)){
            currentCam++;

            if(currentCam >= cameras.Length){
                currentCam = 0;
            }

            for(int i =0; i<cameras.Length; i++){
                if(i == currentCam){
                    cameras[i].SetActive(true);
                }else{
                    cameras[i].SetActive(false);
                }
            }
        }
    }


    public void SeTarget(CarController playercar){
        topDownCam.target = playercar;
        cineCam.m_Follow = playercar.transform;
        cineCam.m_LookAt = playercar.transform;
    }

}
