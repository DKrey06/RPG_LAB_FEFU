using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private string _targetScene = "Scene_2";
    [SerializeField] private string _spawnPointName = "Spawn_Scene2";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TransitionManager.Instance?.MoveToScene(_targetScene, _spawnPointName);
        }
    }
}