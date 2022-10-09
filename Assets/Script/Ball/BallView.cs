using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BallView : MonoBehaviourPun
{
    [SerializeField] private Animator _animator;
    [SerializeField] private BallModel _model;
    
    private static readonly int Spawning = Animator.StringToHash("Spawning");

    private void Start()
    {
        if (!photonView.IsMine) Destroy(this);
        _model.OnStopSpawning += UpdateSpawning;
        _animator.SetBool(Spawning, true);
    }

    private void UpdateSpawning()
    {
        if (!photonView.IsMine) return;
        _animator.SetBool(Spawning, false);
    }
}
