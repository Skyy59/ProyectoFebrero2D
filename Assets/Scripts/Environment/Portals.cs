using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor.Animations;

public class Portals : MonoBehaviour
{
    public Transform ptr;
    private bool isontrigger;
    
    public Rigidbody2D playerRb;
    public Transform PortalOut;
    public Animator cAnim;

    public GameObject portalD;
    private void Awake()
    {
        
    }

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
        cAnim.SetTrigger("isInPortal");
        cAnim.SetBool("portal", true);
        cAnim.SetBool("Fall", false);
        cAnim.SetBool("Jump", false);
        StartCoroutine(MoveInPortal());
        yield return new WaitForSeconds(1.5f);
        ptr.position = PortalOut.position;
        playerRb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1f);
        cAnim.SetBool("portal", false);
        playerRb.simulated = true;
        yield return new WaitForSeconds(1f);
        Destroy(portalD.gameObject);
        
        
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
