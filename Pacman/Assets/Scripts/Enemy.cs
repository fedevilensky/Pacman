using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Pathfinding
{
    RUNNING_AWAY,
    CHASING
}

public class Enemy : MovingObject
{
    public GameObject killRadius;
    public const float pathfindingDelay = 1f;
    public float timeSinceLastPathfind;
    public Pathfinding lastPathfind;
    public string log = "";

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
        timeSinceLastPathfind = pathfindingDelay;
        lastPathfind = Pathfinding.CHASING;
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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameManager.instance.hasEnded)
        {
            Vertex playerPosition = GameManager.instance.RealCoordsToMap(GameManager.instance.player.transform.position);
            Vertex myPos = GameManager.instance.RealCoordsToMap(transform.position);
            timeSinceLastPathfind += Time.deltaTime;
            if(destination == null || (vertexComp.Equals(myPos, destination)&&CanTurn(transform.position)))
            {
                destination = navigator.GetNextStep(myPos, playerPosition);
                log += navigator.print;
                rb2D.velocity = Vector2.zero;
                ChangeDirection(destination);
            }
                Move(moveDirection);

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

    private bool CanTurn(Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x) - (int)Mathf.Abs(pos.x);
        float yPos = Mathf.Abs(pos.y) - (int)Mathf.Abs(pos.y);
        if (IsInTurnRange(xPos) && IsInTurnRange(yPos))
            return true;
        else
            return false;
    }

    private bool IsInTurnRange(float val)
    {
        return (val > 0.3f && val < 0.7f);
    }

    private void ChangeDirection(Vertex to)
    {
        Vertex myPos = GameManager.instance.RealCoordsToMap(transform.position);
        int xDirection = (to.x - myPos.x) > 0 ? 1 : ((to.x - myPos.x) < 0 ? -1 : 0);
        int yDirection = (myPos.y - to.y) > 0 ? 1 : ((myPos.y - to.y) < 0 ? -1 : 0);
        moveDirection =  new Vector2(xDirection, yDirection);
    }
    
    
    
    /*
    private Vector3 FindWaypoint()
    {
        Vertex ret = lastPlayerPosition;
        int max = 0;
        foreach (Vector3 v in GameManager.instance.waypointList)
        {
            if (destination == null || !vertexComp.Equals(v, destination))
            {
                int newDistance = distanceCalculator.Calculate(v, lastPlayerPosition);
                if (max < newDistance)
                {
                    max = newDistance;
                    ret = v;
                }
            }
        }

        return ret;
    }*/
}
