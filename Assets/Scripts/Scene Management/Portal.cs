using UnityEngine;

namespace SDVA.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private string destinationScene;
        [SerializeField] private string destinationPortalName;
        private PortalManager portalManager;

        void Awake()
        {
            GameObject portalManager = GameObject.FindGameObjectWithTag("PortalManager");
            if (portalManager != null)
            {
                this.portalManager = portalManager.GetComponent<PortalManager>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") &&
                !string.IsNullOrWhiteSpace(destinationScene) && !string.IsNullOrWhiteSpace(destinationPortalName))
            {
                portalManager.TeleportTo(destinationScene, destinationPortalName);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Transform SpawnPos = transform.GetChild(0);
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(SpawnPos.position + SpawnPos.forward * 0.5f, 0.2f);
            Gizmos.DrawSphere(SpawnPos.position, 0.4f);
        }
    }
}
