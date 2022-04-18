using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    [SerializeField] private Text scoreText = null;
    [SerializeField] private int keyColumns = 4;
    [SerializeField] private List<GameObject>[] keyStreams;

    private GameObject[] columnsOfKeysInProcessing;
    private bool isContinuousKey = false;
    private float score;

    private void Start()
    {
        score = 0.0f;
        scoreText.text = "Score: " + score.ToString();
        keyStreams = new List<GameObject>[keyColumns];
        columnsOfKeysInProcessing = new GameObject[keyColumns];
        for (int i=0; i<keyStreams.Length; ++i)
        {
            keyStreams[i] = new List<GameObject>();
        }
    }

    public void AddToKeyStream(GameObject key, int index)
    {
        keyStreams[index].Add(key);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ProcessKeyStream(0, true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ProcessKeyStream(1, true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ProcessKeyStream(2, true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ProcessKeyStream(3, true);
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            ProcessKeyStream(0, false);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            ProcessKeyStream(1, false);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            ProcessKeyStream(2, false);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            ProcessKeyStream(3, false);
        }
    }

    private void ProcessKeyStream(int stream, bool pressedDown)
    {
        List<GameObject> str = keyStreams[stream];

        if (str.Count == 0) { return; }

        if (!pressedDown && (str.ToArray()[0] == null || str.ToArray()[0].Equals(null)))
        {
            return;
        }

        if (!pressedDown && (columnsOfKeysInProcessing[stream] == null || columnsOfKeysInProcessing[stream].Equals(null)))
        {
            return;
        }

        // sanity check
        while (str.ToArray()[0] == null || str.ToArray()[0].Equals(null))
        {
            // remove null elements from stream
            str.RemoveAt(0);
            if (str.Count == 0)
            {
                return;
            }
        }

        columnsOfKeysInProcessing[stream] = str.ToArray()[0];
        GameObject key = columnsOfKeysInProcessing[stream];
        if (pressedDown)
        {
            // Key pressed
            // Check if key is within a margin of error y position relative to baseline
            // If not, no points and destroy key
            // Otherwise...
            // If key is continuous, don't destroy key and set isContinuous to true and assign key in processing
            // Otherwise destroy key and gain points

            //Debug.Log(keyInProcessing.transform.position.y - baseLine.transform.position.y);
            
            key.GetComponent<Key>().HandlePress();
            if (!key.GetComponent<Key>().GetIsContinuous())
            {
                // If not continuous, set to null after pressing.
                columnsOfKeysInProcessing[stream] = null;
            }
        }
        else
        {
            // Key released
            if (key != null)
            {
                key.GetComponent<Key>().HandleRelease();
            }
            columnsOfKeysInProcessing[stream] = null;
            str.RemoveAt(0);
        }
    }

    public void AddToScore(float score)
    {
        this.score += score;
        scoreText.text = "Score: " + this.score.ToString();
    }
}
