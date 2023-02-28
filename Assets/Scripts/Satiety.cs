using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satiety : MonoBehaviour
{
    [SerializeField] private float _maxSatiety = 100;
    [SerializeField] private float _deltaSatiety = 0.1f;
    [SerializeField] private float _limitSatiety = 10;
    [SerializeField] private GameObject _gameControllerObject;
    [SerializeField] private GameObject _player;

    private GameController _gameController;
    private PlayerController _playerController;

    private float _currentSatiety;
    private void Start()
    {
        _currentSatiety = _maxSatiety;

        if (_gameControllerObject.TryGetComponent(out GameController gameController) == false)
        {
            Debug.LogError("Uncorrect game controller");
        }
        _gameController = gameController;

        if (_player.TryGetComponent(out PlayerController playerController) == false)
        {
            Debug.LogError("Player not specified");
        }
        _playerController = playerController;
    }

    private void FixedUpdate()
    {
        _currentSatiety -= _deltaSatiety * Time.deltaTime;
        if (_currentSatiety <= 0) {
            _gameController.GameOver();
        }
        else if (_currentSatiety <= _limitSatiety)
        {
            _playerController.isLowSatiety = true;
        }
    }
}
