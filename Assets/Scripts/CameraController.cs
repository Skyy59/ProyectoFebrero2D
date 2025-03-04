using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public Transform player;
 

    private float horz;
    private float vert;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        horz = Input.GetAxis("CamaraHorizontal");
        vert = Input.GetAxis("CamaraVertical");

        Vector3 cam = new(player.transform.position.x + horz, (player.transform.position.y + 1) + vert, transform.position.z);
        transform.position = Vector3.Lerp(player.transform.position, cam, 3f);

        

       

    }

   

    

}



