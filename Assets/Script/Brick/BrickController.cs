using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickController : MonoBehaviour
{
    [SerializeField] private BrickModel _brickModel;
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        var ball = col.gameObject.GetComponent<BallModel>();
        if (ball != null)
        {
            _brickModel.Damage(1); // Change to be the ball having a damage?
        }
    }
}
