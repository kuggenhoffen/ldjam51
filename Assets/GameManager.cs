using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameObject player;
    public GameObject playerPrefab;
    public GameObject teleporter;
    private UIManager uiManager;

    const float initialRespawnTime = 3f;
    const float respawnTime = 1f;
    const float teleportTime = 10f;
    float respawnTimer = respawnTime;
    float teleportTimer = 0f;
    private AudioSource audioSource;
    public AudioClip teleportFinished;
    public AudioClip teleportActivating;
    public AudioClip dropSfx;
    public AudioClip beepSfx;
    bool teleportActivatingPlayed = false;
    public Blackout blackoutPanel;
    private PlayerController controller;

    private enum PlayState {
        respawning,
        playing,
        gameover
    };
    private int loopCount = 0;
    private float gameoverTimer = 0f;
    private int teleportTimerInt;

    private PlayState state = PlayState.respawning;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        controller = player.GetComponent<PlayerController>();
        uiManager = GetComponent<UIManager>();
        audioSource = teleporter.GetComponent<AudioSource>();
        RestartLoop(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor && Input.GetButtonDown("Respawn"))
        {
            RestartLoop();
        }

        if (state == PlayState.playing && Input.GetButton("Cancel")) {
            GameOver(false);
        }

        if (state == PlayState.gameover) {
            gameoverTimer += Time.deltaTime;
            if (gameoverTimer > 2f && Input.GetButtonDown("Interact")) {
                SceneManager.LoadScene("TitleScene");
            }
        }

        if (respawnTimer > 0f)
        {
            respawnTimer -= Time.deltaTime;
        }
        else if (state == PlayState.respawning)
        {
            Respawn();
            state = PlayState.playing;
        }

        if (state == PlayState.playing) {
            teleportTimer -= Time.deltaTime;
            if (teleportTimer <= 0f) {
                // Restart loop
                RestartLoop();
                teleportTimer = teleportTime;
            }
            else if (teleportTimer < teleportActivating.length - 0.4f && !teleportActivatingPlayed) {
                teleportActivatingPlayed = true;
                audioSource.PlayOneShot(teleportActivating);
            }
            else if (teleportTimer > 0f && teleportTimer < teleportTime - 1f) {
                if ((int)teleportTimer != teleportTimerInt) {
                    audioSource.PlayOneShot(beepSfx);
                    teleportTimerInt = (int)teleportTimer;
                }
            }
            uiManager.SetTeleportTimer(teleportTimer);
        }
    }

    void RestartLoop(bool initial = false)
    {
        float blackoutDelay = initial ? 0f : 0.05f;
        DestroyPlayer();
        controller.PrepareRespawn();
        respawnTimer = initial ? initialRespawnTime : respawnTime;
        state = PlayState.respawning;
        blackoutPanel.SetBlackout(true, blackoutDelay);
        if (initial) {
            audioSource.PlayOneShot(teleportActivating);
        }
    }

    void DestroyPlayer()
    {
        //Destroy(player);
        //player = null;
    }

    void Respawn()
    {
        //player = GameObject.Instantiate(playerPrefab, 
        //                                teleporter.transform.position + Vector3.up * 1.2f, 
        //                                Quaternion.Euler(0f, teleporter.transform.rotation.eulerAngles.y, 0f));
        controller.SetTransform(teleporter.transform.position + Vector3.up * 1.2f, Quaternion.Euler(0f, teleporter.transform.rotation.eulerAngles.y, 0f));
        controller.inputActive = true;
        teleportTimer = teleportTime;
        teleportActivatingPlayed = false;
        blackoutPanel.SetBlackout(false, 2f);
        audioSource.PlayOneShot(teleportFinished);
        loopCount++;
        if (loopCount == 1) {
            uiManager.ShowToastTip("The teleporter is stuck in a retry loop, I need to figure out a way to fix it!", 3f);
        }
    }

    public void GameOver(bool success)
    {
        state = PlayState.gameover;
        if (audioSource.isPlaying) {
            audioSource.Stop();
        }
        controller.inputActive = false;
        blackoutPanel.SetBlackout(true, 2f);
        blackoutPanel.ShowEndscreen(loopCount, success);
    }
}
