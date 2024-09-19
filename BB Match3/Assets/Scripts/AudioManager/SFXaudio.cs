using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXaudio : MonoBehaviour
{
    [Header("----------- Audio Source --------------")]



    [SerializeField] AudioSource SFXSource;


    [Header("------- Audio Clip --------")]

    public AudioClip match;
    public AudioClip match01;
    public AudioClip match02;
    public AudioClip match03;

    public AudioClip select;
    public AudioClip deny;
    public AudioClip gravity;
    public AudioClip gravity01;
    public AudioClip gravity02;
    public AudioClip gravity03;
    public AudioClip p1;
 
    public AudioClip p102;
    public AudioClip p103;
    public AudioClip p104;
    public AudioClip p2;
    public AudioClip p3;
    public AudioClip p4;
    public AudioClip p5;


    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
        DontDestroyOnLoad(clip);
    }
}
