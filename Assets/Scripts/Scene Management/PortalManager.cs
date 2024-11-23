using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
// using SDVA.Control;
using SDVA.Saving;
using SDVA.UI;

namespace SDVA.SceneManagement
{
    public class PortalManager : MonoBehaviour
    {
        [SerializeField] float fadeInTime = 1f;
        [SerializeField] float waitBetweenFadeTime = 2f;
        [SerializeField] float fadeOutTime = 1f;
        // PlayerController player;
        SavingWrapper savingWrapper;

        private void Awake()
        {
            savingWrapper = FindObjectOfType<SavingWrapper>();
            // player = FindObjectOfType<PlayerController>();
            
            savingWrapper.LoadLastScene();
            StartCoroutine(WaitToFadeIn());
        }

        private IEnumerator WaitToFadeIn()
        {
            Fader fader = GetComponentInChildren<Fader>();
            fader.SetAlpha(1f);
            yield return new WaitForSecondsRealtime(waitBetweenFadeTime);
            yield return fader.FadeOut(fadeInTime);
        }

        public void TeleportTo(string scene, string portalName)
        {
            StartCoroutine(Teleport(scene, portalName));
        }

        private IEnumerator Teleport(string scene, string portalName)
        {
            // player.SetControl(false);
            savingWrapper.Save();

            yield return transform.GetComponentInChildren<Fader>().FadeIn(fadeOutTime);
            yield return SceneManager.LoadSceneAsync(scene);
            // player.SetControl(false);
            
            savingWrapper.Load();

            // player = FindObjectOfType<PlayerController>();
            // player.GetComponent<NavMeshAgent>().Warp(GameObject.Find(portalName).transform.GetChild(0).position);
            // player.transform.rotation = GameObject.Find(portalName).transform.GetChild(0).rotation;

            yield return WaitToFadeIn();
            // player.SetControl(true);
        }
    }
}
