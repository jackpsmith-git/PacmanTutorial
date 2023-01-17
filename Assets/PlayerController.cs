using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    MovementController movementController;

    // Start is called before the first frame update
    void Start()
    {
        movementController = GetComponent<MovementController>();
    }

    // Update is called once per frame
    void Update()
    {
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
    }
}
