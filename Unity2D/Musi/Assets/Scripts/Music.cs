using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Music")]
public class Music : ScriptableObject
{
    [SerializeField] private AudioClip music = null;
    [SerializeField] private float[] timeStamps;
    [SerializeField] private GameObject[] keys;

    public AudioClip GetMusic()
    {
        return music;
    }

    public float[] GetTimeStamps()
    {
        return timeStamps;
    }

    public GameObject[] GetKeys()
    {
        return keys;
    }
}
