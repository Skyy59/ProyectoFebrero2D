using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;

public class FinalPortal : MonoBehaviour
{
    public Transform ptr;
    private bool isontrigger;

    public Rigidbody2D playerRb;
    public Transform PortalOut;
    public Animator cAnim;
    public GameObject Transition;
    Animator tAnimator;

    public TextMeshProUGUI ty;
    public TextMeshProUGUI credits;
    public GameObject portalD;
    AudioManager audioManager;


    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        tAnimator = Transition.GetComponent<Animator>();
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
        tAnimator.SetTrigger("End");
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(Text());
        


    }

    IEnumerator MoveInPortal()
    {
        float timer = 0f;
        while (timer < 0.5f)
        {
            audioManager.PlayTPSFX(audioManager.portalIn);
            ptr.position = Vector2.MoveTowards(ptr.position, transform.position, 3 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;


        }

    }

    IEnumerator Text()
    {
        ty.enabled = true;
        yield return new WaitForSeconds(8f);
        ty.enabled = false;
        yield return new WaitForSeconds(0.5f);
        credits.enabled = true;
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

