using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControl : MonoBehaviour
{
    public PlayerSphere playerSphere;
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.deltaPosition.y > 0f)
            {
                playerSphere.TryToMove("up");
            }
            if (touch.deltaPosition.y < 0f)
            {
                playerSphere.TryToMove("down");
            }
            if (touch.deltaPosition.x > 0f)
            {
                playerSphere.TryToMove("right");
            }
            if (touch.deltaPosition.x < 0f)
            {
                playerSphere.TryToMove("left");
            }
        }
    }
}