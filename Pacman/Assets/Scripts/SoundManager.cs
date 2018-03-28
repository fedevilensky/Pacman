using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//klankbeeld sounds
public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;
    public AudioSource bgm;
    public GameObject[] enemies;
    public AudioClip[] enemyAudios;
    public float maxDistance;
    public float intervalDistance;
    public float minDistance;
    public string error;
    private float closestDistance;
    private AudioSource[] aSources;

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
        aSources = GetComponents<AudioSource>();
        for (int i = 0; i < aSources.Length; i++)
        {
            aSources[i].clip = enemyAudios[i];
            aSources[i].loop = true;
            aSources[i].volume = 0;
            aSources[i].Play();
        }
        aSources[0].volume = 0.5f;
        DontDestroyOnLoad(gameObject);
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.loading)
        {
            closestDistance = maxDistance+1;
            foreach (GameObject enemy in enemies)
            {
                float distance = Vector2.Distance(enemy.transform.position, GameManager.instance.player.transform.position);
                distance = (distance > maxDistance) ? maxDistance : (distance < minDistance) ? minDistance : distance;
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                }
            }
            AdjustAudios();
        }
    }


    public void AdjustAudios()
    {
        int audioNumber = 1;
        float distanceRange = maxDistance;
        while (distanceRange > closestDistance && audioNumber < enemyAudios.Length)
        {
            float volume = 1 - (closestDistance - minDistance) / (distanceRange - minDistance);
            distanceRange -= intervalDistance;
            aSources[audioNumber].volume = volume;
            error = "playing " + (audioNumber) + "at" +volume;
            audioNumber++;
        }
        for (int i = audioNumber; i < aSources.Length; i++)
        {
            error += "muting " + (i);
            aSources[i].volume = 0f;
        }
    }
    /*
    public void LoadEnemySound(GameObject enemy, float distance)
    {
        if (audioNumber != enemySound)
        {
            enemySound = audioNumber;
            error =""+ audioNumber;
            AudioSource source;
            source = enemy.GetComponent<AudioSource>();
            source.clip = enemyAudios[audioNumber];
            source.loop = true;
            source.Play();
        }
    }*/
    

}
