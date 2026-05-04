using UniRx;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public static Subject<PlayerSpawner> OnPlayerSpawned = new Subject<PlayerSpawner>();
    public Transform spawnPosition;
    void Start()
    {
        //trigger a unirx event 
        OnPlayerSpawned.OnNext(this);
    }
    public void SpawnPlayer()
    {
        GameObject player = Instantiate(GameManagerScripts.GameManager.Get().playerPrefab, spawnPosition.position, Quaternion.identity);
        player.name = "Player";
        player.transform.position = spawnPosition.position;
    }
    public void LinkCinemachineCamera()
    {
        Cinemachine.CinemachineVirtualCamera virtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            virtualCamera.Follow = GameObject.Find("Player").transform;
            virtualCamera.LookAt = GameObject.Find("Player").transform;
        }
    }
}
