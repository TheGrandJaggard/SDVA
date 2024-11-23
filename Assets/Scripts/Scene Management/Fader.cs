using System.Collections;
using UnityEngine;

namespace SDVA.UI
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        private bool fadeOutOverride = false;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public IEnumerator FadeIn(float FadeTime)
        {
            fadeOutOverride = true;
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime / FadeTime;
                yield return null;
            }
            fadeOutOverride = false;
        }

        public IEnumerator FadeOut(float FadeTime)
        {
            while (canvasGroup.alpha > 0)
            {
                if(fadeOutOverride) { yield return null; }
                canvasGroup.alpha -= Time.deltaTime / FadeTime;
                yield return null;
            }
        }

        public void SetAlpha(float alpha)
        {
            if (canvasGroup == null) { Awake(); }
            canvasGroup.alpha = alpha;
        }
    }
}
