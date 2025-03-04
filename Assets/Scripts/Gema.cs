using Unity.VisualScripting;
using UnityEngine;

public class Gema : MonoBehaviour
{
    public SpriteRenderer portal1SR;
    public BoxCollider2D portal1BC;
    public SpriteRenderer portal2SR;
    public GameObject gema;

    private bool enable;

    public float strength = 0.5f;
    public float speed = 1f;
    private float newposy;
    private float originalposy;

    void Start()
    {
        originalposy = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (enable == true)
        {
            portal1SR.enabled = true;
            portal2SR.enabled = true;
            portal1BC.enabled = true;
            Destroy(gema);
        }

        Floating();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enable = true;
        }
    }

    private void Floating()
    {
        newposy = Mathf.Sin(Time.time * speed) * strength + originalposy;

        transform.position = new Vector3(transform.position.x, newposy, transform.position.z);
    }
}
