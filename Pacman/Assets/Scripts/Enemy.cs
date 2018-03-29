using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MovingObject
{
    public GameObject killRadius;
    public string log = "";
    public float maxSightDistance;
    [HideInInspector] public StrategySelector strategy;

    private bool calculatingPath = false;
    private int infinity = -1;
    private Vertex lastPlayerPosition = null;
    private Vertex lastPosition = null;
    private VertexEqualityComparer vertexComp = new VertexEqualityComparer();
    private Vertex destination;
    private Navigator navigator;
    private Vector2 moveDirection;

    void Awake()
    {
        navigator = new Navigator();
        moveDirection = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (!GameManager.instance.hasEnded)
        {
            if (coll.gameObject.tag == "Player")
            {
                if (!GameManager.instance.playerHasGun)
                {
                    GameManager.instance.gameOver = true;
                }
                CollisionWithPlayer();
            }
        }
    }

    public void CollisionWithPlayer()
    {
        rb2D.bodyType = RigidbodyType2D.Static;
    }

    private bool NegligibleVelocity()
    {
        return Mathf.Sqrt(Mathf.Pow(rb2D.velocity.x, 2) + Mathf.Pow(rb2D.velocity.y, 2)) < (movingSpeed * GameManager.instance.gameSpeed) / 3;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameManager.instance.loading)
        {
            killRadius.SetActive(GameManager.instance.playerHasGun);
            strategy.Update(Time.fixedDeltaTime);
            Vertex playerPosition = GameManager.instance.RealCoordsToMap(GameManager.instance.player.transform.position);
            Vertex myPos = GameManager.instance.RealCoordsToMap(transform.position);
            movingSpeed = strategy.Velocity;
            if (moveDirection == Vector2.zero || CanTurn(transform.position))
            {
                if (SeePlayer(playerPosition, myPos) && strategy.InteractOnSight())
                {
                    moveDirection = strategy.MoveOnSight(playerPosition, myPos, GameManager.instance.player.GetComponent<Rigidbody2D>().velocity);
                    destination = null;
                }
                else if (destination == null && NegligibleVelocity()  || Graph.ContainsVertex(myPos))
                {
                    destination = strategy.NextVertex(playerPosition, myPos, GameManager.instance.player.GetComponent<Rigidbody2D>().velocity);
                    GetMovementDirection(destination);
                }
                Move(moveDirection);
            }


            //FALTA COMPORTAMIENTO SI VE AL JUGADOR Y COMPORTAMIENTO DE SI EL JUGADOR TIENE EL ARMA
            /*
            if (lastPlayerPosition == null || !vertexComp.Equals(lastPlayerPosition, playerPosition))
            {

                lastPlayerPosition = playerPosition;
                if (!calculatingPath && (lastPosition == null || !vertexComp.Equals(lastPlayerPosition, lastPosition)))
                {
                    if (lastPosition == null)
                    {
                        lastPosition = myPos;
                    }

                    if (GameManager.instance.playerHasGun)
                    {
                        killRadius.SetActive(true);
                        if (!(lastPathfind == Pathfinding.RUNNING_AWAY && timeSinceLastPathfind < pathfindingDelay))
                        {
                            if (lastPathfind != Pathfinding.RUNNING_AWAY || wayToDestination.Count < 2 || rb2D.velocity == Vector2.zero)
                            {
                               // destination = FindWaypoint();
                            }
                            lastPathfind = Pathfinding.RUNNING_AWAY;
                            AStarWithQueue(distanceCalculator);
                            timeSinceLastPathfind = 0f;
                        }

                    }
                    else
                    {
                        killRadius.SetActive(false);
                        if (!(lastPathfind == Pathfinding.CHASING && timeSinceLastPathfind < pathfindingDelay))
                        {
                            lastPathfind = Pathfinding.CHASING;
                            destination = lastPlayerPosition;
                            AStarWithQueue(nullCalculator);
                            timeSinceLastPathfind = 0f;
                        }
                    }
                }
            }

            if (wayToDestination.Count > 1)
            {
                Vector2 movementDirection = GetMovementDirection(myPos);
                Move(movementDirection);
            }
            else if (GameManager.instance.playerHasGun)
            {
                lastPlayerPosition = null;
                destination = null;
            }*/
        }
    }

    private float Distance(Vertex v1, Vertex v2)
    {
        return Mathf.Sqrt(Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.y - v2.y, 2));
    }

    private bool SeePlayer(Vertex v1, Vertex v2)
    {
        if (v1.x != v2.x && v1.y != v2.y || Distance(v1, v2) >= maxSightDistance)
            return false;
        else
        {
            bool ret;
            if (v1.x != v2.x)
            {
                int pos = v1.x;
                int it = 1;
                if (pos > v2.x)
                    it = -1;
                while (pos != v2.x && GameManager.instance.tileManager.IsFloor(pos, v1.y))
                {
                    pos += it;
                }
                ret = GameManager.instance.tileManager.IsFloor(pos, v1.y);
            }
            else
            {
                int pos = v1.y;
                int it = 1;
                if (pos > v2.y)
                    it = -1;
                while (pos != v2.y && GameManager.instance.tileManager.IsFloor(v1.x, pos))
                {
                    pos += it;
                }
                ret = GameManager.instance.tileManager.IsFloor(v1.x, pos);
            }
            return ret;
        }
    }

    private bool CanTurn(Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x - (int)pos.x);
        float yPos = Mathf.Abs(pos.y - (int)pos.y);
        return IsInTurnRange(xPos) && IsInTurnRange(yPos);
    }

    private bool IsInTurnRange(float val)
    {
        return (val > 0.3f && val < 0.7f);
    }

    private void GetMovementDirection(Vertex to)
    {
        if (to == null)
        {
            moveDirection = Vector2.zero;
        }
        else
        {
            Vertex myPos = GameManager.instance.RealCoordsToMap(transform.position);
            int xDirection = (to.x - myPos.x) > 0 ? 1 : ((to.x - myPos.x) < 0 ? -1 : 0);
            int yDirection = (myPos.y - to.y) > 0 ? 1 : ((myPos.y - to.y) < 0 ? -1 : 0);
            moveDirection = new Vector2(xDirection, yDirection);
        }
    }


}
