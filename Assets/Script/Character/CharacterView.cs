using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CharacterView : MonoBehaviourPun
{
    [SerializeField] private CharacterModel _characterModel;
    [SerializeField] private CharacterUI _characterUI;

    private void Start()
    {
        if (!photonView.IsMine) return;
        _characterModel.OnUpdateScore += UpdateScore;
    }

    private void UpdateScore()
    {
        _characterUI.SetScore(_characterModel.Score.ToString());
    }

    public void SetCharacterUI(CharacterUI characterUI)
    {
        _characterUI = characterUI;
    }
}
