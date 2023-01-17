using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    MovementController movementController;

    public SpriteRenderer sprite;
    public Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        movementController = GetComponent<MovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("moving", true);

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movementController.setDirection("left");
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            movementController.setDirection("right");
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            movementController.setDirection("up");
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            movementController.setDirection("down");
        }

        bool flipX = false;
        bool flipY = false;
        if (movementController.lastMovingDirection == "left")
        {
            animator.SetInteger("direction", 0);
        }
        else if (movementController.lastMovingDirection == "right")
        {
            animator.SetInteger("direction", 0);
            flipX = true;
        }
        else if (movementController.lastMovingDirection == "up")
        {
            animator.SetInteger("direction", 1);
        }
        else if (movementController.lastMovingDirection == "down")
        {
            animator.SetInteger("direction", 1);
            flipY = true;
        }

        sprite.flipY = flipY;
        sprite.flipX = flipX;
    }
}
