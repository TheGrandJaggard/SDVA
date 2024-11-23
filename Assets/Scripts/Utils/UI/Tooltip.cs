using System.Collections;
using UnityEngine;

namespace SDVA.Utils.UI
{
    /// <summary>
    /// Abstract base class of tooltip which handles spawning in the correct
    /// position on the screen relative to the mouse cursor.
    /// 
    /// Override the abstract functions to create a tooltip for your own data.
    /// </summary>
    public abstract class Tooltip : MonoBehaviour
    {
        // LIFECYCLE METHODS

        private void Awake()
        {
            GetComponentInChildren<CanvasGroup>().alpha = 0f;
            StartCoroutine(FollowMouse());
        }

        // PRIVATE

        private IEnumerator FollowMouse()
        {
            yield return new WaitForSecondsRealtime(0.8f);
            PositionTooltip();
            GetComponentInChildren<CanvasGroup>().alpha = 1f;
            yield return new WaitForEndOfFrame();
            while (true)
            {
                PositionTooltip();
                yield return new WaitForEndOfFrame();
            }
        }

        private void PositionTooltip()
        {
            var corners = new Vector3[4];
            GetComponent<RectTransform>().GetWorldCorners(corners);
            var tooltipSize = new Vector2(Mathf.Abs(corners[1].x - corners[2].x), Mathf.Abs(corners[0].y - corners[1].y));
            var mPos = Input.mousePosition;

            var left = mPos.x + tooltipSize.x > Screen.width;
            var above = mPos.y - tooltipSize.y < 0;

            GetComponent<RectTransform>().pivot = GetPivots(above, left);
            transform.position = mPos;
        }
        
        private Vector2 GetPivots(bool above, bool left)
        {
            if (above && !left) { return new Vector2(0, 0); }       // Bottom Left
            else if (!above && !left) { return new Vector2(0, 1); } // Top Left
            else if (!above && left) { return new Vector2(1, 1); }  // Top Right
            else { return new Vector2(1, 0); }                      // Bottom Right
        }
    }
}
