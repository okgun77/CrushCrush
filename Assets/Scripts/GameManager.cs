using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        // VSync 끄기
        QualitySettings.vSyncCount = 0;

        // 타겟 프레임레이트 설정
        Application.targetFrameRate = 120;
    }

    public void RestartGame()
    {
        // 현재 씬의 이름을 가져와서 씬을 다시 로드합니다.
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void ExitGame()
    {
        // 게임 종료
        Application.Quit();
        Debug.Log("Game is exiting");   // 확인
    }
}
