using UnityEngine;

namespace SDVA.Utils
{
    public class PointAtCursor : MonoBehaviour
    {
        // Update is called once per frame
        void FixedUpdate()
        {
            var cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var direction = new Vector2(cursorPosition.x - transform.position.x, cursorPosition.y - transform.position.y);
            transform.right = direction;

            if (direction.x <= 0)
            {
                transform.transform.eulerAngles = new Vector3(
                    transform.transform.eulerAngles.x + 180,
                    transform.transform.eulerAngles.y,
                    -transform.transform.eulerAngles.z
                );
            }
        }
    }
}