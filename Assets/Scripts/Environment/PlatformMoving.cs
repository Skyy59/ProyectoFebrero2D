using DG.Tweening;
using UnityEngine;

public class Platforms : MonoBehaviour
{
    public Transform waypoint1;
    public Transform waypoint2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlatformMove();
    }

    void PlatformMove()
    {
        transform.DOMove(waypoint2.position, 5f).SetDelay(1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            transform.DOMove(waypoint1.position, 5f).SetDelay(1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                PlatformMove();
            });
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }
}
