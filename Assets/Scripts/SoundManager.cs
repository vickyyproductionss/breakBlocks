using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public AudioSource whoosh;
	public AudioSource impact;
	public AudioSource slide;
	public AudioClip Btn_click;

	public static SoundManager instance;
	private void Awake()
	{
		instance = this;
	}
	public void Audiosource_Play(AudioSource source)
	{
		//Add a condition to check if we can play this
		if(PlayerPrefs.GetInt("Sound") == 1)
		{
			source.Play();
		}
	}
	public void BtnClickSoundPlay()
	{
		playThisSound(Btn_click);
	}
	public void playThisSound(AudioClip clip)
	{
		if (PlayerPrefs.GetInt("Sound") == 1)
		{
			GameObject audioSource = new GameObject();
			audioSource.transform.SetParent(transform);
			AudioSource source = audioSource.AddComponent<AudioSource>();
			source.clip = clip;
			source.Play();
			Destroy(audioSource, 2);
		}
	}
}