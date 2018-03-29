using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

    protected float movingSpeed;

    protected Rigidbody2D rb2D;

	// Use this for initialization
	protected virtual void Start () {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
	}

    protected void Move(Vector2 movement)
    {
        if (!GameManager.instance.hasEnded)
        {
            rb2D.velocity = movement * movingSpeed*GameManager.instance.gameSpeed;
        }
        else
        {
            rb2D.bodyType = RigidbodyType2D.Static;
        }
    }


}
