using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour
{
	[SerializeField] GameObject TuitWindow;
	[SerializeField] GameObject Highlighter;
	[SerializeField] GameObject[] Buttons;
	[SerializeField] TMP_Text Head;
	[SerializeField] TMP_Text Description;
	[SerializeField] Button NextButton;
	bool HowToPlayDone;
    void Start()
    {
        if(!PlayerPrefs.HasKey("HowToPlay"))
		{
			Invoke(nameof(StartTuition), 3);
		}
    }
	bool tuiting = false;
	void StartTuition()
	{
		TeachHowToPlay(0);
	}
	void TeachHowToPlay(int index)
	{
		tuiting = true;
		TuitWindow.SetActive(true);
		switch (index)
		{
			case 0:
				Head.text = "Speed";
				Description.text = "2x each balls speed.\n\n<color=green>Free</color>";
				Highlighter.GetComponent<RectTransform>().position = Buttons[index].GetComponent<RectTransform>().position;
				NextButton.onClick.RemoveAllListeners();
				NextButton.onClick.AddListener(() => TeachHowToPlay(1));
				break;
			case 1:
				Head.text = "All down";
				Description.text = "pulls down all balls instantly.\n\n<color=green>Free</color>";
				Highlighter.GetComponent<RectTransform>().position = Buttons[index].GetComponent<RectTransform>().position;
				NextButton.onClick.RemoveAllListeners();
				NextButton.onClick.AddListener(() => TeachHowToPlay(2));
				break;
			case 2:
				Head.text = "All directions";
				Description.text = "Creates super block.must try this\n\n<color=green>100 gems</color>";
				Highlighter.GetComponent<RectTransform>().position = Buttons[index].GetComponent<RectTransform>().position;
				NextButton.onClick.RemoveAllListeners();
				NextButton.onClick.AddListener(() => TeachHowToPlay(3));
				break;
			case 3:
				Head.text = "Swipe rows";
				Description.text = "Busts bottom three rows\n\n<color=green>100 gems</color>";
				Highlighter.GetComponent<RectTransform>().position = Buttons[index].GetComponent<RectTransform>().position;
				NextButton.onClick.RemoveAllListeners();
				NextButton.onClick.AddListener(() => TeachHowToPlay(4));
				break;
			case 4:
				Head.text = "Add balls";
				Description.text = "Adds 10 extra balls.\n\n<color=green>100 gems</color>";
				Highlighter.GetComponent<RectTransform>().position = Buttons[index].GetComponent<RectTransform>().position;
				NextButton.onClick.RemoveAllListeners();
				NextButton.onClick.AddListener(() => TeachHowToPlay(5));
				break;
			case 5:
				Head.text = "Add time";
				Description.text = "Adds extra 10 minutes.\n\n<color=green>100 gems</color>";
				Highlighter.GetComponent<RectTransform>().position = Buttons[index].GetComponent<RectTransform>().position;
				NextButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "FINSIH";
				PlayerPrefs.SetString("HowToPlay", "Done");
				NextButton.onClick.RemoveAllListeners();
				NextButton.onClick.AddListener(() => StartCoroutine(OnTutorialFinished()));
				break;
		}
	}
	IEnumerator OnTutorialFinished()
	{
		tuiting = false;
		yield return new WaitForSeconds(0.1f);
		TuitWindow.SetActive(false);
	}

	private void Update()
	{
		if(tuiting)
		{
			Buttons[0].transform.parent.gameObject.SetActive(true);
			Buttons[0].SetActive(true);
			Buttons[1].SetActive(true);
		}
	}

	#region SaveData
	//private void SaveObject(Object object)
	//{
	//	// Convert the object to JSON string
	//	string jsonString = JsonUtility.ToJson(myObject);

	//	// Save the JSON string to PlayerPrefs
	//	PlayerPrefs.SetString("myObject", jsonString);

	//	// Save PlayerPrefs to disk (optional)
	//	PlayerPrefs.Save();

	//	Debug.Log("Object saved!");
	//}

	//private void LoadObject()
	//{
	//	// Retrieve the JSON string from PlayerPrefs
	//	string jsonString = PlayerPrefs.GetString("myObject");

	//	// Convert the JSON string back to an object
	//	myObject = JsonUtility.FromJson<MyClass>(jsonString);

	//	Debug.Log("Object loaded!");
	//	Debug.Log("myInt: " + myObject.myInt);
	//	Debug.Log("myFloat: " + myObject.myFloat);
	//	Debug.Log("myString: " + myObject.myString);
	//}
	#endregion
}
