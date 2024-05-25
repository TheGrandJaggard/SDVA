using DG.Tweening;
using UnityEngine;

namespace SDVA.InventorySystem
{
    public class ItemPickup : Pickup
    {
        private Vector3 movementUpdates;
        [SerializeField] float movementTime = 2f;

        private float bouncePos = 0f;
        [SerializeField] float bounceHeight = 0.4f;
        [SerializeField] int bounceNum = 3;

        private Vector3 startPos;
        private Vector3 endPos;

        private void Start()
        {
            startPos = transform.position;
            endPos = new Vector3(transform.position.x + Random.onUnitSphere.x * 3f,
                transform.position.y + Random.onUnitSphere.y * 3f,
                transform.position.z);

            // This tween should move the pickup to the end position, updating movementUpdates.
            DOTween.To(() => startPos, (Vector3 newVector) => movementUpdates = newVector, endPos, movementTime).SetEase(Ease.OutElastic);
            
            // This sequence should bounce the pickup, updating bouncePos. It contains one tween for up, and another for down
            Sequence bounceSeq = DOTween.Sequence().SetLoops(bounceNum, LoopType.Restart);
            bounceSeq.Append(DOTween.To(() => bouncePos, (float newFloat) => bouncePos = newFloat, bounceHeight, movementTime / (bounceNum * 2))
                .SetEase(Ease.OutCubic));
            bounceSeq.Append(DOTween.To(() => bouncePos, (float newFloat) => bouncePos = newFloat, 0, movementTime / (bounceNum * 2))
                .SetEase(Ease.InCubic));
            
            // This tween should reduce bounce height
            DOTween.To(() => bounceHeight, (float newFloat) => bounceHeight = newFloat, 0f, movementTime);
        }

        private void Update()
        {
            if (movementUpdates == new Vector3()) { return; }
            transform.position = new Vector3(movementUpdates.x, movementUpdates.y + bouncePos, movementUpdates.z);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PickupItem();
            }
        }
    }
}