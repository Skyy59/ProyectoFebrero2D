using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------Audio Source------")]
    [SerializeField] AudioSource mSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource TPSource;


    [Header("------Audio Clip------")]
    public AudioClip background;
    public AudioClip death;
    public AudioClip portalIn;
    public AudioClip jump;
    public AudioClip Dash;

    private void Start()
    {
        mSource.clip = background;
        mSource.loop = true;
        mSource.Play();
        
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayTPSFX(AudioClip clip)
    {
        TPSource.PlayOneShot(clip);
    }
}
