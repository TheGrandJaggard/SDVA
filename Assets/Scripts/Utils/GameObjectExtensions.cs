using UnityEngine;

namespace SDVA.Utils
{
    public static class GameObjectExtensions
    {
        private static readonly string destroyedTag = "TO_BE_DESTROYED";

        public static void Destroy(this GameObject gameObject)
        {
            if (gameObject.CompareTag(destroyedTag)) { return; }
            gameObject.tag = destroyedTag;
            Object.Destroy(gameObject);
        }

        public static bool IsDestroyed(this GameObject gameObject)
        {
            return gameObject.CompareTag(destroyedTag);
        }
    }
}
