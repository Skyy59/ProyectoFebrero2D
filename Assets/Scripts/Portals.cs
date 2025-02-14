using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Portals : MonoBehaviour
{
    public Transform ptr; 

    public bool isontrigger;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isontrigger = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isontrigger == true && Input.GetKeyDown(KeyCode.E))
        {
            ptr.position = new Vector2(5.13f, 10.32f);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
      
        isontrigger = true;
        
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
       
        isontrigger = false;
        
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
       
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        collision.transform.position = new Vector2(5.13f, 10.32f);
    //        Debug.Log("Press input");
    //    }
    //}
}
