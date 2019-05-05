using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	[System.Serializable]
	public class SoundEffect
	{
		public string name;
		public AudioClip[] clips;
	}

	static AudioManager instance;

	[SerializeField]
	SoundEffect[]		m_Effects;

    // Start is called before the first frame update
    void Start()
    {
        if (instance && instance != this)
			Debug.LogError("Multiple AudioManager instances found.");
		else
			instance = this;
    }

    public static void PlaySoundEffect (string name, Vector3 position)
	{
		instance.mPlaySoundEffect(name, position);
	}

	void mPlaySoundEffect (string name, Vector3 position, float volume = 1f)
	{
		foreach (var effect in m_Effects)
		{
			if (effect.name == name)
			{
				AudioSource.PlayClipAtPoint(effect.clips[Random.Range(0, effect.clips.Length)], position, volume);
				return;
			}
		}
	}
}
