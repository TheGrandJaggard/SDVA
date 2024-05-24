using System.Collections;
using UnityEngine;

namespace SDVA.Utils
{
    public class DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] bool destroyAfterParticleEffect = false;
        [SerializeField] float waitTime;
        
        void Start()
        {
            if (!destroyAfterParticleEffect) { StartCoroutine(DestroyTimer()); }
        }

        void Update()
        {
            if (destroyAfterParticleEffect && GetComponent<ParticleSystem>() != null && GetComponent<ParticleSystem>().IsAlive() == false)
            { Destroy(gameObject); }
        }

        IEnumerator DestroyTimer()
        {
            yield return new WaitForSeconds(waitTime);
            Destroy(gameObject);
        }
    }
}