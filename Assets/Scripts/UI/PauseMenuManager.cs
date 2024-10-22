using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button titleButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject pauseMenuPrefab;

    private TouchManager touchManager;
    
    private bool isPaused = false;
    

    private void Start()
    {
        Init();
        touchManager = FindAnyObjectByType<TouchManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                PauseMenuEnable();
            }
            else
            {
                PauseMenuDisable();
            }
        }
    }

    private void Init()
    {
        pauseMenuPrefab.SetActive(false);
        continueButton.onClick.AddListener(OnContinueButtonClicked);
        titleButton.onClick.AddListener(OnTitleButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void PauseMenuEnable()
    {
        Time.timeScale = 0f;
        pauseMenuPrefab.SetActive(true);
        isPaused = true;

        // 모든 움직이는 오브젝트에 대해 일시정지 호출
        var movableObjects = GetMovableObjects();
        foreach (var movable in movableObjects)
        {
            movable.SetPaused(true);
        }

        // 스폰 매니저에서 스폰을 멈춤
        var spawnManager = FindAnyObjectByType<SpawnManager>();
        if (spawnManager != null)
        {
            spawnManager.StopSpawning();
        }

        // TouchManager 일시정지
        if (touchManager != null)
        {
            touchManager.SetPaused(true);
        }
    }

    private void PauseMenuDisable()
    {
        Time.timeScale = 1f;
        pauseMenuPrefab.SetActive(false);
        isPaused = false;

        // 모든 움직이는 오브젝트에 대해 일시정지 해제 호출
        var movableObjects = GetMovableObjects();
        foreach (var movable in movableObjects)
        {
            movable.SetPaused(false);
        }

        // 스폰 매니저에서 스폰을 재개
        var spawnManager = FindAnyObjectByType<SpawnManager>();
        if (spawnManager != null)
        {
            spawnManager.StartSpawning();
        }

        // TouchManager 일시정지
        if (touchManager != null)
        {
            touchManager.SetPaused(false);
        }
    }


    // IMovable 인터페이스를 구현한 MonoBehaviour 컴포넌트들을 찾음
    private List<IMovable> GetMovableObjects()
    {
        List<IMovable> movables = new List<IMovable>();
        MonoBehaviour[] allBehaviours = FindObjectsOfType<MonoBehaviour>();

        foreach (var behaviour in allBehaviours)
        {
            if (behaviour is IMovable movable)
            {
                movables.Add(movable);
            }
        }

        return movables;
    }

    private void OnContinueButtonClicked()
    {
        PauseMenuDisable();
    }

    private void OnTitleButtonClicked()
    {
        PauseMenuDisable();
        SceneManager.LoadScene("Title");
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
        continueButton.onClick.RemoveListener(OnContinueButtonClicked);
        titleButton.onClick.RemoveListener(OnTitleButtonClicked);
        exitButton.onClick.RemoveListener(OnExitButtonClicked);
    }
}
