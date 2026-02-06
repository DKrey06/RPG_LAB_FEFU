using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private float fadeInDuration = 1.5f;

    [Header("Settings")]
    [SerializeField] private string gameOverMessage = "ВЫ ПРОИГРАЛИ";
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private Color panelColor = new Color(0.1f, 0.1f, 0.1f, 0.95f);

    public static GameOverManager Instance { get; private set; }

    private CanvasGroup panelCanvasGroup;
    private PlayerStats playerStats;
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeUI();
    }

    void InitializeUI()
    {
        if (gameOverPanel != null)
        {
            panelCanvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
            {
                panelCanvasGroup = gameOverPanel.AddComponent<CanvasGroup>();
            }
            panelCanvasGroup.alpha = 0f;

            Image panelImage = gameOverPanel.GetComponent<Image>();
            if (panelImage != null)
            {
                panelImage.color = panelColor;
            }
        }

        if (gameOverText != null)
        {
            gameOverText.text = gameOverMessage;
            gameOverText.gameObject.SetActive(false);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
            mainMenuButton.gameObject.SetActive(false);
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetGameOver();

        FindPlayer();

        Debug.Log($"GameOverManager: загружена сцена {scene.name}");
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                Debug.Log("GameOverManager нашел игрока в сцене");
            }
        }
        else
        {
            Debug.LogWarning("Игрок не найден в сцене");
        }
    }

    void Update()
    {
        if (!isGameOver && playerStats != null && playerStats.CurrentHP <= 0)
        {
            ShowGameOverScreen();
        }
    }

    void ShowGameOverScreen()
    {
        if (isGameOver || gameOverPanel == null) return;

        isGameOver = true;
        gameOverPanel.SetActive(true);

        if (gameOverText != null) gameOverText.gameObject.SetActive(true);
        if (mainMenuButton != null) mainMenuButton.gameObject.SetActive(true);

        StartCoroutine(FadeInGameOverScreen());

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("Game Over экран показан");
    }

    IEnumerator FadeInGameOverScreen()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);

            if (panelCanvasGroup != null)
            {
                panelCanvasGroup.alpha = alpha;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 1f;
        }
    }

    void GoToMainMenu()
    {
        ResetGameOver();

        DestroyPlayerObject();

        SceneManager.LoadScene(mainMenuSceneName);
        Destroy(gameObject);
    }

    void DestroyPlayerObject()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Destroy(player);
            Debug.Log("Игрок уничтожен при переходе в меню");
        }
    }

    void ResetGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        isGameOver = false;

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
        }

        if (gameOverText != null) gameOverText.gameObject.SetActive(false);
        if (mainMenuButton != null) mainMenuButton.gameObject.SetActive(false);

        Debug.Log("Game Over сброшен");
    }

    public void ForceGameOver()
    {
        ShowGameOverScreen();
    }
}