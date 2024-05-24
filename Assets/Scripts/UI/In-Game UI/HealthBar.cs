using UnityEngine;

namespace SDVA.UI
{
    [RequireComponent(typeof(Fader))]
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] RectTransform foreground;

        private void LateUpdate()
        {
            transform.rotation = Camera.main.transform.rotation;
        }

        public void SetPercentage(float percentage)
        {
            foreground.localScale = new Vector3 (percentage, 1f, 1f);
            
            if (!Mathf.Approximately(percentage, 1f))
                { transform.GetChild(0).gameObject.SetActive(true); }
            
            if (Mathf.Approximately(percentage, 0f))
                { StartCoroutine(GetComponent<Fader>().FadeOut(FadeTime: 1f)); }
        }

        public void Hide(bool enabled)
        {
            if (enabled && !Mathf.Approximately(foreground.localScale.x, 1f))
            {
                transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}