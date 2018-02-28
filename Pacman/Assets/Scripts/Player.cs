using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObject {
    
    // Update is called once per frame
    void FixedUpdate ()
    {
            float movementX = Input.GetAxis("Horizontal");
            float movementY = Input.GetAxis("Vertical");
            Vector2 movement = new Vector2(movementX, movementY);
            // StopMoving(movementX, movementY);
            Move(movement);
    }

    void StopMoving(float x, float y)
    {
        float xVelocity = (x == 0) ? 0 : rb2D.velocity.x;
        float yVelocity = (y == 0) ? 0 : rb2D.velocity.y;

        rb2D.velocity = new Vector2(xVelocity, yVelocity);
    }

    
}
