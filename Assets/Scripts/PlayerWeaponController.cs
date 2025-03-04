using DG.Tweening;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private Transform ptr;
    [SerializeField] private Transform proyectilprefab;
    [SerializeField] private Transform pivot;
    [SerializeField] private float projectilspeed;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ShootProjectile();
        }   
        else if (Input.GetKeyDown(KeyCode.W) && Input.GetKeyDown(KeyCode.F))
        {
            ShootProjectileUp();
        }
        
    }


    void ShootProjectile()
    {
        Transform proyectil = Instantiate(proyectilprefab);
        proyectil.position = pivot.position;
        Rigidbody2D rigidbody = proyectil.GetComponent<Rigidbody2D>();

        if (ptr.localScale.x > 0)
        {
            rigidbody.AddForce(transform.right * projectilspeed, ForceMode2D.Impulse);
        }
        else if (ptr.localScale.x < 0)
        {
            rigidbody.AddForce((transform.right * -1) * projectilspeed, ForceMode2D.Impulse);
        }

        
    }

    void ShootProjectileUp()
    {
        Transform proyectil = Instantiate(proyectilprefab);
        proyectil.position = pivot.position;
        Rigidbody2D rigidbody = proyectil.GetComponent<Rigidbody2D>();

        rigidbody.AddForce(transform.up * projectilspeed, ForceMode2D.Impulse);
    }
}
