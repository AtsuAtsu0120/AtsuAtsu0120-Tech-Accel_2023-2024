using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIntaract
{
    /// <summary>
    /// インタラクトしたときの動作
    /// </summary>
    public void OnIntaract();
    public string InteractName { get; }
}
