using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject player;
    public GameObject playerPrefab;
    public GameObject teleporter;
    private UIManager uiManager;

    const float respawnTime = 1f;
    const float teleportTime = 10f;
    float respawnTimer = 0f;
    float teleportTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        uiManager = GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Respawn"))
        {
            RestartLoop();
        }

        if (respawnTimer > 0f)
        {
            respawnTimer -= Time.deltaTime;
        }
        else if (player == null)
        {
            Respawn();
        }

        if (player != null) {
            teleportTimer -= Time.deltaTime;
            if (teleportTimer <= 0f) {
                // Restart loop
                //RestartLoop();
                teleportTimer = teleportTime;
            }
            uiManager.SetTeleportTimer(teleportTimer);
        }
    }

    void RestartLoop()
    {
        if (player != null) 
        {
            DestroyPlayer();
        }
        respawnTimer = respawnTime;
    }

    void DestroyPlayer()
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        controller.PrepareRespawn();
        Destroy(player);
        player = null;
    }

    void Respawn()
    {
        player = GameObject.Instantiate(playerPrefab, 
                                        teleporter.transform.position + Vector3.up * 1.2f, 
                                        Quaternion.Euler(0f, teleporter.transform.rotation.eulerAngles.y, 0f));
        teleportTimer = teleportTime;
        
    }
}
