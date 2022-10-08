using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CharacterController : MonoBehaviourPun
{
    [SerializeField] private bool _vertical;
    [SerializeField] private bool _flip;
    [SerializeField] private SpawnPoint _spawnPoint;
    [SerializeField] private CharacterWallOfDeath _wallOfDeath; 
    
    private CharacterModel _model;

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            Destroy(this);
            return;
        }
        _model = GetComponent<CharacterModel>();
        _wallOfDeath.OnBallEnter += Die;
    }

    private void Update()
    {

        var inputDirection = _vertical ? Input.GetAxisRaw("Vertical") : Input.GetAxisRaw("Horizontal");
        inputDirection *= _flip ? -1 : 1;
        _model.Move(new Vector2(inputDirection, 0));
        // _model.Move(vertical ? new Vector2(0, inputDirection) : new Vector2(inputDirection, 0), vertical);

    }

    public void SetSpawnPoint(SpawnPoint spawnPoint)
    {
        _spawnPoint = spawnPoint;
    }
    
    public void SetFlip(bool flip)
    {
        _flip = flip;
    }

    public void SetVertical(bool vertical)
    {
        _vertical = vertical;
        _model.Init(_vertical);
    }

    public void SetCharaterInformation(CharacterUI characterInfo)
    {
        _model.SetCharacterInformation(characterInfo);
    }

    private void Die()
    {
        _spawnPoint.SetOccupied(false);
        _model.Die();
    }

}
