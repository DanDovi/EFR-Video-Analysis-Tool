using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseJump : MonoBehaviour {
    private float jumpForce = 20.0f;

    public bool canJump = false;

    private void Update () {       
        if((Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.B)) && canJump)
        {
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.0f, jumpForce));
        }
	}
}
