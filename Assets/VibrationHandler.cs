using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationHandler : MonoBehaviour
{
    public static VibrationHandler Instance;
	private void Awake()
	{
		Instance = this;
	}
	public void VibrateForMilliseconds(int ms)
    {
		if(PlayerPrefs.GetInt("Vibration") == 1)
		{
			Vibration.Vibrate(ms);
		}
    }
}
