using UnityEngine;

public class PlatformStatic : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform Ptr;

    [Header("Platform")]
    [SerializeField] private Transform Platformtr;
    [SerializeField] private BoxCollider2D PlCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collision();
    }

    void Collision()
    {
        //si la posicion vertical del jugador es mayor a la de la plataforma, esta gana colision
        if (Ptr.transform.position.y > Platformtr.transform.position.y)
        {
            //se activa el boxcollider2D de la plataforma para darle una hitbox
            Platformtr.GetComponent<BoxCollider2D>().enabled = true;

        }
        else
        {
            //si el personaje esta por debajo de la plataforma, esta pierde colision para poder atravesarla
            Platformtr.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Cuando entra en collision con el Jugador comparando si tiene la tag de "Player", convierte al Jugador en hijo de la plataforma para que se mueva junto a ella
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = transform;

        }

    }

    void OnCollisionExit2D(Collision2D collision)
    {
        //quita el padre-hijo de la plataforma-jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }

    }
}
