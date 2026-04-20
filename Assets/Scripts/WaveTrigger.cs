using UnityEngine;

public class WaveTrigger : MonoBehaviour
{
    public ZombieSpawner spawner;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !triggered)
        {
            triggered = true;
            spawner.StartWaves();
        }
    }
}