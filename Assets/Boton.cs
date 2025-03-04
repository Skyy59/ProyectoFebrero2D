
using UnityEngine;

public class Boton : MonoBehaviour
{
    private bool enable;
    private bool pressed;
    public Animator bAnimator;
    public GameObject door;
    Animator dAnimator;
    BoxCollider2D dCollider;

    private void Awake()
    {
        dAnimator = door.GetComponent<Animator>();
        dCollider = door.GetComponent<BoxCollider2D>();
    }


    void Update()
    {
        if (enable)
        {
            bAnimator.SetTrigger("pressed");
            pressed = true;

        }

        if (pressed)
        {
            dAnimator.SetBool("sus", true);
            dCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Proyectil"))
        {
            enable = true;
            
        }
    }
}
