using UnityEngine;

namespace SDVA.Saving
{
    [RequireComponent(typeof(JsonSavingSystem))]
    public class SavingWrapper : MonoBehaviour
    {
        const string saveFile = "Test-o-save-o";
        JsonSavingSystem MySavingSystem;

        private void Awake()
        {
            MySavingSystem = GetComponent<JsonSavingSystem>();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                LoadLastScene();
            }
            else if(Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            else if(Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            else if(Input.GetKeyDown(KeyCode.D))
            {
                Delete();
            }
        }

        private void Delete()
        {
            MySavingSystem.Delete(saveFile);
            print("Save File Deleted");
        }

        public void Save()
        {
            MySavingSystem.Save(saveFile);
            print("Game Saved (in an unepic way)");
        }

        public void Load()
        {
            MySavingSystem.Load(saveFile);
            print("Game Loaded");
        }

        public void LoadLastScene()
        {
            StartCoroutine(MySavingSystem.LoadLastScene(saveFile));
            print("Last Scene Done Loading");
        }
    }
}
