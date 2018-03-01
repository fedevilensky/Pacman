using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;
    public AudioSource bgm;
    public GameObject[] enemies;
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
                float distance = Vector2.Distance(GameManager.instance.player.transform.position, enemy.transform.position);
                distance = (distance > maxDistance) ? maxDistance : (distance < minDistance) ? minDistance : distance;

                float volume = 1 - (distance - minDistance) / (maxDistance - minDistance);

                enemy.GetComponent<AudioSource>().volume = volume;
            }
        }
    }

}
