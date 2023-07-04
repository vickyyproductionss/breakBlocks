using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject ON_PN,OFF_PN,ON_MUSIC,OFF_MUSIC,ON_SOUND,OFF_SOUND,ON_VIBRATE,OFF_VIBRATE;
    [SerializeField] private GameObject ONICO_PN, OFFICO_PN, ONICO_MUSIC, OFFICO_MUSIC, ONICO_SOUND,OFFICO_SOUND;
    [SerializeField] private TMP_Text Info;
    private void Awake()
    {
        SetUI();   
    }
    void SetUI()
    {
        PushNotification(PlayerPrefs.GetInt("PNs") == 1);
        Music(PlayerPrefs.GetInt("Music") == 1);
        Sound(PlayerPrefs.GetInt("Sound") == 1);
        Vibration(PlayerPrefs.GetInt("Vibration") == 1);
        Info.text = $"Username: {PlayerPrefs.GetString("displayName")} \n User ID: {PlayerPrefs.GetString("PFID")}";
    }
    public void PushNotification(bool enabled)
    {
        if(enabled)
        {
            ON_PN.SetActive(true);
            ONICO_PN.SetActive(true);
            OFFICO_PN.SetActive(false);
            OFF_PN.SetActive(false);
            PlayerPrefs.SetInt("PNs", 1);
        }
        else
        {
            ON_PN.SetActive(false);
            ONICO_PN.SetActive(false);
            OFFICO_PN.SetActive(true);
            OFF_PN.SetActive(true);
            PlayerPrefs.SetInt("PNs", 0);
        }
    }
    public void Music(bool enabled)
    {
        if (enabled)
        {
            ON_MUSIC.SetActive(true);
            ONICO_MUSIC.SetActive(true);
            OFFICO_MUSIC.SetActive(false);
            OFF_MUSIC.SetActive(false);
            PlayerPrefs.SetInt("Music", 1);
        }
        else
        {
            ON_MUSIC.SetActive(false);
            ONICO_MUSIC.SetActive(false);
            OFFICO_MUSIC.SetActive(true);
            OFF_MUSIC.SetActive(true);
            PlayerPrefs.SetInt("Music", 0);
        }
    }
    public void Sound(bool enabled)
    {
        if (enabled)
        {
            ON_SOUND.SetActive(true);
            ONICO_SOUND.SetActive(true);
            OFFICO_SOUND.SetActive(false);
            OFF_SOUND.SetActive(false);
            PlayerPrefs.SetInt("Sound", 1);
        }
        else
        {
            ON_SOUND.SetActive(false);
            ONICO_SOUND.SetActive(false);
            OFFICO_SOUND.SetActive(true);
            OFF_SOUND.SetActive(true);
            PlayerPrefs.SetInt("Sound", 0);
        }
    }
    public void Vibration(bool enabled)
    {
        if (enabled)
        {
            ON_VIBRATE.SetActive(true);
            OFF_VIBRATE.SetActive(false);
            PlayerPrefs.SetInt("Vibration", 1);
        }
        else
        {
            ON_VIBRATE.SetActive(false);
            OFF_VIBRATE.SetActive(true);
            PlayerPrefs.SetInt("Vibration", 0);
        }
    }
}
