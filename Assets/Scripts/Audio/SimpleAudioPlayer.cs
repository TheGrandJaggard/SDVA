using UnityEngine;

namespace SDVA.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SimpleAudioPlayer : MonoBehaviour
    {
        private AudioPlayerPool pool;
        private float secondsAlive = 1f;
        private float clipLength = 1f;
        
        public void SetPool(AudioPlayerPool pool)
        {
            this.pool = pool;
        }

        public void SetPosition(Vector3 position) => transform.position = position;

        public void SetPitch(float pitch) => GetComponent<AudioSource>().pitch = pitch;
        
        public void SetClip(AudioClip clip)
        {
            GetComponent<AudioSource>().clip = clip;
            GetComponent<AudioSource>().Play();
            clipLength = clip.length;
        }

        private void OnEnable()
        {
            secondsAlive = 0f;
        }

        private void Update()
        {
            if (secondsAlive < clipLength)
            {
                secondsAlive += Time.deltaTime;
            }
            else
            {
                pool.pool.Release(this);
            }
        }
    }
}