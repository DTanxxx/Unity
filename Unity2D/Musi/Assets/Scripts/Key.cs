using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private bool isContinuous = false;
    [SerializeField] private GameObject correctKeyPressVFX = null;
    [SerializeField] private float keyFadeOutDuration = 5.0f;
    [SerializeField] private float scorePerKey = 15.0f;
    [SerializeField] private float marginOfErrorAtBaseline = 3.0f;

    private float moveSpeed;
    private bool isPressed = false;
    private bool fadingOut = false;
    private GameObject baseLine;
    private float keyHeight;

    private void Start()
    {
        baseLine = GameObject.FindGameObjectWithTag("Base Line");
        keyHeight = GetComponent<BoxCollider2D>().size.y * transform.localScale.y;
    }

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public bool GetIsContinuous()
    {
        return isContinuous;
    }

    private void Update()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, -moveSpeed);
        if (fadingOut) { return; }
        if (isContinuous && isPressed)
        {
            if (transform.position.y + keyHeight / 2 - baseLine.transform.position.y <= -marginOfErrorAtBaseline)
            {
                // Left baseline, add points and destroy key
                FindObjectOfType<GameSession>().AddToScore(scorePerKey);
                isPressed = false;
                Destroy(gameObject);
            }
        }
    }

    public void HandlePress()
    {
        if (fadingOut) { return; }
        bool isValid = Mathf.Abs(transform.position.y - keyHeight/2 - baseLine.transform.position.y) <= marginOfErrorAtBaseline;
        if (!isContinuous && isValid)
        {
            // Spawn particle effect
            GameObject VFXInstance = Instantiate(correctKeyPressVFX, transform.position, correctKeyPressVFX.transform.rotation);
            FindObjectOfType<GameSession>().AddToScore(scorePerKey);
            // Destory key
            Destroy(gameObject);
        }
        else if (isContinuous && isValid)
        {
            // Spawn particle effect
            //GameObject VFXInstance = Instantiate(correctKeyPressVFX, transform.position, correctKeyPressVFX.transform.rotation);
            Debug.Log("Pressed a continuous key");
            isPressed = true;
        }
        else
        {
            StartCoroutine(FadeOutKeyAndDestroy());
        }
    }

    public void HandleRelease()
    {
        if (!isContinuous) { return; }
        if (fadingOut) { return; }

        // If isContinuous, then check if the key in processing has left baseline
        float yPosThreshold = baseLine.transform.position.y;
        isPressed = false;
        Debug.Log(transform.position.y + keyHeight / 2);
        if (transform.position.y + keyHeight/2 - yPosThreshold <= marginOfErrorAtBaseline)
        {
            // Good release
            Debug.Log("Good release");
            FindObjectOfType<GameSession>().AddToScore(scorePerKey);
            isPressed = false;
        }
        else
        {
            // Still over baseline, no points and destroy key.
            Debug.Log("Released too early!");
        }
        Destroy(gameObject);
    }

    private IEnumerator FadeOutKeyAndDestroy()
    {
        fadingOut = true;
        float time = 0.0f;
        float alpha = GetComponent<SpriteRenderer>().color.a;
        Color originColor = GetComponent<SpriteRenderer>().color;
        while (time < keyFadeOutDuration)
        {
            GetComponent<SpriteRenderer>().color =
                new Color(originColor.r, originColor.g, originColor.b,
                (keyFadeOutDuration - time) / keyFadeOutDuration * alpha);
            time += Time.deltaTime;
            yield return null;
        }
        // Fade out the key, then destroy;
        Destroy(gameObject);
    }
}
