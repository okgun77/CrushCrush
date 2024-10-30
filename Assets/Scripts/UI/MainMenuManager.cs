using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject mainMenuPrefab;

    private bool isPaused = false;

    private void Start()
    {
        Init();
    }

    
    private void Init()
    {
        // pauseMenuPrefab.SetActive(false);
        startButton.onClick.AddListener(OnStartButtonClicked);
        optionButton.onClick.AddListener(OnOptionButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene("Stage01_Aurora");
    }

    private void OnOptionButtonClicked()
    {

    }

    private void OnExitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(OnStartButtonClicked);
        optionButton.onClick.RemoveListener(OnOptionButtonClicked);
        exitButton.onClick.RemoveListener(OnExitButtonClicked);
    }


}
