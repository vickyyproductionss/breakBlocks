using System.Threading.Tasks;
using UnityEngine;
using Firebase.Database;

public class Server : MonoBehaviour
{
	public static Server Instance { get; private set; }
	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
	public async Task<int> CheckMovesValueExists(int level)
	{
		Debug.Log("Checkingg");
		DatabaseReference movesRef = FirebaseDatabase.DefaultInstance.GetReference("Moves").Child(level.ToString());

		DataSnapshot snapshot = await movesRef.GetValueAsync();

		if (snapshot.Exists)
		{
			return int.Parse(snapshot.Value.ToString());
		}
		else
		{
			return -1;
		}
	}

	public async Task UpdateOrCreateMovesValue(int level, int moves)
	{
		Debug.Log("Checkingg");
		DatabaseReference movesRef = FirebaseDatabase.DefaultInstance.GetReference("Moves").Child(level.ToString());

		DataSnapshot snapshot = await movesRef.GetValueAsync();

		if (snapshot.Exists)
		{
			int existingMoves = int.Parse(snapshot.Value.ToString());

			if (moves < existingMoves)
			{
				await movesRef.SetValueAsync(moves);
			}
		}
		else
		{
			await movesRef.SetValueAsync(moves);
		}
	}
}
