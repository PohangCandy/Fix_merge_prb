using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.Experimental.GraphView;

public class PlayerController : MonoBehaviour
{


    public static bool scene_move = false;
    public static bool scene_update = false;
    Scene scene;
    Rigidbody2D Player_rb;
    BoxCollider2D Player_col;
    Slider slider;
    GameObject door;
    public Canvas stamina_skill_ui;
    public Canvas phone;
    Animator animator;
    public float power;
    public float nomal_speed;
    static Vector3 localscale;
    Vector3 spawn_point;
    SpriteRenderer sprite;

    int s_namesize;
    string []not_visible_S_name;
    public float stamina_useable_time_max;
    public float stamina_de_time_max;

    Transform cabinet_trs;

    float direction;
    float speed;
    float stamina_useable_time;
    float stamina_de_time;
    float stemina_speed = 1.5f;
    float restroom_speed = 2.0f;

    
    Text skill;
    float buff_time = 5f;
    float skill_time = 30f;
    float see_cool;

    bool isspawn = false;
    bool isinside = false;
    bool isbuff=false;
    bool isrest = true;
    bool iscan_hide=false;
    bool ishide=false;
    bool usestamina = false;
    bool ispenalti = false;
    int div;

    Transform left = null; //왼쪽 포탈의 위치
    Transform right = null;//오른쪽 포탈의 위치
    // Start is called before the first frame update
    private void Awake()
    {
        localscale = transform.localScale;
        spawn_point = transform.position;
    }
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator=GetComponent<Animator>();
        see_cool = skill_time;
        skill=GameObject.Find("Skill").GetComponent<Text>();
        slider = GameObject.Find("Stamina").GetComponent<Slider>();//���׹̳���� ��ü�� ã�Ƽ� �׾��� slider�� �����´�
        stamina_de_time = stamina_de_time_max;
        speed = nomal_speed;
        stamina_useable_time = stamina_useable_time_max;
        //speed = 0.3f;
        Player_rb= GetComponent<Rigidbody2D>();
        Player_col= GetComponent<BoxCollider2D>();
        P_set_Not_visble_name_set();
    }
    
    IEnumerator Find_left_right(Transform left, Transform right)
    {

        yield return new WaitForSeconds(0.1f);
        if (scene_move == false) yield return null;
        else if (GameObject.FindWithTag("Left") == null)
        {
            StartCoroutine(Find_left_right(left, right));
        }
        else if (GameObject.FindWithTag("Right") == null)
        {
            StartCoroutine(Find_left_right(left, right));
        }
        else
        {
            
            scene_move = false;
            scene_update = true;
            left = GameObject.FindWithTag("Left").GetComponent<Transform>();
            right = GameObject.FindWithTag("Right").GetComponent<Transform>();
            set_l_R(left, right);
            StopAllCoroutines();
            //Debug.Log(left.position);
            //Debug.Log(right.position);

        }
 /*       if (left != null && right != null)
            yield return null;
        else 
        {
            StartCoroutine(Find_left_right(left,right));
        }*/
    }
    void set_l_R(Transform _left, Transform _right) { left = _left;right = _right; }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(scene_move);
        if(scene_move)
        {
             
            StartCoroutine(Find_left_right( left, right));

        }
        if(scene_update)
        {
            //DataManager.instance.LoadData();
            DataManager.instance.LoadData();
            div = DataManager.instance.playerData.movedirection;
            Debug.Log(div);
            if (div == -1) transform.position = right.position;// +new Vector3(-10,0,0);
            if (div == 1) transform.position = left.position;// +new Vector3(10,0,0);
            scene_update=false;
        }
        //Debug.Log(spawn_point);
        StartCoroutine(P_isnotvisible_scene());
        player_slider_update();//�����̴��� ������Ʈ ����
        
        if (!ishide)//���� �ʴ°�쿡 �����̱� ����
        {
            player_move();
            if(isbuff==false)
                player_use_stamina();
        }
        door_in();//����ü��Ƿ� ���� �Լ�
  
        if (iscan_hide)//ĳ��ݰ� ������������
            player_hide();
        
        if(isrest)
        {
            skill_readyUI();
            useRest();
        }
        if(isrest==false&&isbuff==false)
        {
            skill_coolUI();
        }
    }
    //create game set names not visible scene
    static public void set_move_scene() { scene_move = true; }
    void P_set_Not_visble_name_set()
    {
        s_namesize = 10;
        not_visible_S_name = new string[s_namesize];//c#에서는 garbage콜렉터가 해당하는 공간을 삭제해줌
        not_visible_S_name[0] = "Loading";
        not_visible_S_name[1] = "inside";
        not_visible_S_name[2] = "Title";
    }
    IEnumerator Velocity_zero()
    {
        Player_rb.velocity= Vector3.zero;
        yield return new WaitForSeconds(0.5f);
    }
    void P_spawn() { transform.position = spawn_point; Player_rb.gravityScale = 1; }
    //is not visible scene? check function
    IEnumerator P_isnotvisible_scene()
    {
        yield return new WaitForSeconds(0.8f);
        scene = SceneManager.GetActiveScene();
        int count = 0;
        for (int i = 0; i < s_namesize; i++)
        {
            if (not_visible_S_name[i] == null)
                break;
            if (not_visible_S_name[i] == scene.name)
            {
                P_sprite_N_visible();
                canvas_invisible();
                count++;
                
                break;
            }
        }
        if (count == 0)
        {
            P_sprite_Y_visible();
            canvas_visible();
            //P_spawn();
            if (isspawn)
            {
                StartCoroutine(Velocity_zero());
                P_spawn();
                isspawn = false;
            }
        }
        
        
    }
    void canvas_invisible()
    {
        stamina_skill_ui.enabled = false;
        phone.enabled = false;
    }

    void canvas_visible()
    {
        stamina_skill_ui.enabled = true;
        phone.enabled = true;
    }
    void P_defaultSetting()
    {
        stamina_de_time = stamina_de_time_max;
        speed = nomal_speed;
        stamina_useable_time = stamina_useable_time_max;
    }
    void P_sprite_N_visible() { sprite.enabled = false; }
    void P_sprite_Y_visible() { sprite.enabled = true; }
    IEnumerator goinside()
    {
        isinside = true;
        SceneManager.LoadScene("inside");
        yield return new WaitForSeconds(0.1f);
        //P_isnotvisible_scene();
        Player_rb.gravityScale = 0;
        P_sprite_N_visible();
        
        stamina_useable_time = 0;
        yield return new WaitForSeconds(2f);
        while (true)
        {
            if (GameObject.Find("DialogTest").GetComponent<DialogTest>().IsDialogEnd()==true)
            {
                if (Input.anyKeyDown||Input.anyKey)
                {
                    SceneManager.LoadScene("Title");
                    yield return new WaitForSeconds(0.1f);
                    isspawn = true;
                    //P_isnotvisible_scene();
                    /*P_sprite_N_visible();
                    P_spawn();*/
                    break;
                }
            }
            yield return new WaitForSeconds(2f);
        }
        
    }
    void door_in()
    {
        if (Input.GetKey(KeyCode.E))
        {
            door = GameObject.Find("door");
            if (door == null) ;
            else
            {
                bool ison = door.GetComponent<E_On_GUI>().Getison();
                if (door != null && ison == true)
                {
                    StartCoroutine(goinside());
                }
            }
            //door = GameObject.Find("RandomDoor_ctr").GetComponent<Door_spawner>().Getobject();
        }
    }
    void useRest()
    {
        if(Input.GetKey(KeyCode.R)&&!isinside)
        {
            StartCoroutine(gorest());
        }
    }
    IEnumerator gorest()
    {
        SceneManager.LoadScene("Restroom");
        isrest = false;
        Player_rb.gravityScale = 0;
        isbuff = true;
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("NewPath");
        Player_rb.gravityScale = 1;
        stamina_useable_time = stamina_useable_time_max;
        speed =nomal_speed* restroom_speed ;
        usestamina = false;
        yield return new WaitForSeconds(buff_time);
        isbuff = false;
        speed = nomal_speed/ restroom_speed ;
        //UnloadSceneOptions.
        yield return new WaitForSeconds(skill_time);
        isrest=true;
    }
    void skill_coolUI()
    {

        see_cool -= Time.deltaTime;
        skill.text = "Skill\n" + (int)see_cool;
    }
    void skill_readyUI()
    {
        see_cool = skill_time;
        skill.text = "Skill\n" +"Ready";
    }
    void player_slider_update()
    {
        slider.value = stamina_useable_time / stamina_useable_time_max;
    }
    void player_hide()
    {
       
        if (!ishide)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                nowalk();
                noRun();
                Player_rb.velocity = Vector2.zero;
                this.transform.position=cabinet_trs.position;
                gameObject.tag = "Untouchable";
                ishide = true;
                // Player_col.enabled = false;
            }
        }
        else
        {
            if (stamina_useable_time < stamina_useable_time_max)
            {
                stamina_useable_time += Time.deltaTime;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                gameObject.tag = "Player";
                ishide = false;
                //���׹̳� �ۿ� 
                {
                    speed = nomal_speed;
                    ispenalti = false;
                    stamina_de_time = stamina_de_time_max;
                    usestamina = false;
                }

            }
        }
    }
    void setRun()
    {
        animator.SetBool("IsRun", true);
    }
    void noRun()
    {
        animator.SetBool("IsRun", false);
    }

    void setwalk()
    {
        animator.SetBool("IsWalk", true);
    }
    void nowalk()
    {
        animator.SetBool("IsWalk",false);
    }
    void player_move()
    {
        if (Input.GetKey(KeyCode.A))
        {
            //방향뒤집기용
            transform.localScale = new Vector3(localscale.x, localscale.y, localscale.z);
            setwalk();
            direction = -1;

            // Player_rb.AddForce(new Vector2(direction * power * speed, 0f));
            transform.Translate(new Vector2(direction * power * speed*Time.deltaTime,0f));
            //  Debug.Log("left");
        }

        else if (Input.GetKey(KeyCode.D))
        {
            setwalk();
            //방향뒤집기용
            transform.localScale=new Vector3(localscale.x*-1,localscale.y,localscale.z);
            direction = 1;

            //Player_rb.AddForce(new Vector2(direction * power * speed, 0f));
            transform.Translate(new Vector2(direction * power * speed * Time.deltaTime, 0f));
            //   Debug.Log("right");
        }
        else
        {
            nowalk();
        }
        /*    if (Input.GetKeyUp(KeyCode.A)) ������ �ٵ�� �ҽÿ� velocity�� ���־� ���߱⿡ �̸� ����
            {

                //Player_rb.velocity = Vector2.zero;
                transform.Translate(new Vector2(direction * power * speed, 0f));
                //   Debug.Log("leftstop");
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                //Player_rb.velocity = Vector2.zero;
                transform.Translate(new Vector2(direction * power * speed, 0f));
                // Debug.Log("rightstop");
            }*/
    }
    void player_use_stamina()
    {
        //Debug.Log(Player_rb.velocity);
       
        if (stamina_useable_time > 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                speed = nomal_speed* stemina_speed ;
                usestamina = true;
                //  Debug.Log(speed);
                setRun();
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                speed =nomal_speed/stemina_speed ;
                usestamina = false;

                noRun();
                // Debug.Log(speed);
            }
        }

        if (usestamina)
        {
            stamina_useable_time -= Time.deltaTime;
            
            if(stamina_useable_time<0)
            {
                Debug.Log("isp");

                noRun();
                speed = nomal_speed / 2;
                ispenalti = true;
                usestamina = false;
            }
        }
        else if(!ispenalti)//���׹̳��� ������� �ʰ� ���Ƽ ���°� �ƴҰ��
        {
            if (stamina_useable_time < stamina_useable_time_max)
            {
                stamina_useable_time += Time.deltaTime;
            }
        }
        
        if(ispenalti)
        {
          //  Debug.Log(math.abs(Player_rb.velocity.x));
            if (math.abs(Player_rb.velocity.x) >= 2f)
            {
                Player_rb.AddForce(-Player_rb.velocity.normalized * nomal_speed); 
            }
            stamina_de_time -= Time.deltaTime;
            if(stamina_de_time<0)
            {
                speed = nomal_speed/2;
                //Debug.Log("isnotp");
                ispenalti = false;
                stamina_de_time = stamina_de_time_max;
            }
        }
        

    }
    void OnTriggerStay2D(Collider2D other)//��� �浹�ϰ� �����ÿ� �Ͼ
    {
        if (other.CompareTag("Cabinet")) // �浹�� ��ü�� �±װ� "Player"�� ���
        {
            cabinet_trs = other.transform;
            iscan_hide = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Cabinet")) // �浹�� ��ü�� �±װ� "Player"�� ���
        {
            iscan_hide = false;
        }
    }



}
