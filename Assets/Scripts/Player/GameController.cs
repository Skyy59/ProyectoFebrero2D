using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class GameController : MonoBehaviour
{

    Vector2 Checkpointpos;
    public SpriteRenderer sprRender;
    public Rigidbody2D Rb;


    void Start()
    {
        Checkpointpos = transform.position;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    #region METHODS
    void Die()
    {
        StartCoroutine(Respawn(0.5f));
    }

    public void UpdateCheckpoint(Vector2 pos)
    {
        Checkpointpos = pos;
    }
    IEnumerator Respawn(float duration)
    {
        Rb.simulated = false;
        Rb.linearVelocity = new Vector2(0, 0);
        sprRender.enabled = false;
        yield return new WaitForSeconds(duration);
        transform.position = Checkpointpos;
        sprRender.enabled = true;
        Rb.simulated = true;
    }
    #endregion
}
