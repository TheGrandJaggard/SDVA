using UnityEngine;
using TMPro;

namespace SDVA.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DamageTextScript : MonoBehaviour
    {
        private DamageTextPool pool;
        private float secondsAlive = 0;
        
        public void SetPool(DamageTextPool pool)
        {
            this.pool = pool;
        }

        private void OnEnable()
        {
            GetComponent<RectTransform>().rotation = Camera.main.transform.rotation;
            GetComponent<TextMeshPro>().alpha = 1f;
            secondsAlive = 0;
        }

        private void Update()
        {
            if (secondsAlive < 1.5f)
            {
                secondsAlive += Time.deltaTime;
                GetComponent<TextMeshPro>().alpha -= 0.03f;
                transform.position += Vector3.up * 0.03f;
            }
            else
            {
                pool.pool.Release(this);
            }
        }

        public void SetPosition(Vector3 position)
        {
            position += Vector3.up * 1; // to put us above the head
            transform.position = position;
        }

        public void SetDamage(int damage) => GetComponent<TextMeshPro>().text = damage.ToString();
    }
}
