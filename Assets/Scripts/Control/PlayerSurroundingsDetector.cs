using System.Collections.Generic;
using SDVA.InventorySystem;
using SDVA.UI.InventorySystem;
using UnityEngine;

namespace SDVA.Control
{
    public class PlayerSurroundingsDetector : MonoBehaviour
    {
        // Exposed fields for setting parameters in Unity Inspector
        [Tooltip("This directly impacts performance (I don't know how much though). It sets the maximum range for all other ranges. Should be larger than all other ranges.")]
        [SerializeField] float detectionRange = 4f;
        [SerializeField] float guiProviderRange = 1f;
        [SerializeField] float itemMagnetRange = 3f;
        [SerializeField] float itemMagnetMovementSpeed = 0.1f;
        [SerializeField] InventoryUIController inventoryController;
        private List<GameObject> guiProvidersInRange;

        private void Awake()
        {
            guiProvidersInRange = new();
        }

        private void Update()
        {
            foreach (var item in Physics2D.OverlapCircleAll(transform.position, detectionRange))
            {
                if (item.gameObject.Equals(gameObject)) { continue; }
                if (item.transform.IsChildOf(transform)) { continue; }

                if (item.TryGetComponent<IGuiProvider>(out var provider)
                    && Vector2.SqrMagnitude(item.transform.position - transform.position) < Mathf.Pow(guiProviderRange, 2)
                    && !guiProvidersInRange.Contains(item.gameObject))
                {
                    guiProvidersInRange.Add(item.gameObject);
                    inventoryController.AddGuiToOtherTab(provider);
                }
                else if (item.TryGetComponent<Pickup>(out var pickup)
                    && Vector2.SqrMagnitude(item.transform.position - transform.position) < Mathf.Pow(itemMagnetRange, 2))
                {
                    DoPickupLogic(pickup);
                }
            }

            for (int i = 0; i < guiProvidersInRange.Count; i++)
            {
                GameObject item = guiProvidersInRange[i];
                if (Vector2.SqrMagnitude(item.transform.position - transform.position) > Mathf.Pow(guiProviderRange, 2))
                {
                    guiProvidersInRange.Remove(item);
                    inventoryController.RemoveGuiFromOtherTab(item.GetComponent<IGuiProvider>());
                    i --; // to account for our removal of an element
                }
            }
        }

        /// <summary>
        /// Applies magnetism to pickups so they will come to the player to be picked up.
        /// </summary>
        /// <param name="pickup">The pickup to magnetize.</param>
        private void DoPickupLogic(Pickup pickup)
        {
            // TODO
        }
    }
}