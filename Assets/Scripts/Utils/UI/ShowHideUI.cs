using UnityEngine;
using UnityEngine.Events;

namespace SDVA.Utils.UI
{
    public class ShowHideUI : MonoBehaviour
    {
        /// <summary>
        /// Key press that triggers events.
        /// </summary>
        [SerializeField] KeyCode toggleKey = KeyCode.Escape;
        /// <summary>
        /// Event called whenever Toggle Key is pressed.
        /// Alternates between false when main UI should be hidden and true when main UI should be shown.
        /// </summary>
        [SerializeField] UnityEvent<bool> Toggled;
        /// <summary>
        /// Event called whenever Toggle Key is pressed.
        /// Alternates between true when main UI should be hidden and false when main UI should be shown.
        /// </summary>
        [SerializeField] UnityEvent<bool> Inverse;
        /// <summary>
        /// Should main UI start game showing?
        /// </summary>
        [SerializeField] bool showingUI = false;

        // Start is called before the first frame update
        void Start()
        {
            Toggled ??= new UnityEvent<bool>();
            Inverse ??= new UnityEvent<bool>();
            
            showingUI = false;
            Toggled.Invoke(showingUI);
            Inverse.Invoke(!showingUI);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                showingUI = !showingUI;
                Toggled.Invoke(showingUI);
                Inverse.Invoke(!showingUI);
            }
        }
    }
}