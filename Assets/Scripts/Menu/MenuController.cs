using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Game;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance { get; private set; }

    public string hubSceneName = "HUB";
    public GameObject continueButton;
    public GameObject transitionObject;
    public AudioMixer mixer;

    private Menu activeMenu;

    private void Awake()
    {
        Instance = this;

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
        SaveManager.Apply();

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