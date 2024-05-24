using UnityEngine;
using SDVA.Saving;
using Newtonsoft.Json.Linq;

namespace SDVA.Control
{
    public class BasicMovement : MonoBehaviour, IJsonSaveable
    {
        // Exposed fields for setting parameters in Unity Inspector
        [SerializeField] private float speed = 1f;

        void FixedUpdate()
        {
            float userInputV = Input.GetAxis("Vertical");
            float userInputH = Input.GetAxis("Horizontal");

            transform.position = new Vector3(
                transform.position.x + userInputH * speed * Time.deltaTime,
                transform.position.y + userInputV * speed * Time.deltaTime,
                transform.position.z);
        }

        JToken IJsonSaveable.CaptureAsJToken()
        {
            return transform.position.ToToken();
        }

        void IJsonSaveable.RestoreFromJToken(JToken state)
        {
            transform.position = state.ToVector3();
        }
    }
}