using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
	public class AudioPlayer : MonoBehaviour
	{
		private bool masterVolumeOn = true;
		private bool bgmVolumeOn = true;
		private bool sfxVolumeOn = true;
		private Queue<GameObject> bgmQueue = new Queue<GameObject>();

		private static AudioPlayer _instance = null;
		public static AudioPlayer instance
		{
			get
			{
				if (_instance == null)
				{
					GameObject go = new GameObject("Audio Player");
					_instance = go.AddComponent<AudioPlayer>();
					DontDestroyOnLoad(go);
				}

				return _instance;
			}
		}

		public MyAudioSettings settings { private get; set; } = null;

		private GameObject bgmObject = null;

		private const string MASTER_VOLUME = "MasterVolume";
		private const string BGM_VOLUME = "BGMVolume";
		private const string SFX_VOLUME = "SFXVolume";

        /// <summary>
        /// Setup the audio source component
        /// </summary>
        /// <param name="go">the gameobject</param>
        /// <param name="clip">the audio clip</param>
        /// <param name="mixerGroup">the output audio mixer group</param>
        /// <param name="volume">the volume of the audio source</param>
        /// <returns>the audiosource component</returns>
        private AudioSource SetupAudioSource(GameObject go, AudioClip clip, AudioMixerGroup mixerGroup, float volume)
		{
			AudioSource audioSource = go.AddComponent<AudioSource>();
			audioSource.outputAudioMixerGroup = mixerGroup;
			audioSource.volume = volume;
			audioSource.clip = clip;

			return audioSource;
		}

		/// <summary>
		/// Play bgm music 
		/// </summary>
		/// <param name="clip">the audio clip</param>
		/// <param name="volume">the audio clip volume</param>
		public void PlayBGM(AudioClip clip, float volume = 1.0f)
		{
			bgmObject = new GameObject($"sfx {clip.name}");
            DontDestroyOnLoad(bgmObject);
			bgmQueue.Enqueue(bgmObject);

            AudioSource audioSource = SetupAudioSource(bgmObject, clip, settings.bgmAudioMixerGroup, volume);
			audioSource.volume = settings.bgmStartFadeEnabled ? 0.0f : 1.0f;
			audioSource.loop = true;
			audioSource.Play();

			if (settings.bgmStartFadeEnabled)
				StartCoroutine(FadeVolume(audioSource, audioSource.volume, 1.0f, settings.bgmFadeTime));
		}

		public void StopBGM() => StartCoroutine(StopBGMCoroutine());

		/// <summary>
		/// play an sfx once
		/// </summary>
		/// <param name="clip">the audio clip</param>
		/// <param name="volume">the volume</param>
		/// <param name="dontDestroyOnLoad">don't destroy on load?</param>
		public void PlaySFX(AudioClip clip, float volume = 1.0f, bool dontDestroyOnLoad = false)
		{
			if (!masterVolumeOn || !sfxVolumeOn)
				return;

			GameObject go = new GameObject($"sfx {clip.name}");

			AudioSource audioSource = SetupAudioSource(go, clip, settings.sfxAudioMixerGroup, volume);
			audioSource.Play();

			if (dontDestroyOnLoad)
				DontDestroyOnLoad(go);

			// destroy the game object at the end of the clip
			Destroy(go, clip.length);
		}

		/// <summary>
		/// Play an sfx in loop
		/// </summary>
		/// <param name="clip">the audio clip</param>
		/// <param name="volume">the volume of the clip</param>
		/// <returns>the audio source playing the clip or null if the clip cannot be played</returns>
		public AudioSource LoopSFX(AudioClip clip, float volume = 1.0f)
		{
            if (!masterVolumeOn || !sfxVolumeOn)
                return null;

            GameObject go = new GameObject($"sfx {clip.name}");

			AudioSource audioSource = SetupAudioSource(go, clip, settings.sfxAudioMixerGroup, volume);
			audioSource.loop = true;
			audioSource.Play();

			return audioSource;
		}

		/// <summary>
		/// Create a gameobject with an audio source for the specified audio clip ready to be played
		/// </summary>
		/// <param name="clip">the audio clip</param>
		/// <param name="volume">the volume of the clip</param>
		/// <returns>the audio source component</returns>
		public AudioSource InitSFX(AudioClip clip, float volume = 1.0f)
		{
            GameObject go = new GameObject($"sfx {clip.name}");
            AudioSource audioSource = SetupAudioSource(go, clip, settings.sfxAudioMixerGroup, volume);
			audioSource.playOnAwake = false;

			return audioSource;
        }

        /// <summary>
        /// toggle master audio
        /// </summary>
        /// <param name="value">true --> turn ON master; false --> turn OFF master</param>
        /// <param name="fade">true (by default) --> fade audio; false --> don't fade audio</param>
        public void ToggleMaster(bool value, bool fade = true)
		{
			masterVolumeOn = value;
			if (fade)
				ToggleVolume(settings.masterAudioMixerGroup, MASTER_VOLUME, masterVolumeOn);
			else
                settings.masterAudioMixerGroup.audioMixer.SetFloat(MASTER_VOLUME, masterVolumeOn ? 0.0f : -80.0f);
        }

        /// <summary>
        /// toggle bgm audio
        /// </summary>
        /// <param name="value">true --> turn ON bgm; false --> turn OFF bgm</param>
        /// <param name="fade">true (by default) --> fade audio; false --> don't fade audio</param>
        public void ToggleBGM(bool value, bool fade = true)
		{
			bgmVolumeOn = value;
			if (fade)
				ToggleVolume(settings.bgmAudioMixerGroup, BGM_VOLUME, bgmVolumeOn);
			else
                settings.bgmAudioMixerGroup.audioMixer.SetFloat(BGM_VOLUME, bgmVolumeOn ? 0.0f : -80.0f);
        }

		/// <summary>
		/// toggle sfx audio
		/// </summary>
		/// <param name="value">true --> turn ON sfx; false --> turn OFF sfx</param>
		public void ToggleSFX(bool value)
		{
			sfxVolumeOn = value;
			ToggleVolume(settings.sfxAudioMixerGroup, SFX_VOLUME, sfxVolumeOn);
		}

		/// <summary>
		/// toggle the volume of the specified audio mixer group
		/// </summary>
		/// <param name="mixerGroup">the target audio mixer group</param>
		/// <param name="volumeName">the volume parameter name of the target audio mixer group</param>
		/// <param name="isVolumeOn">true --> turn off audio; false --> turn on audio</param>
		private void ToggleVolume(AudioMixerGroup mixerGroup, string volumeName, bool isVolumeOn)
		{
			float currentVolume;
			mixerGroup.audioMixer.GetFloat(volumeName, out currentVolume);
			if (settings.settingsFadeEnabled)
				StartCoroutine(FadeVolume(mixerGroup, volumeName, currentVolume, isVolumeOn ? 0.0f : -80.0f));
			else
				mixerGroup.audioMixer.SetFloat(volumeName, isVolumeOn ? 0.0f : -80.0f);
		}

        /// <summary>
        /// gradually change volume of the target audio mixer group (used when toggling audio from settings)
        /// </summary>
        /// <param name="mixerGroup">the target audio mixer group</param>
        /// <param name="volumeName">the volume parameter name of the target audio mixer group</param>
        /// <param name="currentVolume">the current volume of the target audio mixer group</param>
        /// <param name="targetVolume">the target volume the audio mixer group must reach</param>
        /// <returns></returns>
        private IEnumerator FadeVolume(AudioMixerGroup mixerGroup, string volumeName, float currentVolume, float targetVolume)
		{
			float currentTime = 0.0f;
			float interpolationValue;

			while(currentTime < settings.settingsFadeTime)
			{
				currentTime += Time.unscaledDeltaTime;

				if (currentTime > settings.settingsFadeTime)
					currentTime = settings.settingsFadeTime;

				interpolationValue = currentTime / settings.settingsFadeTime;
				mixerGroup.audioMixer.SetFloat(volumeName, Mathf.Lerp(currentVolume, targetVolume, interpolationValue));
				yield return null;
			}

			mixerGroup.audioMixer.SetFloat(volumeName, targetVolume);
		}

        /// <summary>
        /// gradually change volume of the target audio source
        /// </summary>
        /// <param name="audioSource">the target audio source</param>
        /// <param name="currentVolume">the normalized current volume of the audio source</param>
        /// <param name="targetVolume">the normalized target volume the audio source must reach</param>
        /// <param name="fadeTime">the fade lenght in seconds</param>
        /// <returns></returns>
        private IEnumerator FadeVolume(AudioSource audioSource, float currentVolume, float targetVolume, float fadeTime)
		{
			float currentTime = 0.0f;
			float interpolationValue;
			float decibelCurrentVolume;
			float decibelTargetVolume;
			float currentDecibelTarget;

			// make sure the volumes aren't equal to zero, otherwise the Log10 doesn't work
            if (currentVolume == 0.0f)
				currentVolume = Mathf.Epsilon;

			if (targetVolume == 0.0f)
				targetVolume = Mathf.Epsilon;

			// convert the normalized volume to decibel
			decibelCurrentVolume = Mathf.Clamp(20.0f * Mathf.Log10(currentVolume), -80.0f, 0.0f);
			decibelTargetVolume = Mathf.Clamp(20.0f * Mathf.Log10(targetVolume), -80.0f, 0.0f);

            while (currentTime < fadeTime)
			{
				currentTime += Time.unscaledDeltaTime;

				if (currentTime > fadeTime)
					currentTime = fadeTime;

				// lerp the decibel volume and then convert again to normalized volume
				interpolationValue = currentTime / fadeTime;
                currentDecibelTarget = Mathf.Lerp(decibelCurrentVolume, decibelTargetVolume, interpolationValue);
                audioSource.volume = Mathf.Pow(10.0f, currentDecibelTarget / 20.0f);
                yield return null;
			}

			audioSource.volume = Mathf.Pow(10.0f, decibelTargetVolume / 20.0f); ;
		}

		/// <summary>
		/// Stop the least recently played background music
		/// </summary>
		/// <returns></returns>
		private IEnumerator StopBGMCoroutine()
		{
			GameObject bgm;

			if (bgmQueue.TryDequeue(out bgm))
			{
				AudioSource audioSource = bgm.GetComponent<AudioSource>();
				if (settings.bgmEndFadeEnabled)
					yield return StartCoroutine(FadeVolume(audioSource, audioSource.volume, 0.0f, settings.bgmFadeTime));
				Destroy(bgm);
			}
		}

		public void PauseBGM()
		{
			if (bgmObject == null)
				return;

			bgmObject.GetComponent<AudioSource>().Pause();
		}

		public void ResumeBGM()
		{
            if (bgmObject == null)
                return;

            bgmObject.GetComponent<AudioSource>().Play();
        }
	}
}
