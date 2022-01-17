using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonTouchControl : MonoBehaviour
{
    public PlayerSphere playerSphere;
    private void Update()
    {
        if (Input.GetKey("w"))
        {
            playerSphere.TryToMove("up");
        }
        if (Input.GetKey("s"))
        {
            playerSphere.TryToMove("down");
        }
        if (Input.GetKey("d"))
        {
            playerSphere.TryToMove("right");
        }
        if (Input.GetKey("a"))
        {
            playerSphere.TryToMove("left");
        }
    }
}