using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public string hubSceneName = "HUB";

    public GameObject continueButton;

    public NewSaveMenu newSaveMenu;
    public LoadSaveMenu loadSaveMenu;
    public GameObject settingsMenu;

    public GameObject transitionObject;

    private bool inMenu;

    private void Awake()
    {
        SettingsManager.LoadSettings();
        SaveManager.LoadSaves();

        if (SaveManager.activeSave != null)
            continueButton.SetActive(true);
    }

    private void OnApplicationQuit()
    {
        SettingsManager.SaveSettings();
        SaveManager.SaveAll();
    }

    public void StartGame()
    {
        GameData.Set(SaveManager.activeSave.gameData);

        Transition transition = Instantiate(transitionObject).GetComponent<Transition>();
        transition.reversed = true;
        transition.doneCallback = (transition) => SceneManager.LoadSceneAsync(hubSceneName);
    }

    public void OnContinue()
    {
        StartGame();
    }

    public void OnNewSave()
    {
        if (inMenu)
        {
            loadSaveMenu.Hide();
            settingsMenu.SetActive(false);
        }

        newSaveMenu.Show();
        inMenu = true;
    }

    public void OnLoadSave()
    {
        if (inMenu)
        {
            newSaveMenu.Hide();
            settingsMenu.SetActive(false);
        }

        loadSaveMenu.Show();
        inMenu = true;
    }

    public void OnSettings()
    {
        if (inMenu)
        {
            newSaveMenu.Hide();
            loadSaveMenu.Hide();
        }

        settingsMenu.SetActive(true);
        inMenu = true;
    }

    public void OnQuit()
    {
        Application.Quit();
        EditorApplication.ExitPlaymode();
    }
}