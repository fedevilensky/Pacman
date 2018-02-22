using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour {

    public float maxDistance;
    public float minDistance;

    private SpriteRenderer sprRenderer;


	// Use this for initialization
	void Start () {
        sprRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        DistanceLightColor();

    }
    void DistanceLightColor()
    {
        Vector2 playerPosition = GameManager.instance.player.transform.position;
        Vector2 enemyPosition = GameManager.instance.enemy.transform.position;
        float distance = Vector2.Distance(playerPosition, enemyPosition);
        if (distance > maxDistance)
        {
            distance = maxDistance;
        }
        else if (distance < minDistance)
        {
            distance = minDistance;
        }
        float GreenIntensity = (int)(255f * (distance - minDistance) / (maxDistance - minDistance));
        // Color lightColor = new Color(255f, GreenIntensity, 0f,1f);
        Color tmp = sprRenderer.color;
        tmp.g = GreenIntensity/255;
        sprRenderer.color = tmp;
    }
}
