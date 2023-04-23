using System.Collections;
using System.Collections.Generic;
using Malee.List;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawnManager : MonoBehaviour
{
    [Reorderable]
    public ListOfEnemySpawnWaves waves;

    public float currentWave = 0;

    public float spawnRadius = 10f;

    public Exit playerExit;

    [Header("Listening On")]
    [SerializeField]
    private VoidEventChannelSO _onSceneReady = default; //Raised by SceneLoader when the scene is set to active

    private void OnEnable()
    {
        _onSceneReady.OnEventRaised += SpawnWaves;
        SceneManager.activeSceneChanged += OnSceneChanged;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        _onSceneReady.OnEventRaised -= SpawnWaves;
        SceneManager.activeSceneChanged -= OnSceneChanged;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene current)
    {
        playerExit = null;
    }

    private void OnSceneChanged(Scene current, Scene next)
    {
        playerExit = null;
        GameObject exitGO = GameObject.FindWithTag("PlayerExit");
        if (exitGO != null)
        {
            playerExit = exitGO.GetComponent<PlayerExit>();
            LockPlayerExit();
        }
    }

    void SpawnWaves()
    {
        StartCoroutine(SpawnWavesRoutine());
    }

    IEnumerator SpawnWavesRoutine()
    {
        Debug.Log("Spawning waves");
        for (int i = 0; i < waves.Count; i++)
        {
            waves[i].SpawnWave();
            while (!waves[i].waveComplete)
                yield return null;
            Debug.Log("Wave: " + (i+1) + "Completed!");
        }
        UnlockPlayerExit();
    }

    private void LockPlayerExit()
    {
        if (playerExit != null) playerExit.locked = true;
    }

    private void UnlockPlayerExit()
    {
        if (playerExit != null) playerExit.locked = false;
    }
}

[System.Serializable]
public class ListOfEnemySpawnWaves : ReorderableArray<EnemySpawnWave> { }

[System.Serializable]
public class EnemySpawnWave
{
    public string name;

    // public WaveSpawnType waveSpawnType;
    // public WaveTriggerType waveTriggerType;

    // public float timer;

    // private float _timer;

    // public Collider colliderTrigger;

    public float spawnRadius = 10f;

    public BasicEnemy[] enemiesToSpawn;
    [HideInInspector]
    public int currentEnemies;

    public bool waveComplete = false;

    public void SpawnWave()
    {
        currentEnemies = enemiesToSpawn.Length;
        foreach (var enemyPrefab in enemiesToSpawn)
        {
            BasicEnemy enemy = GameObject.Instantiate(enemyPrefab, PickRandomSpawnPoint(), Quaternion.identity).GetComponent<BasicEnemy>();
            enemy.OnEnemyDied.AddListener (EnemyKilled);
        }
    }

    private void EnemyKilled(BasicEnemy enemy)
    {
        if (currentEnemies - 1 <= 0)
        {
            currentEnemies = 0;
            waveComplete = true;
        }
        else
            currentEnemies -= 1;
    }

    private Vector3 PickRandomSpawnPoint()
    {
        return Vector3.zero;
    }
}

public enum WaveTriggerType
{
    Timer,
    EliminateLastWave,
    TriggerCollider
}

public enum WaveSpawnType
{
    All,
    OneByOne,
    Random
}
