using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyGenerator : MonoBehaviour
{
   
    GameObject enemy;
    public GameObject slow_enemyPrefab;
    public GameObject fast_enemyPrefab;
    public bool enemy_exist; 
    [SerializeField] bool All_RoomLightOff;
    [SerializeField] bool AllLightOff;
    [SerializeField] float check;
    public int patten;
    Scene scene;

    private void Awake()
    {
        var obj = FindObjectsOfType<EnemyGenerator>();
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
        AllLightOff = false;
        All_RoomLightOff = false;
        enemy_exist = false;
    }


    void Update()
    {
        Check_Enemy_Exist();

        scene = SceneManager.GetActiveScene();
        check = 0f;
        
        if (scene.name == "PatientRoom101" || scene.name == "PatientRoom102")
        {

            Check_Room_Light();

        }

        Check_ALL_Light();

    }

    void Check_Room_Light()
    {
            GameObject[] off = GameObject.FindGameObjectsWithTag("Light");

            for (int i = 0; i < off.Length; i++)
            {
                check += off[i].GetComponent<LightController>().lightOff;
            }

            if (check == 0)
            {
                All_RoomLightOff = true;
            }
            if (check >= 1)
            {
                All_RoomLightOff = false;
            }
       

            if (this.enemy_exist == false && this.All_RoomLightOff == true && AllLightOff == false)
            {
                Appear_SlowEnemy();
            }

            if (All_RoomLightOff == false) //when the player turn on any light
            {
                Destroy(enemy);
                //enemy_exist = false;
            }
    }

    void Check_ALL_Light()
    {
        int[] RoomLights = GameObject.Find("GameDirector").GetComponent<GameDirector>().Room_Lights;

        for (int i = 0; i < RoomLights.Length; i++)
        {
            check += RoomLights[i];
        }

        if (check == 0)
        {
            AllLightOff = true;
        }
        if (check >= 1)
        {
            AllLightOff = false;
        }


        if (this.enemy_exist == false && this.AllLightOff == true)
        {
            Appear_FastEnemy();
        }
    }

    void Appear_SlowEnemy()
    {
        this.enemy = Instantiate(slow_enemyPrefab);
        //this.enemy_exist = true;
        this.patten = 1;
    }

    void Appear_FastEnemy()
    {
        this.enemy = Instantiate(fast_enemyPrefab);
        //this.enemy_exist = true;
        this.patten = 2;
    }

    void Check_Enemy_Exist()
    {
        if(GameObject.FindGameObjectWithTag("Enemy"))
        {
            enemy_exist = true;
        }
        else
        {
            enemy_exist = false;
        }
    }
}
