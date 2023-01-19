using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public enum GhostNodeStatesEnum
    {
        respawning,
        leftNode,
        rightNode,
        centerNode,
        startNode,
        movingInNodes
    }

    public GhostNodeStatesEnum ghostNodeState;
    public GhostNodeStatesEnum respawnState;

    public enum GhostType
    {
        red,
        blue,
        pink,
        orange
    }

    public GhostType ghostType;

    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;

    public MovementController movementController;

    public GameObject startingNode;

    public bool readyToLeaveHome = false;

    public GameManager gameManager;

    public bool testRespawn = false;

    public bool isFrightened = false;

    public GameObject[] scatterNodes;
    public int scatterNodeIndex;

    // Start is called before the first frame update
    void Awake()
    {
        scatterNodeIndex = 0;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementController = GetComponent<MovementController>();
        // Start red ghost on the start node
        if (ghostType == GhostType.red)
        {
            ghostNodeState = GhostNodeStatesEnum.startNode;
            respawnState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeStart;
            readyToLeaveHome = true;
        }
        // Start pink ghost on the center node
        else if (ghostType == GhostType.pink)
        {
            ghostNodeState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeCenter;
            respawnState = GhostNodeStatesEnum.centerNode;
        }
        // Start blue ghost on left node
        else if (ghostType == GhostType.blue)
        {
            ghostNodeState = GhostNodeStatesEnum.leftNode;
            startingNode = ghostNodeLeft;
            respawnState = GhostNodeStatesEnum.leftNode;
        }
        // Start orange ghost on right node
        else if (ghostType == GhostType.orange)
        {
            ghostNodeState = GhostNodeStatesEnum.rightNode;
            startingNode = ghostNodeRight;
            respawnState = GhostNodeStatesEnum.rightNode;
        }
        movementController.currentNode = startingNode;
        transform.position = startingNode.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (testRespawn == true)
        {
            readyToLeaveHome = false;
            ghostNodeState = GhostNodeStatesEnum.respawning;
            testRespawn = false;
        }

        if (movementController.currentNode.GetComponent<NodeController>().isSideNode)
        {
            movementController.SetSpeed(2);
        }
        else
        {
            movementController.SetSpeed(3);
        }
    }

    public void ReachedCenterOfNode(NodeController nodeController)
    {
        if (ghostNodeState == GhostNodeStatesEnum.movingInNodes)
        {
            // Scatter mode
            if (gameManager.currentGhostMode == GameManager.GhostMode.scatter)
            {
                // If we reached the scatter node, add one to our scatter node index
                if (transform.position.x == scatterNodes[scatterNodeIndex].transform.position.x && transform.position.y == scatterNodes[scatterNodeIndex].transform.position.y)
                {
                    scatterNodeIndex++;

                    if (scatterNodeIndex == scatterNodes.Length -1)
                    {
                        scatterNodeIndex = 0;
                    }
                }
                string direction = GetClosestDirection(scatterNodes[scatterNodeIndex].transform.position);
                movementController.setDirection(direction);
            }
            // Frightened mode
            else if (isFrightened)
            {

            }
            // Chase mode
            else
            {
                // Determine next game mode to go to
                if (ghostType == GhostType.red)
                {
                    DetermineRedGhostDirection();
                }
                else if (ghostType == GhostType.pink)
                {
                    DeterminePinkGhostDirection();
                }
                else if (ghostType == GhostType.blue)
                {
                    DetermineBlueGhostDirection();
                }
                else if (ghostType == GhostType.orange)
                {
                    DetermineOrangeGhostDirection();
                }
            }         
        }
        else if (ghostNodeState == GhostNodeStatesEnum.respawning)
        {
            string direction = "";

            // We have reached our start node, move to the center node
            if (transform.position.x == ghostNodeStart.transform.position.x && transform.position.y == ghostNodeStart.transform.position.y)
            {
                direction = "down";
            }
            // We have reached our center node, either finish respawn, or move to the left/right node
            else if (transform.position.x == ghostNodeCenter.transform.position.x && transform.position.y == ghostNodeCenter.transform.position.y)
            {
                if (respawnState == GhostNodeStatesEnum.centerNode)
                {
                    ghostNodeState = respawnState;
                }
                else if (respawnState == GhostNodeStatesEnum.leftNode)
                {
                    direction = "left";
                }
                else if (respawnState == GhostNodeStatesEnum.rightNode)
                {
                    direction = "right";
                }
            }
            // If our respawn state is either the left or right node, and we got to that node, leave home again
            else if (
                (transform.position.x == ghostNodeLeft.transform.position.x && transform.position.y == ghostNodeLeft.transform.position.y)
                || 
                (transform.position.x == ghostNodeRight.transform.position.x && transform.position.y == ghostNodeRight.transform.position.y)
                )
            {
                ghostNodeState = respawnState;
            }
            // We are in the gameboard still, locate our start node
            else
            {
                // Determine the quickest direction to home
                direction = GetClosestDirection(ghostNodeStart.transform.position);
            }

            
            movementController.setDirection(direction);
        }
        else
        {
            // If we are ready to leave home
            if (readyToLeaveHome)
            {
                // If we are in the left home node, move to center
                if (ghostNodeState == GhostNodeStatesEnum.leftNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.centerNode;
                    movementController.setDirection("right");
                }
                // If we are in the right home node, move to center
                else if (ghostNodeState == GhostNodeStatesEnum.rightNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.centerNode;
                    movementController.setDirection("left");
                }
                // If we are in the center node, move to start node
                else if (ghostNodeState == GhostNodeStatesEnum.centerNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.startNode;
                    movementController.setDirection("up");
                }
                // If we are in start node, start moving throughout the nodes
                else if (ghostNodeState == GhostNodeStatesEnum.startNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.movingInNodes;
                    movementController.setDirection("left");
                }
            }
        }
    }

    // Determine the direction of the red ghost
    void DetermineRedGhostDirection()
    {
        string direction = GetClosestDirection(gameManager.pacman.transform.position);
        movementController.setDirection(direction);
    }

    // Determine the direction of the pink ghost
    void DeterminePinkGhostDirection()
    {
        string pacmansDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distanceBetweenNodes = 0.3f;

        Vector2 target = gameManager.pacman.transform.position;
        if (pacmansDirection == "left")
        {
            target.x -= (distanceBetweenNodes * 2);
        }
        else if (pacmansDirection == "right")
        {
            target.x += (distanceBetweenNodes * 2);
        }
        else if (pacmansDirection == "up")
        {
            target.y =+ (distanceBetweenNodes * 2);
        }
        else if (pacmansDirection == "down")
        {
            target.y -= (distanceBetweenNodes * 2);
        }

        string direction = GetClosestDirection(target);
        movementController.setDirection(direction);
    }

    // Determine the direction of the blue ghost
    void DetermineBlueGhostDirection()
    {
        string pacmansDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distanceBetweenNodes = 0.3f;

        Vector2 target = gameManager.pacman.transform.position;
        if (pacmansDirection == "left")
        {
            target.x -= (distanceBetweenNodes * 2);
        }
        else if (pacmansDirection == "right")
        {
            target.x += (distanceBetweenNodes * 2);
        }
        else if (pacmansDirection == "up")
        {
            target.y =+ (distanceBetweenNodes * 2);
        }
        else if (pacmansDirection == "down")
        {
            target.y -= (distanceBetweenNodes * 2);
        }

        GameObject redGhost = gameManager.redGhost;
        float xDistance = target.x - redGhost.transform.position.x;
        float yDistance = target.y - redGhost.transform.position.y;

        Vector2 blueTarget = new Vector2(target.x + xDistance, target.y + yDistance);
        string direction = GetClosestDirection(blueTarget);
        movementController.setDirection(direction);
    }

    // Determine the direction of the orange ghost
    void DetermineOrangeGhostDirection()
    {

    }

    // Determines the ghost's closest direction to Pacman
    string GetClosestDirection(Vector2 target)
    {
        float shortestDistance = 0;
        string lastMovingDirection = movementController.lastMovingDirection;
        string newDirection = "";

        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();

        // If we can move up and we aren't reversing
        if (nodeController.canMoveUp && lastMovingDirection != "down")
        {
            // Get the node above us
            GameObject nodeUp = nodeController.nodeUp;
            // Get the distance between our top node and Pacman
            float distance = Vector2.Distance(nodeUp.transform.position, target);

            // If this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "up";
            }
        }

        if (nodeController.canMoveDown && lastMovingDirection != "up")
        {
            // Get the node below us
            GameObject nodeDown = nodeController.nodeDown;
            // Get the distance between our top node and Pacman
            float distance = Vector2.Distance(nodeDown.transform.position, target);

            // If this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "down";
            }
        }

        if (nodeController.canMoveLeft && lastMovingDirection != "right")
        {
            // Get the node to the left of us
            GameObject nodeLeft = nodeController.nodeLeft;
            // Get the distance between our top node and Pacman
            float distance = Vector2.Distance(nodeLeft.transform.position, target);

            // If this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "left";
            }
        }

        if (nodeController.canMoveRight && lastMovingDirection != "left")
        {
            // Get the node to the right of us
            GameObject nodeRight = nodeController.nodeRight;
            // Get the distance between our top node and Pacman
            float distance = Vector2.Distance(nodeRight.transform.position, target);

            // If this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "right";
            }
        }

        return newDirection;
    }
}
