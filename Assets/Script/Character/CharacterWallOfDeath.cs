using System;
using UnityEngine;


public class CharacterWallOfDeath : MonoBehaviour
{
    public Action OnBallEnter = delegate {  };
    
        
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        var ball = col.gameObject.GetComponent<BallModel>();
        if (ball != null)
        {
            OnBallEnter.Invoke();
        }
    }
    
}