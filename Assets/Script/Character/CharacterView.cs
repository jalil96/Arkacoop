using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private CharacterModel _characterModel;
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void Update()
    {
        _scoreText.text = _characterModel.Score.ToString();
    }
}
