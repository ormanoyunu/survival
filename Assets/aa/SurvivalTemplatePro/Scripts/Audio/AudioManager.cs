using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SurvivalTemplatePro
{
    /// <summary>
    /// 
    /// </summary>
    public class AudioManager : Singleton<AudioManager> 
	{
        [SerializeField]
		private AudioMixer m_AudioMixer;

		[SerializeField]
		private AudioSource m_2DAudioSource;

		private readonly Dictionary<AudioSource, Coroutine> m_LevelSetters = new Dictionary<AudioSource, Coroutine>();
		private Dictionary<string, AudioMixerGroup> m_AudioMixerGroups;


		public void Play2D(AudioClip clip, float volume)
		{
			if (m_2DAudioSource)
				m_2DAudioSource.PlayOneShot(clip, volume);
		}

		public void SetAudioSourceOutput(AudioSource audioSource, string outputGroupName = "Master")
		{
			if (m_AudioMixerGroups == null)
				GenerateAudioMixerGroupsDictionary();

			if (m_AudioMixerGroups.TryGetValue(outputGroupName, out AudioMixerGroup outputGroup))
				audioSource.outputAudioMixerGroup = outputGroup;
			else
				Debug.LogError($"The audio mixer group ''{outputGroupName}'' could not be found in the mixer!");
		}

		/// <summary>
		/// 
		/// </summary>
		public AudioSource CreateAudioSource(GameObject objectToAddTo, bool is2D = false, float startVolume = 1f, float minDistance = 1f, string outputGroup = "Master")
		{
			AudioSource audioSource = objectToAddTo.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.volume = startVolume;
			audioSource.spatialBlend = is2D ? 0f : 1f;
			audioSource.minDistance = minDistance;

			SetAudioSourceOutput(audioSource, outputGroup);

			return audioSource;
		}

		/// <summary>
		/// 
		/// </summary>
		public AudioSource CreateAudioSource(string name, Transform parent, Vector3 localPosition, bool is2D = false, float startVolume = 1f, float minDistance = 1f, string outputGroup = "Master") 
		{
			GameObject audioObject = new GameObject(name, typeof(AudioSource));
			
			audioObject.transform.parent = parent;
			audioObject.transform.localPosition = localPosition;
			AudioSource audioSource = audioObject.GetComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.volume = startVolume;
			audioSource.spatialBlend = is2D ? 0f : 1f;
			audioSource.minDistance = minDistance;

			SetAudioSourceOutput(audioSource, outputGroup);

			return audioSource;
		}

		/// <summary>
		/// 
		/// </summary>
		public void LerpVolumeOverTime(AudioSource audioSource, float targetVolume, float speed) 
		{
			if (m_LevelSetters.ContainsKey(audioSource)) 
			{
				if (m_LevelSetters[audioSource] != null)
					StopCoroutine(m_LevelSetters[audioSource]);
				
				m_LevelSetters[audioSource] = StartCoroutine(C_LerpVolumeOverTime(audioSource, targetVolume, speed));
			} 
			else 
				m_LevelSetters.Add(audioSource, StartCoroutine(C_LerpVolumeOverTime(audioSource, targetVolume, speed)));
		}

		private void GenerateAudioMixerGroupsDictionary() 
		{
			m_AudioMixerGroups = new Dictionary<string, AudioMixerGroup>();
			AudioMixerGroup[] audioMixerGroups = m_AudioMixer.FindMatchingGroups("");

			foreach (var group in audioMixerGroups)
				m_AudioMixerGroups.Add(group.name, group);
		}

		/// <summary>
		/// 
		/// </summary>
		private IEnumerator C_LerpVolumeOverTime(AudioSource audioSource, float volume, float speed) 
		{
			while(audioSource != null && Mathf.Abs(audioSource.volume - volume) > 0.01f) 
			{
				audioSource.volume = Mathf.MoveTowards(audioSource.volume, volume, Time.deltaTime * speed);
				yield return null;
			}

			if(audioSource.volume == 0f)
				audioSource.Stop();
			
			m_LevelSetters.Remove(audioSource);
		}
	}
}