using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Portals : MonoBehaviour
{
    public Transform ptr; 
    private bool isontrigger;
    public Animation anim;
    public Rigidbody2D playerRb;
    public Transform PortalOut;
  
    
   

    void Start()
    {
        isontrigger = false;
        
    }

 
    void Update()
    {
        if (isontrigger == true && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(PortalIn());
        }
    }

    #region PORTAL IN & OUT
    IEnumerator PortalIn()
    {
        playerRb.simulated = false;
        anim.Play("PortalIn");
        StartCoroutine(MoveInPortal());
        yield return new WaitForSeconds(0.5f);
        ptr.position = PortalOut.position;
        playerRb.linearVelocity = Vector2.zero;
        anim.Play("PortalOut");
        yield return new WaitForSeconds(0.5f);
        playerRb.simulated = true;
    }

    IEnumerator MoveInPortal()
    {
        float timer = 0f;
        while (timer < 0.5f)
        {
            
            ptr.position = Vector2.MoveTowards(ptr.position, transform.position, 3 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;


        }

    }
    #endregion

    #region CHECKS
    private void OnTriggerEnter2D(Collider2D collision)
    {
      
        isontrigger = true;
        
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
       
        isontrigger = false;
        
    }
    #endregion


}
