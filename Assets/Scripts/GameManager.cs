using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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
