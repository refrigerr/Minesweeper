using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoudEffects : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip defeat, victory;
    
    public void playDefeat()
    {
        audioSource.clip = defeat;
        audioSource.Play();
    }
    public void playVictory()
    {
        audioSource.clip = victory;
        audioSource.Play();
    }
}
