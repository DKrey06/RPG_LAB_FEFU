using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }
    private Transform _player;
    private string _targetSpawnPointName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);    
        }
    }

    public void MoveToScene(string sceneName, string spawnPointName)
    {
        _targetSpawnPointName = spawnPointName;
        SceneManager.LoadScene(sceneName);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_player == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) _player = playerObj.transform;
        }

        if (!string.IsNullOrEmpty(_targetSpawnPointName))
        {
            var spawn = GameObject.Find(_targetSpawnPointName);
            if (spawn != null && _player != null)
            {
                _player.position = spawn.transform.position;
            }
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
