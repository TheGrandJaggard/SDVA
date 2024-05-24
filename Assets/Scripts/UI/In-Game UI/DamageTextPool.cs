using UnityEngine;
using UnityEngine.Pool;

namespace SDVA.UI
{
    public class DamageTextPool : MonoBehaviour
    {
        public ObjectPool<DamageTextScript> pool;
        [SerializeField] private DamageTextScript damageTextScriptPrefab;

        private void Awake()
        {
            pool = new ObjectPool<DamageTextScript>(
                CreateObject, OnGetObject, OnReleaseObject, OnDestroyObject, maxSize: 5);
        }
        
        private DamageTextScript CreateObject()
        {
            DamageTextScript damageText = Instantiate(damageTextScriptPrefab);
            damageText.SetPool(this);
            return damageText;
        }

        private void OnGetObject(DamageTextScript damageTextScript)
        {
            damageTextScript.gameObject.SetActive(true);
        }

        private void OnReleaseObject(DamageTextScript damageTextScript)
        {
            damageTextScript.gameObject.SetActive(false);
        }

        private void OnDestroyObject(DamageTextScript damageTextScript)
        {
            Destroy(damageTextScript.gameObject);
        }
    }
}