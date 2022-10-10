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
    [SerializeField] private Color _color = Color.cyan;

    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        if (!photonView.IsMine) return;

        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _spriteRenderer.color = _color;

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
