using UnityEngine;
using System.Collections;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
        }

        public IEnumerator FadeOut(float time)
        {
            while (canvasGroup.alpha < 1)  // while alpha is not 1
            {
                // moving alpha towards 1
                canvasGroup.alpha += Time.deltaTime / time;

                // wait for a single frame
                yield return null;
            }
        }

        public IEnumerator FadeIn(float time)
        {
            while (canvasGroup.alpha > 0)  // while alpha is not 0
            {
                // moving alpha towards 0
                canvasGroup.alpha -= Time.deltaTime / time;

                // wait for a single frame
                yield return null;
            }
        }
    }
}
