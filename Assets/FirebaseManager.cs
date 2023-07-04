using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
	public static FirebaseManager instance;
	private FirebaseApp app;//For Firebase Messaging
	public FirebaseRemoteConfig remoteConfig;

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(this);
		}
	}
	void Start()
    {
		Invoke(nameof(InitialiseFirebase), 0.1f);
		Invoke(nameof(FetchDataAsync), 0.2f);
    }

	#region Remote-Config
	// Start a fetch request.
	// FetchAsync only fetches new data if the current data is older than the provided
	// timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
	// By default the timespan is 12 hours, and for production apps, this is a good
	// number. For this example though, it's set to a timespan of zero, so that
	// changes in the console will always show up immediately.
	public Task FetchDataAsync()
	{
		SetDefaults();
		System.Threading.Tasks.Task fetchTask =
		Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
			TimeSpan.Zero);
		return fetchTask.ContinueWithOnMainThread(FetchComplete);
	}
	void SetDefaults()
	{
		System.Collections.Generic.Dictionary<string, object> defaults =
		new System.Collections.Generic.Dictionary<string, object>();

		// These are the values that are used if we haven't fetched data from the
		// server
		// yet, or if we ask for values that the server doesn't have:
		defaults.Add("Message", "No messages");
		defaults.Add("BallsEase", 3);
		defaults.Add("Difficulty", 3);
		defaults.Add("InitialRows", 3);
		defaults.Add("TargetFactor", 2500);
		defaults.Add("TimeFactor", 120);
		defaults.Add("UnderMaintenance", false);
		Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
		  .ContinueWithOnMainThread(task => { });
	}
	private void FetchComplete(Task fetchTask)
	{
		if (!fetchTask.IsCompleted)
		{
			Debug.LogError("Retrieval hasn't finished.");
			return;
		}

		remoteConfig = FirebaseRemoteConfig.DefaultInstance;
		var info = remoteConfig.Info;
		if (info.LastFetchStatus != LastFetchStatus.Success)
		{
			Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
			return;
		}

		// Fetch successful. Parameter values must be activated to use.
		remoteConfig.ActivateAsync()
		.ContinueWithOnMainThread(
		task =>
		{
			//Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
		});
	}
	private void InitialiseRemoteConfig()
	{
		Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener += ConfigUpdateListenerEventHandler;
	}
	// Handle real-time Remote Config events.
	void ConfigUpdateListenerEventHandler(
	   object sender, Firebase.RemoteConfig.ConfigUpdateEventArgs args)
	{
		if (args.Error != Firebase.RemoteConfig.RemoteConfigError.None)
		{
			Debug.Log(String.Format("Error occurred while listening: {0}", args.Error));
			return;
		}

		Debug.Log("Updated keys: " + string.Join(", ", args.UpdatedKeys));
		// Activate all fetched values and then display a welcome message.
		remoteConfig.ActivateAsync().ContinueWithOnMainThread(
		  task => {
			  //Display welcome message
		  });
	}

	// Stop the listener.
	void OnDestroy()
	{
		Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener -= ConfigUpdateListenerEventHandler;
	}
	#endregion

	#region Firebase Messaging
	private void InitialiseFirebase()
	{
		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
			var dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available)
			{
				// Create and hold a reference to your FirebaseApp,
				// where app is a Firebase.FirebaseApp property of your application class.
				app = Firebase.FirebaseApp.DefaultInstance;
				Invoke(nameof(InitialiseRemoteConfig), 0.1f);
				// Set a flag here to indicate whether Firebase is ready to use by your app.
				OnFirebaseInitialised();
			}
			else
			{
				UnityEngine.Debug.LogError(System.String.Format(
				  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
				// Firebase Unity SDK is not safe to use here.
			}
		});
	}
	void OnFirebaseInitialised()
	{
		if(PlayerPrefs.GetInt("PNs") == 1)
		{
			Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
			Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
		}
	}
	public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
	{
		UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
	}

	public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
	{
		UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
	}
	#endregion

	#region Firebase Analytics
	/// <summary>
	/// Log an event with an int parameter.
	/// </summary>
	/// <param name="EventName"></param>
	/// <param name="EventParam"></param>
	/// <param name="value"></param>
	public void LogAnEvent(string EventName, string EventParam, int value)
    {
		// Log an event with an int parameter.
		Firebase.Analytics.FirebaseAnalytics
		  .LogEvent(
			EventName,
			EventParam,
			value
		  );
	}
	/// <summary>
	/// Log an event with a string parameter.
	/// </summary>
	/// <param name="EventName"></param>
	/// <param name="EventParam"></param>
	/// <param name="value"></param>
	public void LogAnEvent(string EventName, string EventParam, string value)
	{
		Firebase.Analytics.FirebaseAnalytics
		  .LogEvent(
			EventName,
			EventParam,
			value
		  );
	}
	/// <summary>
	/// Log an event with a float parameter.
	/// </summary>
	/// <param name="EventName"></param>
	/// <param name="EventParam"></param>
	/// <param name="value"></param>
	public void LogAnEvent(string EventName, string EventParam, float value)
	{
		Firebase.Analytics.FirebaseAnalytics
		  .LogEvent(
			EventName,
			EventParam,
			value
		  );
	}
	/// <summary>
	/// Log an event with a evnt name only.
	/// </summary>
	/// <param name="Eventname"></param>
	public void LogAnEvent(string Eventname)
	{
		Firebase.Analytics.FirebaseAnalytics.LogEvent(Eventname);
	}
	/// <summary>
	/// Sets users property in firebase analytics
	/// </summary>
	/// <param name="Key"></param>
	/// <param name="Value"></param>
	public void SetUserProperty(string Key, string Value)
	{
		Firebase.Analytics.FirebaseAnalytics.SetUserProperty(Key, Value);
	}
	#endregion
}