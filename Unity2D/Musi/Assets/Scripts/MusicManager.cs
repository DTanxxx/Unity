using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private Music musicToPlay = null;

    private KeySpawner keySpawner;
    private AudioSource musicPlayer;

    private void Start()
    {
        keySpawner = FindObjectOfType<KeySpawner>();
        musicPlayer = GetComponent<AudioSource>();
        StartCoroutine(PlayMusic());
    }

    private IEnumerator PlayMusic()
    {
        musicPlayer.PlayOneShot(musicToPlay.GetMusic());
        float[] timeStamps = musicToPlay.GetTimeStamps();
        GameObject[] keys = musicToPlay.GetKeys();
        for (int i = 0; i < timeStamps.Length; ++i)
        {
            if (i == 0) { yield return new WaitForSeconds(timeStamps[i]); }
            else { yield return new WaitForSeconds(timeStamps[i] - timeStamps[i - 1]); }
            // Time stamp is up, spawn key.
            keySpawner.SpawnKey(keys[i]);
        }
    }

    public void SetMusicToPlay(Music music)
    {
        musicToPlay = music;
    }

    public Music GetMusicToPlay()
    {
        return musicToPlay;
    }
}
