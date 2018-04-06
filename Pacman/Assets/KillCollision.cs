using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCollision : MonoBehaviour {

    public GameObject enemyObject;
    public string error;

    private Enemy enemy;

    private void Awake()
    {
        enemy = enemyObject.GetComponent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {

        if (!GameManager.instance.hasEnded)
        {
            if (coll.gameObject.tag == "Player")
            {
                if (GameManager.instance.playerHasGun)
                {

                    GameManager.instance.enemyDead = true;
                }

                enemy.CollisionWithPlayer();
            }
        }
    }
}
