using Audio;
using UnityEngine;

namespace EndlessTavernUI
{
    public class ADVCanvas : MonoBehaviour
    {
        private void OnEnable() => AudioPlayer.instance.PauseBGM();

        private void OnDisable() => AudioPlayer.instance.ResumeBGM();
    }
}
