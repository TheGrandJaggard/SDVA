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

        [SerializeField] float magnetism;

        private void Start()
        {
            SetupTweens();
        }

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

        private void Update()
        {
            transform.position = movementUpdates
                               + Vector3.up * bouncePos
                               + ItemMagnetism() * Time.deltaTime;
            
            // new Vector3(movementUpdates.x, movementUpdates.y + bouncePos, movementUpdates.z);
        }

        private Vector3 ItemMagnetism() // TODO this seems very non-performant
        {
            var magnetismDirection = new Vector3();
            foreach (var other in Physics2D.OverlapCircleAll(transform.position, 0.3f))
            {
                Debug.Log("Hit " + other.name);
                if (other.TryGetComponent<ItemPickup>(out var otherItem)
                    && ReferenceEquals(GetItem(), otherItem.GetItem()))
                {
                    Debug.Log("magging " + other.name);
                    magnetismDirection += (transform.position - other.transform.position) * magnetism;
                    if ((transform.position - other.transform.position).sqrMagnitude < 0.01f)
                    {
                        Setup(GetItem(), GetNumber() + otherItem.GetNumber());
                    }
                }
            }
            return magnetismDirection;
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