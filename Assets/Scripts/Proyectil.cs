using UnityEngine;

public class Proyectil : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.2f, LayerMask.GetMask("Environment"));

        if (col)
        {
            Destroy(gameObject);
        }
    }
}
