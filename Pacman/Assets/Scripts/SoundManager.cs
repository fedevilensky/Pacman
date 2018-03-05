using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;
    public AudioSource bgm;
    public GameObject[] enemies;
    public AudioClip[] enemyAudios;
    public float maxDistance;
    public float minDistance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.loading)
        {

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector2.Distance(enemy.transform.position, GameManager.instance.player.transform.position);
                distance = (distance > maxDistance) ? maxDistance : (distance < minDistance) ? minDistance : distance;

                float volume = 1 - (distance - minDistance) / (maxDistance - minDistance);

                enemy.GetComponent<AudioSource>().volume = volume;
            }
        }
    }

    public void LoadEnemySound()
    {
        AudioSource source;

        foreach (GameObject enemy in enemies)
        {
            source = enemy.GetComponent<AudioSource>();
            source.clip = enemyAudios[Random.Range(0, enemyAudios.Length)];
            source.loop = true;
            source.Play();
        }
    }

}
