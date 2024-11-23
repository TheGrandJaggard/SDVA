using UnityEngine;

namespace SDVA.SceneManagement
{
    public class CreatePersistantObjects : MonoBehaviour
    {
        [SerializeField] GameObject[] PersistantObjects;
        static bool DoneAlready = false;

        private void Awake()
        {
            if (DoneAlready) { return; }
            DoneAlready = true;
            
            foreach (GameObject Object in PersistantObjects)
            {
                DontDestroyOnLoad(Instantiate(Object));
            }
        }
    }
}
