using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using UnityEngine.UI;

public class PowerupUseController : MonoBehaviour
{
	public PowerupType indexOfPowerup;//0-AllDirection,1-BottomThreeLines,2-Add10Balls,3-AddTime
	[SerializeField] TMP_Text Heading;
	[SerializeField] TMP_Text Description;
	[SerializeField] GameObject AllDirectionsPrefab;
	[SerializeField] Button AllDirectionButton, BottomClearButton, AddBallsButton; 

	public void init(int index)
	{
		GameManager.instance.PauseTimer();
		if(index == 0)
		{
			SetDetails(PowerupType.AllDirection);
		}
		else if(index == 1)
		{
			SetDetails(PowerupType.BottomThree);
		}
		else if(index == 2)
		{
			SetDetails(PowerupType.Add10Balls);
		}
		else if( index == 3)
		{
			SetDetails(PowerupType.AddTime);
		}
	}
	public void SetDetails(PowerupType type)
	{
		indexOfPowerup = type;
		switch (type)
		{
			case PowerupType.AllDirection:
				Heading.text = "Hit all";
				Description.text = "Creates special block,\n on hitting that will impact all others.";
				break;
			case PowerupType.BottomThree:
				Heading.text = "Bottom three";
				Description.text = "Clears ,\n bottom three rows of blocks.";
				break;
			case PowerupType.Add10Balls:
				Heading.text = "Add balls";
				Description.text = "You will get,\n 10 extra balls for this match.";
				break;
			case PowerupType.AddTime:
				Heading.text = "Add time";
				Description.text = "You will get ,\n10 extra minutes for this match.";
				break;
		}
	}
	public enum PowerupType
	{
		AllDirection,BottomThree,Add10Balls,AddTime
	}
	public void OnClick_WatchAd()
	{
		//Show ad to the user and use powerup
		GrantReward();
	}
	public void OnClick_UseGems()
	{
		//Substract 100 gems if available and use powerup
		if(PlayerDataManager.Instance.Gems > 100)
		{
			ExecuteCloudScript("ReduceVirtualCurrency", 100, "GE");
		}
		else
		{
			GameManager.instance.ShowMessage("Not enough gems");
		}
	}
	public void ExecuteCloudScript(string functionName, int _amount, string _code)
	{
		var request = new ExecuteCloudScriptRequest
		{
			FunctionName = functionName,
			GeneratePlayStreamEvent = true,
			FunctionParameter = new
			{
				StatName = _code,
				Amount = _amount
			}
		};

		PlayFabClientAPI.ExecuteCloudScript(request, OnCloudScriptExecuted, OnCloudScriptError);
	}

	private void OnCloudScriptExecuted(ExecuteCloudScriptResult result)
	{
		// Handle the cloud script execution result here
		Debug.Log("Cloud script executed successfully!");
		Debug.Log(result.FunctionResult.ToString() + " Got this");
		if(result.FunctionResult.ToString() == "200")
		{
			PlayerDataManager.Instance.Gems -= 100;
			GrantReward();
		}
		else
		{
			GameManager.instance.ShowMessage("Error occured \n Check your connection!!!",true);
		}
	}

	private void OnCloudScriptError(PlayFabError error)
	{
		GameManager.instance.ShowMessage("Error occured \n Check your connection!!!", true);
		// Handle any errors that occur during cloud script execution
		Debug.LogError("Cloud script execution error: " + error.GenerateErrorReport());
	}
	public void GrantReward()
	{
		GameManager.instance.ResumeTimer();//Resume timer
		this.gameObject.GetComponent<OpeningAnimation>().ChangeHeightClose();
		//Write code to use powerups
		switch (indexOfPowerup)
		{
			case PowerupType.AllDirection:
				AllDirectionPowerupUse();
				AllDirectionButton.interactable = false;
				break;
			case PowerupType.BottomThree:
				BottomThreeRowsClear();
				BottomClearButton.interactable = false;
				break;
			case PowerupType.Add10Balls:
				Add10ExtraBalls();
				AddBallsButton.interactable = false;
				break;
			case PowerupType.AddTime:
				Add10ExtraMinutes();
				break;
		}
	}
	public void AllDirectionPowerupUse()
	{
		float screenWidth = Screen.width;
		float screenHeight = Screen.height;
		float ratio = (float)screenHeight / (float)screenWidth;
		float downUnit = (1.777777778f * 0.55f) / ratio;
		Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3((4 * 0.1f) + 0.05f, 0.2f, Camera.main.nearClipPlane));
		for(int i = 0; i < GameManager.instance.BlockParent.transform.childCount; i++)
		{
			Vector3 blockPos = GameManager.instance.BlockParent.transform.GetChild(i).transform.position;
			if (blockPos.x == pos.x && blockPos.y == pos.y)
			{
				Destroy(GameManager.instance.BlockParent.transform.GetChild(i).gameObject);
			}
		}
		GameObject go = Instantiate(AllDirectionsPrefab, pos, Quaternion.identity);
		go.tag = "AllDirections";
		go.transform.SetParent(GameManager.instance.f3_gameobjectParent.transform);
		GameManager.instance.AllDirectionsBlock = go;
	}


	public GameObject BlockBustingFX;
	public AudioClip wallHit;
	public void BottomThreeRowsClear()
	{
		int row = 1;
		float YPos = 0;
		List<GameObject> list = new List<GameObject>();
		for(int i =0; i < GameManager.instance.BlockParent.transform.childCount; i++)
		{
			if (i == 0)
			{
				YPos = GameManager.instance.BlockParent.transform.GetChild(i).transform.position.y;
				list.Add(GameManager.instance.BlockParent.transform.GetChild(i).gameObject);
			}
			else if(GameManager.instance.BlockParent.transform.GetChild(i).transform.position.y == YPos)
			{
				list.Add(GameManager.instance.BlockParent.transform.GetChild(i).gameObject);
			}
			else if (GameManager.instance.BlockParent.transform.GetChild(i).transform.position.y != YPos && row < 3)
			{
				YPos = GameManager.instance.BlockParent.transform.GetChild(i).transform.position.y;
				list.Add(GameManager.instance.BlockParent.transform.GetChild(i).gameObject);
				row++;
			}
		}
		StartCoroutine(BurstThreeRows(list));
	}
	IEnumerator BurstThreeRows(List<GameObject> list)
	{
		yield return new WaitForFixedUpdate();
		GameObject block = list[list.Count - 1];
		GameObject FX = Instantiate(BlockBustingFX, block.transform);
		block.layer = 3;
		block.GetComponent<SpriteRenderer>().color = GameManager.instance._Colors[PlayerPrefs.GetInt("colorBlock")];
		GameObject sfx2 = new GameObject();
		sfx2.transform.SetParent(GameManager.instance.SoundParent.transform);
		sfx2.AddComponent<AudioSource>();
		AudioSource temp2 = sfx2.GetComponent<AudioSource>();
		temp2.clip = wallHit;
		StartCoroutine(latePlay(temp2));
		Destroy(sfx2, 1);
		FX.transform.parent = GameManager.instance.FX.transform;
		Destroy(block, 0.3f);
		Destroy(FX, 4);
		PlayerGameData.Instance.BlocksBusted++;
		list.Remove(block);
		if(list.Count != 0)
		{
			StartCoroutine(BurstThreeRows(list));
		}
	}
	IEnumerator latePlay(AudioSource aSource)
	{
		yield return new WaitForSeconds(0.2f);
		aSource.Play();
	}

	public void Add10ExtraBalls()
	{
		GameManager.instance.Give10ExtraBalls();
	}
	public void Add10ExtraMinutes()
	{
		GameManager.instance.timeLeft += 600;
	}
}
