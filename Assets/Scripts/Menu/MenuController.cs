using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public string hubSceneName = "HUB";
    public GameObject continueButton;
    public GameObject transitionObject;
    public AudioMixer mixer;

    private Menu activeMenu;

    private void Awake()
    {
        Settings.Mixer = mixer;
        Settings.SettingsManager.LoadSettings();
        SaveManager.LoadSaves();

        if (SaveManager.activeSave != null)
            continueButton.SetActive(true);
    }

    private void Start()
    {
        Settings.Apply();
    }

    private void OnApplicationQuit()
    {
        Settings.SettingsManager.SaveSettings();
        SaveManager.SaveAll();
    }

    public void StartGame()
    {
        GameData.Set(SaveManager.activeSave.gameData);

        Transition transition = Instantiate(transitionObject).GetComponent<Transition>();
        transition.reversed = true;
        transition.doneCallback = (transition) => SceneManager.LoadSceneAsync(hubSceneName);
    }

    public void HideActiveMenu()
    {
        if (activeMenu != null)
            activeMenu.Hide();
    }

    public void ShowMenu(Menu menu)
    {
        HideActiveMenu();
        menu.Show();
        activeMenu = menu;
    }

    public void ContinueWithActiveSave()
    {
        StartGame();
    }

    public void QuitApplication()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
    }
}