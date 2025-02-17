using UnityEngine;

public class Checpoint : MonoBehaviour
{
    GameController gamecontroller;
    void Start()
    {
        gamecontroller = GameObject.FindGameObjectWithTag("Player").GetComponent<GameController>();
    }

  
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
      if (collision.CompareTag("Player"))
        {
            gamecontroller.UpdateCheckpoint(transform.position);
        }  
    }
}
