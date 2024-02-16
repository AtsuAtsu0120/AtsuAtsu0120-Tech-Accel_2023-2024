using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour, IIntaract
{
    [SerializeField] private string message;
    public void OnIntaract()
    {
        Debug.Log(message);
        Destroy(gameObject);
    }

    public string InteractName { get; } = "Test";
}
