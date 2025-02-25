using UnityEngine;

public class Lasers : MonoBehaviour
{
    public SpriteRenderer[] SRender;
    private float countdown = 10f;
    public float timer = 10f;
    public bool enable;
    public BoxCollider2D BCol;

    void Start()
    {

        for (global::System.Int32 i = 0; i < SRender.Length; i++)
        {
            SRender[i].enabled = enable;
        }

        BCol.enabled = enable;

        countdown = enable ? 0 : timer;

    }

    
    void Update()
    {
        
        if (enable == false)
        {
            
            countdown -= Time.deltaTime;
            if (countdown < 0)
            {
                for (global::System.Int32 i = 0; i < SRender.Length; i++)
                {
                    SRender[i].enabled = false;
                }

                BCol.enabled = false;
                countdown = 0;
                enable = true;
            }
        }
        else
        {
            countdown += Time.deltaTime;
            if (countdown >= timer)
            {
                
                for (global::System.Int32 i = 0; i < SRender.Length; i++)
                {
                    SRender[i].enabled = true;
                }

                BCol.enabled = true;
                countdown = timer;
                enable = false;
            }
        }
        
    }
}
