using System.Collections;
using UnityEngine;

namespace SDVA.Utils.UI
{
    /// <summary>
    /// Abstract base class of tooltip.
    /// 
    /// Override the abstract functions to create a tooltip for your own data.
    /// </summary>
    public abstract class Tooltip : MonoBehaviour
    {
        // LIFECYCLE

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
            Debug.Log($"var {left} = {mPos.x} + {tooltipSize.x} > {Screen.width};");
            var above = mPos.y - tooltipSize.y < 0;
            Debug.Log($"var {above} = {mPos.y} - {tooltipSize.y} < 0");

            GetComponent<RectTransform>().pivot = GetPivots(above, left);
            transform.position = mPos;
        }
        

        // private void PositionTooltip(RectTransform anchorRect = null)
        // {
        //     // Required to ensure corners are updated by positioning elements.
        //     Canvas.ForceUpdateCanvases();

        //     var anchorCorners = new Vector3[4];
        //     if (anchorRect) { anchorRect.GetWorldCorners(anchorCorners); }
        //     var corners = new Vector3[4];
        //     GetComponent<RectTransform>().GetWorldCorners(corners);
        //     Debug.Log($"{corners[0]}, {corners[1]}, {corners[2]}, {corners[3]}");
        //     Debug.Log($"{anchorCorners[0]}, {anchorCorners[1]}, {anchorCorners[2]}, {anchorCorners[3]}");

        //     bool below = (anchorRect ? anchorCorners[0].y + anchorCorners[3].y / 2
        //                   : Input.mousePosition.x) > Screen.height / 2;
        //     bool right = (anchorRect ? anchorCorners[0].x + anchorCorners[1].x / 2
        //                   : Input.mousePosition.y) < Screen.width / 2;

        //     int slotCorner = GetCornerIndex(below, right);
        //     int tooltipCorner = GetCornerIndex(below, !right);

        //     transform.position = anchorCorners[slotCorner] - corners[tooltipCorner] + transform.position;
        // }

        // pivots 0,1  1,1
        //        0,0  1,0
        private Vector2 GetPivots(bool above, bool left)
        {
            Debug.Log($"Above: {above}, left: {left}");
            if (above && !left) { return new Vector2(0, 0); }       // Bottom Left
            else if (!above && !left) { return new Vector2(0, 1); } // Top Left
            else if (!above && left) { return new Vector2(1, 1); }  // Top Right
            else { return new Vector2(1, 0); }                      // Bottom Right
        }
    }
}