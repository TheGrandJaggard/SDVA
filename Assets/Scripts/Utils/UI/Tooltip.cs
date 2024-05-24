using System.Collections;
using ModTool.Shared;
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
        // PRIVATE STATE
        private bool followMouse = true;

        // PUBLIC

        /// <summary>
        /// Anchors this tooltip's position.
        /// </summary>
        /// <param name="anchorRect">The UI element to anchor the tooltip to. Null by default.
        /// When null, the tooltip is anchored to the cursor.</param>
        public void Anchor(RectTransform anchorRect)
        {
            followMouse = false;
            StartCoroutine(PositionTooltip(anchorRect));
        }

        // LIFECYCLE

        void Update()
        {
            if (followMouse)
            {
                StartCoroutine(PositionTooltip());
            }
        }

        // PRIVATE

        private IEnumerator PositionTooltip(RectTransform anchorRect = null)
        {
            yield return new WaitForEndOfFrame();
            // Required to ensure corners are updated by positioning elements.
            Canvas.ForceUpdateCanvases();
            Debug.Log("Starting to position");

            var corners = new Vector3[4];
            GetComponent<RectTransform>().GetWorldCorners(corners);
            var tooltipSize = new Vector2(Mathf.Abs(corners[1].x -corners[2].x), Mathf.Abs(corners[0].y -corners[1].y));

            var anchorCorners = new Vector3[4]; // 0:BL, 1: TL, 2:TR, 3:BR
            if (anchorRect == null)
            {
                var mPos = Input.mousePosition;
                anchorCorners = new Vector3[] {mPos, mPos, mPos, mPos};
            }
            else { anchorRect.GetWorldCorners(anchorCorners); }

            var left = anchorCorners[3].x + tooltipSize.x > Screen.width;
            Debug.Log($"var {left} = {anchorCorners[3].x} + {tooltipSize.x} > {Screen.width};");
            var above = anchorCorners[0].y - tooltipSize.y < 0;
            Debug.Log($"var {above} = {anchorCorners[0].y} - {tooltipSize.y} < 0");

            GetComponent<RectTransform>().pivot = GetPivots(above, left);
            transform.position = anchorCorners[GetCornerIndex(above, left)];
            
            transform.GetChild(0).gameObject.SetActive(true);
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

        private int GetCornerIndex(bool above, bool left)
        {
            Debug.Log($"Above: {above}, left: {left}");
            if (!above && left) { return 0; }       // Bottom Left
            else if (above && left) { return 1; }   // Top Left
            else if (above && !left) { return 2; }  // Top Right
            else { return 3; }                      // Bottom Right
        }

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