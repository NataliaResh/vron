using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool isLowHealth = false;
    public bool isLowSatiety = false;

    private void Update()
    {
        if (isLowHealth)
        {
            Debug.Log("Low health");
        }

        if (isLowSatiety)
        {
            Debug.Log("Low satiety");
        }
    }
}
