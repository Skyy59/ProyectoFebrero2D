using UnityEngine;
using DG.Tweening;

public class Camera : MonoBehaviour
{
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);


        ////Using DoTween for smoother transition into cameras
        //if (Input.GetKey(KeyCode.S))
        //{
        //    transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 1f, transform.position.z);
        //}
    }
}



