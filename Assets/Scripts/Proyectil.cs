using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public LayerMask collision;
 

    // Update is called once per frame
    void FixedUpdate()
    {
      

        if (Check())
        {
            Destroy(gameObject);
        }


    }

    private bool Check()
    {
        return Physics2D.OverlapCircle(transform.position, 0.2f, collision);
    }


}
