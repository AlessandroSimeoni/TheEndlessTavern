using UnityEngine;
using UnityEngine.UI;

namespace Customization
{
    [DisallowMultipleComponent]
    [RequireComponent (typeof(Button))]
    public class CustomizationButton : MonoBehaviour
    {
        [SerializeField] private Button button = null;
        [SerializeField] private GameObject lockGameObject = null;

        public bool isLocked { get { return lockGameObject.activeInHierarchy; } }

        public void Toggle(bool value)
        {
            button.interactable = value;
            lockGameObject.SetActive(!value);
        }
    }
}
