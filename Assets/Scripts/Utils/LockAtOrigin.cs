using UnityEngine;

namespace SDVA.Utils
{
    public class LockAtOrigin : MonoBehaviour
    {
        #if UNITY_EDITOR
            void Awake()
            {
                if (transform.position != new Vector3())
                {
                    Debug.LogError("This object is not set to the origin", this);
                    transform.position = new();
                }
            }
        #endif
    }
}
