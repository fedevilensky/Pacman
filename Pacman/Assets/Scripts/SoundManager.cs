using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//klankbeeld sounds
public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;
    public GameObject[] enemies;
    public AudioClip[] enemyAudios;
    public AudioClip playerHasGunSound;
    public AudioClip playerDoesNotHaveGun;
    public float maxDistance;
    public float intervalDistance;
    public float minDistance;
    public string error;
    public AudioSource bgm;

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
            foreach (AudioSource a in aSources)
            {
                if (!a.isPlaying)
                {
                    a.Play();
                }
                else break;
            }
            if (GameManager.instance.playerHasGun)
            {
                if (aSources[0].clip != playerHasGunSound)
                {
                    foreach (AudioSource a in aSources)
                    {
                        a.volume = 0;
                    }
                    aSources[0].clip = playerHasGunSound;
                    aSources[0].volume = 0.7f;
                    aSources[0].Play();
                }
            }
            else
            {
                if(aSources[0].clip != playerDoesNotHaveGun)
                {
                    aSources[0].clip = playerDoesNotHaveGun;
                    aSources[0].volume = 0.5f;
                    aSources[0].Play();
                }
                closestDistance = maxDistance + 1;
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
        else 
        {
            foreach(AudioSource a in aSources)
            {
                a.Stop();
            }
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

}
