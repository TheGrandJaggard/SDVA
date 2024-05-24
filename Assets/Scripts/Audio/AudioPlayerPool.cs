using UnityEngine;
using UnityEngine.Pool;

namespace SDVA.Audio
{
    public class AudioPlayerPool : MonoBehaviour
    {
        public ObjectPool<SimpleAudioPlayer> pool;
        [SerializeField] private SimpleAudioPlayer audioPlayerPrefab;

        private void Awake()
        {
            pool = new ObjectPool<SimpleAudioPlayer>(
                CreateObject, OnGetObject, OnReleaseObject, OnDestroyObject, maxSize: 4);
        }
        
        private SimpleAudioPlayer CreateObject()
        {
            SimpleAudioPlayer audioPlayer = Instantiate(audioPlayerPrefab);
            audioPlayer.SetPool(this);
            return audioPlayer;
        }

        private void OnGetObject(SimpleAudioPlayer audioPlayer)
        {
            audioPlayer.gameObject.SetActive(true);
        }

        private void OnReleaseObject(SimpleAudioPlayer audioPlayer)
        {
            audioPlayer.gameObject.SetActive(false);
        }

        private void OnDestroyObject(SimpleAudioPlayer audioPlayer)
        {
            Destroy(audioPlayer.gameObject);
        }
    }
}