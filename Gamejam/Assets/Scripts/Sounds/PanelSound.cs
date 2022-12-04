using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PanelSound : MonoBehaviour
{
    AudioSource audioData;
    AudioSource BGaudio;
    void Start()
    {
        GameObject bgGameObject = GameObject.FindWithTag("BGmusic");
        BGaudio = bgGameObject.GetComponent<AudioSource>();
        BGaudio.Stop();
        audioData = GetComponent<AudioSource>();
        audioData.Play(0);
    }
}
