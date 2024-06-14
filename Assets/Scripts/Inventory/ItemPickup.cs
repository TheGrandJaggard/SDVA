using DG.Tweening;
using UnityEngine;
using SDVA.Utils;

namespace SDVA.InventorySystem
{
    public class ItemPickup : Pickup
    {
        private Vector3 movementUpdates;
        [SerializeField] float movementTime;
        [SerializeField] float movementDistance;

        private float bouncePos = 0f;
        [SerializeField] float bounceHeight;
        [SerializeField] int bounceNum;
        private float bounceTime;

        private void Start()
        {
            SetupTweens();
        }

        // TODO: ignore player for a small amount of time

        private void SetupTweens()
        {
            bounceTime = movementTime / (bounceNum * 2);
            var startPos = transform.position;
            var endPos = (Vector3)Random.insideUnitCircle * movementDistance + transform.position;

            // This tween moves the pickup to the end position, updating movementUpdates.
            DOTween.To(() => startPos, (Vector3 newVector) => movementUpdates = newVector, endPos, movementTime)
                .SetEase(CustomEase.InHalfer);

            // This tween bounces the object
            StartBounce();

            // This tween reduces bounce height
            DOTween.To(() => bounceHeight, (float newFloat) => bounceHeight = newFloat, 0f, movementTime)
                .SetEase(Ease.Linear);

            // This tween reduces bounce time
            DOTween.To(() => bounceTime, (float newFloat) => bounceTime = newFloat, bounceTime / 2, movementTime)
                .SetEase(Ease.Linear);
        }

        private void StartBounce()
        {
            if (bounceNum == 0) { return; }
            bounceNum -= 1;

            // This sequence should bounce the pickup, updating bouncePos. It contains one tween for up, and another for down.
            Sequence bounceSeq = DOTween.Sequence();
            bounceSeq.Append(DOTween.To(() => bouncePos, (float newFloat) => bouncePos = newFloat, bounceHeight, bounceTime)
                .SetEase(Ease.OutQuad));
            bounceSeq.Append(DOTween.To(() => bouncePos, (float newFloat) => bouncePos = newFloat, 0, bounceTime)
                .SetEase(Ease.InQuad));
            bounceSeq.onComplete += StartBounce;
        }

        // TODO: add magnetism

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