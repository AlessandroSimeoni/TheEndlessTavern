using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    [CreateAssetMenu(fileName = "AudioSettings", menuName = "ScriptableObjects/Audio/AudioSettings")]
    public class MyAudioSettings : ScriptableObject
    {
        [Header("Audio mixer groups")]
        public AudioMixerGroup masterAudioMixerGroup = null;
        public AudioMixerGroup bgmAudioMixerGroup = null;
        public AudioMixerGroup sfxAudioMixerGroup = null;

        [Header("Audio Fading from Settings")]
        [Min(0.0f), Tooltip("The fade time in seconds when audio is toggled from the settings")]
        public float settingsFadeTime = 1.0f;
        [Tooltip("If true, audio will fade when settings are toggled")]
        public bool settingsFadeEnabled = true;
        [Header("BGM Fading")]
        [Min(0.0f), Tooltip("The fade time in seconds when bgm must change from one scene to another")]
        public float bgmFadeTime = 1.0f;
        [Tooltip("If true, bgm will fade at start when played")]
        public bool bgmStartFadeEnabled = true;
        [Tooltip("If true, bgm will fade at end when stopped")]
        public bool bgmEndFadeEnabled = true;

    }
}
