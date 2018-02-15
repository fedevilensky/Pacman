using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

    public float movingSpeed;

    protected Rigidbody2D rb2D;

	// Use this for initialization
	protected virtual void Start () {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
	}

    protected void Move(Vector2 movement)
    {
        rb2D.velocity = movement*movingSpeed;
    }


}
