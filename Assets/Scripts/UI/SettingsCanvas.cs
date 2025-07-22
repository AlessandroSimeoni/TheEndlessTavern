using GameSave;
using UnityEngine;
using UnityEngine.UI;

namespace EndlessTavernUI
{
    public class SettingsCanvas : MonoBehaviour
    {
        [SerializeField] private Toggle masterAudioToggle = null;
        [SerializeField] private Toggle bgmAudioToggle = null;
        [SerializeField] private Toggle sfxAudioToggle = null;

        public void ToggleSettings(GameData gameData)
        {
            masterAudioToggle.SetIsOnWithoutNotify(gameData.masterAudioEnabled);
            bgmAudioToggle.SetIsOnWithoutNotify(gameData.bgmAudioEnabled);
            sfxAudioToggle.SetIsOnWithoutNotify(gameData.sfxAudioEnabled);
        }
    }
}
