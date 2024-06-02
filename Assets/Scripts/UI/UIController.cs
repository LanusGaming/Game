using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public Canvas uiCanvas;
    public Canvas menuCanvas;
    public GameObject escapeMenu;
    public GameObject confirmDialogObject;

    public static UIController instance;

    private Player player;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        player = Player.instance;
    }

    private void Update()
    {
        if (Settings.Controls.ExitPressed)
        {
            if (GameController.instance != null && !GameController.instance.minimap.inMinimapMode)
                return;

            escapeMenu.SetActive(!escapeMenu.activeSelf);
            ChangePauseMode(escapeMenu.activeSelf);
        }
    }

    public GameObject DisplayDialog(GameObject interactDialog, Vector2 position)
    {
        GameObject interactDialogObject = Instantiate(interactDialog, uiCanvas.transform);
        interactDialogObject.transform.localPosition = Camera.main.WorldToScreenPoint(position);

        return interactDialogObject;
    }

    public void OnResumeButtonClick()
    {
        escapeMenu.SetActive(false);
        ChangePauseMode(false);
    }

    public void OnSettingsButtonClick()
    {
        escapeMenu.SetActive(false);
        ChangePauseMode(false);
    }

    public void OnEndRunButtonClick()
    {
        ConfirmDialog dialog = Instantiate(confirmDialogObject, menuCanvas.transform).GetComponent<ConfirmDialog>();
        dialog.text = "Do you really want to end your run early? (Your progress will not be saved!)";
        dialog.confirm = (d) => EndLevel(HUB.buildIndex);
    }

    public void OnMainMenuButtonClick()
    {
        escapeMenu.SetActive(false);
        ChangePauseMode(false);
    }

    public void OnDesktopButtonClick()
    {
        escapeMenu.SetActive(false);
        ChangePauseMode(false);
    }

    private void ChangePauseMode(bool paused)
    {
        player.active = !paused;
        Time.timeScale = (paused) ? 0 : 1;
    }

    private void EndLevel(int nextScene)
    {
        player.active = false;
        player.invincible = true;

        escapeMenu.SetActive(false);
        ChangePauseMode(false);

        Transition transition = Instantiate(GameController.instance.transitionObject).GetComponent<Transition>();
        transition.transform.position = player.transform.position;
        transition.reversed = true;

        transition.doneCallback = (transition) => StartCoroutine(LoadScene(nextScene));
    }

    private IEnumerator LoadScene(int buildIndex)
    {
        yield return new WaitForSeconds(GameController.instance.loadNextLevelDelay);
        SceneManager.LoadSceneAsync(buildIndex);
    }
}
