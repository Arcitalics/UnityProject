using UnityEngine;
using TMPro;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject[] zombiePrefabs;
    public Transform spawnPoint;

    public int[] waves = { 3, 4, 5 };

    public float timeBetweenWaves = 2f;

    public TextMeshProUGUI waveText;

    private int currentWave = 0;
    private int zombiesAlive = 0;

    private bool started = false;

    public void StartWaves()
    {
        if (started) return;

        started = true;
        StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        while (currentWave < waves.Length)
        {
            yield return StartCoroutine(ShowWaveText($"Wave {currentWave + 1} - Fight!"));

            SpawnWave(waves[currentWave]);

            // wait until all zombies are dead
            yield return new WaitUntil(() => zombiesAlive <= 0);

            currentWave++;

            yield return new WaitForSeconds(timeBetweenWaves);
        }

        yield return StartCoroutine(ShowWaveText("All Waves Cleared!"));
    }

    void SpawnWave(int count)
    {
        zombiesAlive = count;

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = zombiePrefabs[Random.Range(0, zombiePrefabs.Length)];

            if (prefab == null) continue;

            // choose side: -1 (left) or +1 (right)
            int side = Random.value > 0.5f ? 1 : -1;

            float minDistance = 6f;  // distance away from player
            float randomOffset = Random.Range(0f, 2f);

            Vector2 spawnPos = new Vector2(
                player.position.x + side * (minDistance + randomOffset),
                spawnPoint.position.y
            );

            GameObject zombie = Instantiate(prefab, spawnPos, Quaternion.identity);

            zombie.GetComponent<ZombieAI>().spawner = this;
        }
    }
    public void ZombieDied()
    {
        zombiesAlive--;
    }

    IEnumerator ShowWaveText(string text)
    {
        waveText.text = text;
        waveText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        waveText.gameObject.SetActive(false);
    }
}
