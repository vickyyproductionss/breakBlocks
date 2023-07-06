using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGameData : MonoBehaviour
{
	//Data for the game that will be accessed throught the match
	[HideInInspector]public int GoldsThisMatch = 0;
	[HideInInspector]public int GemsThisMatch = 0;
	[HideInInspector]public int BlocksBusted = 0;
	[HideInInspector]public int ScoreThisMatch = 0;
	public bool IsGameOver;
	public static PlayerGameData Instance;
	private void Awake()
	{
		Instance = this;
	}
}
