using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicesCapturer : MonoBehaviour
{
	public string LevelString = "";
	private void Start()
	{
		for(int i = 0; i < gameObject.transform.childCount;i++)
		{
			if(gameObject.transform.GetChild(i).gameObject.activeInHierarchy)
			{
				if(string.IsNullOrEmpty(LevelString))
				{
					LevelString += i.ToString();
				}
				else
				{
					LevelString += ("-" + i.ToString());
				}
			}
		}
	}
}
