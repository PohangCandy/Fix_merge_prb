using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedController : MonoBehaviour
{
    // Ư�� �ð��� �Ǹ� �����ϰ� �������,
    // Ŀư�� e�� ������ ����� ȯ�ڰ� �ִ��� ������ �� �� �ִ�.
    // ȯ�ڰ� �������� ������ ��ҿ� ȯ�ڰ� �������ְ� ��ȣ�ۿ��ؼ� ������ ���ư� �� �ִ�.
    // ���� ����� ��� �����ؾ��Ѵ�.
    bool Can_Check;

    public GameObject director;
    public Sprite full;
    public Sprite empty;
    public Sprite cutton;

    
    public int patient_exist;

    private void Start()
    {
        Can_Check = false;
        patient_exist = 1;
       
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(Can_Check == true)
            {
                Open_Cutton();
            }
        }
    }

    void Open_Cutton()
    {
        if (patient_exist == 1)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = this.full;
            StartCoroutine(Close_Cutton());
           
        }

        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = this.empty;
            StartCoroutine(Close_Cutton());
           
        }
    }


    //change sprite according to patient exist
    public void PatientGone()
    {
        patient_exist = 0;
    }

    public void PatientComeback()
    {
        patient_exist = 1;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))      
        {
            Can_Check = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Can_Check = false;
        }
    }

    IEnumerator Close_Cutton()
    {
        yield return new WaitForSeconds(3.0f);
        gameObject.GetComponent<SpriteRenderer>().sprite = this.cutton;
    }
    

}
