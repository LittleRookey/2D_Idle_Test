using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;
public class AudioPlay : MonoBehaviour
{
    [SerializeField] private string audioName;
    public bool playOnAwake;

    private void Awake()
    {
        if (playOnAwake)
            Play();
    }
    public void Play()
    {
        MasterAudio.PlaySound(audioName);
    }
}
