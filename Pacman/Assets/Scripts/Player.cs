using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    public float gunDuration = 15;
    public bool hasGun = false;
    public GameObject lighting;
    public GameObject gun;
    public float maxLightingScale = 15;


    private float gunTimeLeft;
    private Vector3 lightingScale;

    override protected void Start()
    {
        base.Start();

        lightingScale = lighting.transform.localScale;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {

        float movementX = Input.GetAxis("Horizontal");
        float movementY = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(movementX, movementY);
        // StopMoving(movementX, movementY);
        Move(movement);

        if (hasGun)
        {
            gunTimeLeft -= Time.deltaTime;
            //lightSize start

            float scale = gunTimeLeft * (maxLightingScale - lightingScale.x) / gunDuration + lightingScale.x;

            lighting.transform.localScale = new Vector3(scale, scale, 1);

            //lightSize end



            if (gunTimeLeft < 0)
            {
                UnequipGun();
            }
        }

    }

    private void UnequipGun()
    {
        hasGun = false;
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("UnequipGun");
        GameManager.instance.gunIsSpawned = false;
        gun.SetActive(false);
        //lighting.SetActive(true);
        GameManager.instance.playerHasGun = false;
    }

    void StopMoving(float x, float y)
    {
        float xVelocity = (x == 0) ? 0 : rb2D.velocity.x;
        float yVelocity = (y == 0) ? 0 : rb2D.velocity.y;

        rb2D.velocity = new Vector2(xVelocity, yVelocity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Gun")
        {
            GameManager.instance.DestroyGun();

            EquipGun();
        }
    }

    private void EquipGun()
    {
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("EquipGun");
        hasGun = true;
        gunTimeLeft = gunDuration;
        //lighting.SetActive(false);
        gun.SetActive(true);

        GameManager.instance.playerHasGun = true;
    }
}
