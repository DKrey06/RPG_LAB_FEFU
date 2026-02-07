using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameOverManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private float fadeInDuration = 1.5f;

    [Header("Settings")]
    [SerializeField] private string gameOverMessage = "ВЫ ПРОИГРАЛИ";
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private bool hideSpritesOnGameOver = true;

    public static GameOverManager Instance { get; private set; }

    private CanvasGroup panelCanvasGroup;
    private PlayerStats playerStats;
    private bool isGameOver = false;

    private List<SpriteRenderer> hiddenSprites = new List<SpriteRenderer>();
    private List<EnemyAI> disabledEnemies = new List<EnemyAI>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (gameOverCanvas != null)
            {
                DontDestroyOnLoad(gameOverCanvas.gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            if (gameOverCanvas != null) Destroy(gameOverCanvas.gameObject);
            return;
        }

        InitializeUI();
    }

    void InitializeUI()
    {
        // Находим компоненты автоматически если они не установлены
        if (gameOverCanvas == null)
        {
            gameOverCanvas = GetComponentInChildren<Canvas>();
        }

        if (gameOverPanel == null && gameOverCanvas != null)
        {
            gameOverPanel = gameOverCanvas.transform.Find("GameOverPanel")?.gameObject;
        }

        if (gameOverText == null && gameOverPanel != null)
        {
            gameOverText = gameOverPanel.transform.Find("GameOverText_TMP")?.GetComponent<TextMeshProUGUI>();
        }

        if (mainMenuButton == null && gameOverPanel != null)
        {
            mainMenuButton = gameOverPanel.transform.Find("MainMenuButton")?.GetComponent<Button>();
        }

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
                panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            }

            gameOverPanel.SetActive(false);
        }

        if (gameOverText != null)
        {
            gameOverText.text = gameOverMessage;
            gameOverText.gameObject.SetActive(false);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(GoToMainMenu);
            mainMenuButton.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        FindPlayer();
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
        Debug.Log($"GameOverManager: загружена сцена '{scene.name}'");

        if (isGameOver)
        {
            ResetGameOver();
        }

        FindPlayer();

        if (playerStats != null && playerStats.CurrentHP <= 0)
        {
            Debug.LogWarning("Игрок найден мертвым в новой сцене!");
            playerStats.Heal(playerStats.MaxHP); 
        }
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                Debug.Log($"GameOverManager нашел игрока (HP: {playerStats.CurrentHP}/{playerStats.MaxHP})");
            }
            else
            {
                Debug.LogWarning("Игрок найден, но у него нет PlayerStats!");
            }
        }
        else
        {
            Debug.LogWarning("Игрок не найден в сцене!");
            playerStats = null;
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
;
        isGameOver = true;

        if (hideSpritesOnGameOver)
        {
            HideAllSprites();
        }

        DisableAllEnemies();

        StopPlayerAnimations();

        gameOverPanel.SetActive(true);

        if (gameOverText != null) gameOverText.gameObject.SetActive(true);
        if (mainMenuButton != null) mainMenuButton.gameObject.SetActive(true);

        StartCoroutine(FadeInGameOverScreen());

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void HideAllSprites()
    {
        hiddenSprites.Clear();

        SpriteRenderer[] allSprites = FindObjectsOfType<SpriteRenderer>();

        foreach (SpriteRenderer sprite in allSprites)
        {
            if (sprite.gameObject.CompareTag("Player"))
                continue;

            if (gameOverCanvas != null && sprite.transform.IsChildOf(gameOverCanvas.transform))
                continue;

            if (sprite.enabled)
            {
                hiddenSprites.Add(sprite);
                sprite.enabled = false;
            }
        }

    }

    void DisableAllEnemies()
    {
        disabledEnemies.Clear();

        EnemyAI[] allEnemies = FindObjectsOfType<EnemyAI>();

        foreach (EnemyAI enemy in allEnemies)
        {
            if (enemy.enabled)
            {
                disabledEnemies.Add(enemy);
                enemy.enabled = false;

                Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Static;
                }
            }
        }

    }

    void StopPlayerAnimations()
    {
        if (playerStats != null)
        {
            PlayerAnimatorController animController = playerStats.GetComponent<PlayerAnimatorController>();
            if (animController != null)
            {
                animController.StopAllAnimations();

                Animator playerAnimator = playerStats.GetComponent<Animator>();
                if (playerAnimator != null)
                {
                    playerAnimator.enabled = false;
                }
            }

            Rigidbody2D playerRb = playerStats.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.bodyType = RigidbodyType2D.Static;
            }

            Collider2D[] playerColliders = playerStats.GetComponents<Collider2D>();
            foreach (Collider2D collider in playerColliders)
            {
                collider.enabled = false;
            }
        }
    }

    void RestoreAllSprites()
    {
        foreach (SpriteRenderer sprite in hiddenSprites)
        {
            if (sprite != null)
            {
                sprite.enabled = true;
            }
        }
        hiddenSprites.Clear();

        foreach (EnemyAI enemy in disabledEnemies)
        {
            if (enemy != null)
            {
                enemy.enabled = true;

                Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                }
            }
        }
        disabledEnemies.Clear();

        if (playerStats != null)
        {

            PlayerAnimatorController animController = playerStats.GetComponent<PlayerAnimatorController>();
            if (animController != null)
            {
                animController.RestartAnimations();
            }
            Animator playerAnimator = playerStats.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.enabled = true;
            }
            Rigidbody2D playerRb = playerStats.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.bodyType = RigidbodyType2D.Dynamic;
            }
            Collider2D[] playerColliders = playerStats.GetComponents<Collider2D>();
            foreach (Collider2D collider in playerColliders)
            {
                collider.enabled = true;
            }
        }
    }

    IEnumerator FadeInGameOverScreen()
    {
        if (panelCanvasGroup == null) yield break;

        float elapsedTime = 0f;
        panelCanvasGroup.alpha = 0f;

        while (elapsedTime < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            panelCanvasGroup.alpha = alpha;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panelCanvasGroup.alpha = 1f;
    }

    void GoToMainMenu()
    {
        RestoreAllSprites();

        ResetGameOver();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }

        Destroy(gameObject);
        if (gameOverCanvas != null) Destroy(gameOverCanvas.gameObject);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void ResetGameOver()
    {

        RestoreAllSprites();

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
    }

    public void ForceGameOver()
    {
        if (!isGameOver)
        {
            ShowGameOverScreen();
        }
    }

    public void ForceReset()
    {
        ResetGameOver();
    }
}