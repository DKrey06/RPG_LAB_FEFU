using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string _gameSceneName = "Scene1";

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        CleanUpOldPlayers();
    }

    void CleanUpOldPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Destroy(player);
            Debug.Log("Удален старый игрок в меню");
        }
    }

    public void StartGame()
    {
        CleanUpOldPlayers();

        SceneManager.LoadScene(_gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}