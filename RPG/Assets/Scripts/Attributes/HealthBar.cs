using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas canvas = null;

        void Update()
        {
            if (healthComponent.GetFraction() == 1f || healthComponent.GetFraction() <= 0f)
            {
                canvas.enabled = false;
            }
            else
            {
                canvas.enabled = true;
            }

            foreground.localScale = new Vector3(healthComponent.GetFraction(), 1f, 1f);        
        }
    }
}
