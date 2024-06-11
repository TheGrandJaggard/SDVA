using UnityEngine;
using SDVA.Saving;
using UnityEngine.InputSystem;
using Newtonsoft.Json.Linq;

namespace SDVA.Control
{
    public class BasicMovement : MonoBehaviour, IJsonSaveable
    {
        // Exposed fields for setting parameters in Unity Inspector
        [SerializeField] float speed = 1f;
        private PlayerWorldInputActions playerControls;
        private InputAction move;

        private void OnEnable()
        {
            playerControls = new PlayerWorldInputActions();
            move = playerControls.Player.Move;
            move.Enable();
            // fire.performed += function;
        }

        private void OnDisable()
        {
            move.Disable();
        }

        void FixedUpdate()
        {
            transform.position += (Vector3)(speed * Time.deltaTime * move.ReadValue<Vector2>());
        }

        #region Saving
        JToken IJsonSaveable.CaptureAsJToken()
        {
            return transform.position.ToToken();
        }

        void IJsonSaveable.RestoreFromJToken(JToken state)
        {
            transform.position = state.ToVector3();
        }
        #endregion
    }
}