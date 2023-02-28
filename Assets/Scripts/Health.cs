using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100;
    [SerializeField] private float _deltaHealth = 1;
    [SerializeField] private float _limitHealth = 10;
    [SerializeField] private GameObject _gameControllerObject;
    [SerializeField] private GameObject _player;

    private GameController _gameController;
    private PlayerController _playerController;

    private float _currentHealth;
    private float _currentDeltaHealth;
    private void Start()
    {
        _currentHealth = _maxHealth;
        _currentDeltaHealth = _deltaHealth;
        if (_gameControllerObject.TryGetComponent(out GameController gameController) == false) {
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
        if (_currentHealth <= 0)
        {
            _gameController.GameOver();
        }
        else if (_currentHealth <= _limitHealth)
        {
            _playerController.isLowSatiety = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Water water))
        {
            _currentHealth -= _currentDeltaHealth;
            if (_currentHealth <= 0) {
                _gameController.GameOver();
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Water water))
        {
            _currentDeltaHealth = _deltaHealth;
        }
    }
}
