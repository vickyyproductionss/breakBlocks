using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseButtonsManager : MonoBehaviour
{
    [SerializeField] GameObject HomeScreen, LeaderboardScreen, SettingsScreen, AboutScreen;
    public enum Screens
    {
        Home,
        Leaderboard,
        Settings,
        About
    }
    public void SetHomeScreen()
    {
        SwitchScreens(Screens.Home);
    }
    public void SetLeaderboardScreen()
    {
        SwitchScreens(Screens.Leaderboard);
    }
    public void SetSettingsScreen()
    {
        SwitchScreens(Screens.Settings);
    }
    public void SetContactUsScreen()
    {
        SwitchScreens(Screens.About);
    }
    void SwitchScreens(Screens _screen)
    {
        switch (_screen)
        {
            case Screens.Home:
                HomeScreen.SetActive(true);
                LeaderboardScreen.SetActive(false);
                SettingsScreen.SetActive(false);
                AboutScreen.SetActive(false);
                break;
            case Screens.Leaderboard:
                HomeScreen.SetActive(false);
                LeaderboardScreen.SetActive(true);
                SettingsScreen.SetActive(false);
                AboutScreen.SetActive(false);
                break;
            case Screens.Settings:
                HomeScreen.SetActive(false);
                LeaderboardScreen.SetActive(false);
                SettingsScreen.SetActive(true);
                AboutScreen.SetActive(false);
                break;
            case Screens.About:
                HomeScreen.SetActive(false);
                LeaderboardScreen.SetActive(false);
                SettingsScreen.SetActive(false);
                AboutScreen.SetActive(true);
                break;
        }
    }
}
