using UnityEngine;
using UnityEngine.EventSystems;

namespace SDVA.Utils.UI
{
    /// <summary>
    /// Abstract base class that handles the spawning of a tooltip prefab.
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
    }
}
