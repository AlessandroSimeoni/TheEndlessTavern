using UnityEngine;

namespace EndlessTavernUI
{
    public class URLButton : MonoBehaviour
    {
        public string url { get; set; } = "";

        public void OpenURL() => Application.OpenURL(url);
    }
}