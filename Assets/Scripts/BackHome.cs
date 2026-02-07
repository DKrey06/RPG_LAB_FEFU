using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToMenu : MonoBehaviour
{
    [SerializeField] private string menuScene = "MainMenu";

    void Start()
    {
        // Автоматически настраиваем кнопку
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(GoToMenu);
        }
    }

    public void GoToMenu()
    {
        // Сбрасываем время на случай паузы
        Time.timeScale = 1f;

        // Загружаем главное меню
        SceneManager.LoadScene(menuScene);

        // Очищаем все DontDestroyOnLoad объекты игрока
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player.scene.buildIndex == -1)
        {
            Destroy(player);
        }
    }
}