using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MovingObject {
    public GameObject killRadius;
    public string log = "";
    public float maxSightDistance;
    [HideInInspector] public StrategySelector strategy;
    [HideInInspector] public bool isDead;

    private bool calculatingPath = false;
    private float timeSinceDeath = 0;
    private int infinity = -1;
    private Vertex lastPlayerPosition = null;
    private Vertex lastPosition = null;
    private VertexEqualityComparer vertexComp = new VertexEqualityComparer();
    private Vertex destination;
    private Vector2 moveDirection;
    private float respawnDistance = 4f;

    void Awake() {
        moveDirection = Vector2.zero;
        maxSightDistance = 4f;
        isDead = false;
    }

    private void OnCollisionEnter2D(Collision2D coll) {
        if (!GameManager.instance.hasEnded) {
            if (coll.gameObject.tag == "Player") {
                if (!GameManager.instance.playerHasGun) {
                    GameManager.instance.gameOver = true;
                }
            }
        }
    }
    

    private bool NegligibleVelocity() {
        return Mathf.Sqrt(Mathf.Pow(rb2D.velocity.x, 2) + Mathf.Pow(rb2D.velocity.y, 2)) < (movingSpeed * GameManager.instance.gameSpeed) / 3;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!GameManager.instance.loading) {
            if (!isDead) {
                MoveEnemy();
            }
            else {
                RespawnEnemy();
            }
        }
    }

    private void RespawnEnemy() {
        if (!GameManager.instance.playerHasGun && Vector3.Distance(GameManager.instance.player.transform.position, transform.position) > respawnDistance)
            transform.position = new Vector3(-100, -100);
        timeSinceDeath += Time.deltaTime;
        rb2D.velocity = Vector2.zero;
        if (timeSinceDeath >= GameManager.instance.enemySpawnRate) {
            int spawnPoint;
            do {
                spawnPoint = UnityEngine.Random.Range(0, GameManager.instance.waypointList.Count);
            } while (!GameManager.instance.FarFromPlayerSpawn(spawnPoint));
            transform.position = GameManager.instance.waypointList[spawnPoint];
            timeSinceDeath = 0;
            isDead = false;
        }
    }

    private void MoveEnemy() {
        killRadius.SetActive(GameManager.instance.playerHasGun);
        strategy.Update(Time.fixedDeltaTime);
        Vertex playerPosition = GameManager.instance.RealCoordsToMap(GameManager.instance.player.transform.position);
        Vertex myPos = GameManager.instance.RealCoordsToMap(transform.position);
        movingSpeed = strategy.Velocity;
        if (moveDirection == Vector2.zero || CanTurn(transform.position)) {
            if (SeePlayer(playerPosition, myPos) && strategy.InteractOnSight()) {
                log += "  saw player";
                moveDirection = strategy.MoveOnSight(playerPosition, myPos, GameManager.instance.player.GetComponent<Rigidbody2D>().velocity);
                destination = null;
            }
            else if (destination == null && (NegligibleVelocity() || Graph.ContainsVertex(myPos)) || vertexComp.Equals(destination, myPos)) {
                destination = strategy.NextVertex(playerPosition, myPos, GameManager.instance.player.GetComponent<Rigidbody2D>().velocity);
                GetMovementDirection(destination);
            }
            Move(moveDirection);
        }
    }

    private float Distance(Vertex v1, Vertex v2) {
        return Mathf.Sqrt(Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.y - v2.y, 2));
    }

    private bool SeePlayer(Vertex v1, Vertex v2) {
        if (v1.x != v2.x && v1.y != v2.y || Distance(v1, v2) >= maxSightDistance)
            return false;
        else {
            bool ret;
            if (v1.x != v2.x) {
                int pos = v1.x;
                int it = 1;
                if (pos > v2.x)
                    it = -1;
                while (pos != v2.x && GameManager.instance.tileManager.IsFloor(pos, v1.y)) {
                    pos += it;
                }
                ret = GameManager.instance.tileManager.IsFloor(pos, v1.y);
            }
            else {
                int pos = v1.y;
                int it = 1;
                if (pos > v2.y)
                    it = -1;
                while (pos != v2.y && GameManager.instance.tileManager.IsFloor(v1.x, pos)) {
                    pos += it;
                }
                ret = GameManager.instance.tileManager.IsFloor(v1.x, pos);
            }
            return ret;
        }
    }

    private bool CanTurn(Vector3 pos) {
        float xPos = Mathf.Abs(pos.x - (int)pos.x);
        float yPos = Mathf.Abs(pos.y - (int)pos.y);
        return IsInTurnRange(xPos) && IsInTurnRange(yPos);
    }

    private bool IsInTurnRange(float val) {
        return (val > 0.3f && val < 0.7f);
    }

    private void GetMovementDirection(Vertex to) {
        if (to == null) {
            moveDirection = Vector2.zero;
        }
        else {
            Vertex myPos = GameManager.instance.RealCoordsToMap(transform.position);
            int xDirection = (to.x - myPos.x) > 0 ? 1 : ((to.x - myPos.x) < 0 ? -1 : 0);
            int yDirection = (myPos.y - to.y) > 0 ? 1 : ((myPos.y - to.y) < 0 ? -1 : 0);
            moveDirection = new Vector2(xDirection, yDirection);
        }
    }


}
