﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace SDVA.Utils.UI
{
    /// <summary>
    /// Abstract base class that handles the spawning of a tooltip prefab at the
    /// correct position on screen relative to a cursor.
    /// 
    /// Override the abstract functions to create a tooltip spawner for your own
    /// data.
    /// </summary>
    public abstract class TooltipSpawner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // CONFIG DATA
        [Tooltip("The prefab of the tooltip to spawn.")]
        [SerializeField] GameObject tooltipPrefab;

        // PRIVATE STATE
        private Tooltip tooltip;

        /// <summary>
        /// Called when it is time to update the information on the tooltip
        /// prefab.
        /// </summary>
        /// <param name="tooltip">
        /// The spawned tooltip prefab for updating.
        /// </param>
        public abstract void UpdateTooltip(Tooltip tooltip);
        
        /// <summary>
        /// Return true when the tooltip spawner should be allowed to create a tooltip.
        /// </summary>
        public abstract bool CanCreateTooltip();

        // PRIVATE

        private void OnDestroy()
        {
            ClearTooltip();
        }

        private void OnDisable()
        {
            ClearTooltip();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (!CanCreateTooltip()) { return;}
            
            if (!tooltip)
            {
                var parentCanvas = GetComponentInParent<Canvas>();
                var tooltipObject = Instantiate(tooltipPrefab, parentCanvas.transform);

                tooltip = tooltipObject.GetComponent<Tooltip>();
            }
            tooltip.Anchor(GetComponent<RectTransform>());
            UpdateTooltip(tooltip);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            ClearTooltip();
        }

        private void ClearTooltip()
        {
            if (tooltip != null)
            {
                Destroy(tooltip.gameObject);
            }
        }

        // private void PositionTooltip()
        // {
        //     // Required to ensure corners are updated by positioning elements.
        //     Canvas.ForceUpdateCanvases();

        //     var tooltipCorners = new Vector3[4];
        //     tooltip.GetComponent<RectTransform>().GetWorldCorners(tooltipCorners);
        //     var slotCorners = new Vector3[4];
        //     GetComponent<RectTransform>().GetWorldCorners(slotCorners);

        //     bool below = transform.position.y > Screen.height / 2;
        //     bool right = transform.position.x < Screen.width / 2;

        //     int slotCorner = GetCornerIndex(below, right);
        //     int tooltipCorner = GetCornerIndex(!below, !right);

        //     tooltip.transform.position = slotCorners[slotCorner] - tooltipCorners[tooltipCorner] + tooltip.transform.position;
        // }

        // private int GetCornerIndex(bool below, bool right)
        // {
        //     if (below && !right) return 0;
        //     else if (!below && !right) return 1;
        //     else if (!below && right) return 2;
        //     else return 3;
        // }
    }
}