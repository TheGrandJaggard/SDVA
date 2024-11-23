using UnityEngine;

namespace SDVA.Mines
{
    public class DrillController : MonoBehaviour
    {
        [SerializeField] float mineSpeed = 1f;
        [SerializeField] string toolType = "Drill";
        [SerializeField] int toolLevel = 9;
        [SerializeField] float mineDelay = 0.5f;
        private float drillCooldown = 0;
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.GetComponentInParent<MineController>() == null) { return; }
            if (drillCooldown > 0) { return; }
            float timingFactor = mineDelay == 0 ? Time.deltaTime : mineDelay;
            drillCooldown = mineDelay;

            CircleCollider2D drillCollider = GetComponent<CircleCollider2D>();

            Vector3 relativeDrillTipPos = transform.rotation * new Vector3(drillCollider.offset.x, drillCollider.offset.y);
            Vector3 worldDrillTipPos = relativeDrillTipPos + transform.position;
            Vector3Int tilePos = new(Mathf.RoundToInt(worldDrillTipPos.x - 0.5f), Mathf.RoundToInt(worldDrillTipPos.y - 0.5f), 0);

            other.GetComponentInParent<MineController>().MineTile(tilePos, toolType, toolLevel, mineSpeed * timingFactor);
        }

        private void Update() {
            drillCooldown -= Time.deltaTime;
        }
    }
}
