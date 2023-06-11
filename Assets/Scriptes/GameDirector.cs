using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameDirector : MonoBehaviour
{
    public float timeScale;

    public int[] Room_Lights = { 1, 1, 1, 1, 1, 1 };
    public int[] Room_Patients = { 1, 1, 1, 1, 1, 1 };

    [SerializeField]float check_Time = 3.0f; 
    [SerializeField]float delay_Time = 10.0f; //when the one light off, set delay
    [SerializeField]float patient_Limit_Time = 30.0f;
    GameObject patient_Limit_Timer;

    float delta = 0;
    float patient_delta = 0;
    float delay_delta = 0;
    [SerializeField] int TurnOFF_ratio = 5; //probability of light turn off randomly
    [SerializeField] int Disappear_ratio = 5; //probability of patient disappear randomly

    [SerializeField] bool one_light_off_immediately;
    [SerializeField] bool one_patient_disappear_immediately;
    bool Check_All_patient_exist;

    private void Awake()
    {
        var obj = FindObjectsOfType<GameDirector>();
        if (obj.Length == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        timeScale = 1;
        one_light_off_immediately = false;
        one_patient_disappear_immediately = false;
        Check_All_patient_exist = true;
        patient_Limit_Timer = GameObject.Find("Canvas_Timer");
    }

    private void Update()
    {
        Time.timeScale = timeScale; //Manage Fps
        delay_delta -= Time.deltaTime;
        if(delay_delta <= 0f)
        {
            Automatically_light_OFF();
            Automatically_Patient_disappear();
            delay_delta = 0f;
        }
       
        if(one_light_off_immediately == true || one_patient_disappear_immediately == true)
        {
            delay_delta = delay_Time;
            one_light_off_immediately = false;
            one_patient_disappear_immediately = false;
        }

        
        Check_Patient_disappear(); //Check Patient gone evey frame
        if(Check_All_patient_exist == false)//if patient gone, start limit timer
        {
            patient_delta += Time.deltaTime;
            if (patient_delta >= 1.0f)
            {
                
                patient_Limit_Time -= 1.0f;
                if (patient_Limit_Time <= 20f && patient_Limit_Time > 0f)
                {
                    patient_Limit_Timer.transform.Find("LimitTimer").gameObject.SetActive(true);
                    patient_Limit_Timer.transform.Find("LimitTimer").gameObject.GetComponent<Text>().text = "Help...  " + patient_Limit_Time;
                }
                if (patient_Limit_Time <= 0f)
                {
                    Debug.Log("less than 0");
                    Check_Limit_Time();
                }
                patient_delta = 0f;
            }
           
            //Check_Limit_Time();
        }
        else
        {
            patient_Limit_Timer.transform.Find("LimitTimer").gameObject.SetActive(false);
            patient_Limit_Time = 30f;
        }
        


    }

    
    public void Automatically_light_OFF()
    {
        int a = Random.Range(0, Room_Lights.Length); //get random light_number

        delta += Time.deltaTime;
        if (delta > check_Time) // check light every checktime
        {
           
            int random = Random.Range(0, 10);
            if (TurnOFF_ratio > random)
            {
                if (Room_Lights[a] == 1)
                {
                    Room_Lights[a] = 0;
                    one_light_off_immediately = true;
                }
            }
            delta = 0;
        }
    }

    public void Automatically_Patient_disappear()
    {
        int a = Random.Range(0, Room_Patients.Length); //get random light_number

        delta += Time.deltaTime;
        if (delta > check_Time) // check light every checktime
        {

            int random = Random.Range(0, 10);
            if (Disappear_ratio > random)
            {
                if (Room_Patients[a] == 1)
                {
                    Room_Patients[a] = 0;
                    one_patient_disappear_immediately = true;
                }
            }
            delta = 0;
        }
    }



    public void TimeStop()
    {
        timeScale = 0;
    }

    public void TimeStart()
    {
        timeScale = 1;
    }

   public void meetEnemy()
    {
        SceneManager.LoadScene("MeetEnemy");
    }

    public void GameOver_1_didnt_Find_patient()
    {
        SceneManager.LoadScene("PatientDie");
    }

    void Check_Patient_disappear()
    {
        int addAllpatient = 1;
        for (int i = 0; i < Room_Patients.Length; i++) //if patient gone start limit timer
        {
            addAllpatient  *= Room_Patients[i];   
        }
        if (addAllpatient == 0)
        {
            Check_All_patient_exist = false;
        }
        else
        {
            Check_All_patient_exist = true;
        }
    }

    void Check_Limit_Time()
    {
        if(patient_Limit_Time <= 0f)
        {
            GameOver_1_didnt_Find_patient();
        }
       
    }



}
