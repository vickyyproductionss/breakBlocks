using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
	[SerializeField] TMP_Text _goldsVal;
	[SerializeField] TMP_Text _gemsVal;
	public delegate void InventoryUpdateDelegate(int gems, int gold);
	public static ResourcesManager Instance;
	private void Awake()
	{
		Instance = this;
	}
	private int gems;
	private int gold;

	public event InventoryUpdateDelegate OnInventoryUpdate;

	public int Gems
	{
		get { return gems; }
		set
		{
			gems = value;
			_gemsVal.text = ConvertNumber(gems);
			OnInventoryUpdate?.Invoke(gems, gold);
		}
	}

	public int Gold
	{
		get { return gold; }
		set
		{
			gold = value;
			_goldsVal.text = ConvertNumber(gold);
			OnInventoryUpdate?.Invoke(gems, gold);
		}
	}
	public string ConvertNumber(float number)
	{
		string[] suffixes = { "", "K", "M", "B", "T" };
		int suffixIndex = 0;

		while (number >= 1000 && suffixIndex < suffixes.Length - 1)
		{
			number /= 1000;
			suffixIndex++;
		}

		string formattedNumber = number.ToString("0.##") + suffixes[suffixIndex];
		return formattedNumber;
	}
}
