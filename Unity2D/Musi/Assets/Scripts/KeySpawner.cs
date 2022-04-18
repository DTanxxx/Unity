using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawner : MonoBehaviour
{
    //[SerializeField] private GameObject keyPrefab = null;
    [SerializeField] private float keySpeed = 1.0f;
    [SerializeField] private Transform[] spawnPositions;

    private GameSession gameSession;
    private int prevIndex;

    private void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
    }

    public void SpawnKey(GameObject key)
    {
        Debug.Log("Spawn key!");
        int index = prevIndex;
        while (index == prevIndex)
        {
            index = Random.Range(0, spawnPositions.Length);
        }
        prevIndex = index;
        var keyInstance = Instantiate(key, spawnPositions[index].position, Quaternion.identity);
        gameSession.AddToKeyStream(keyInstance, index);
        keyInstance.GetComponent<Key>().SetSpeed(keySpeed);
    }
}
