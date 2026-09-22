using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public InputActionAsset MasterList;

    public List<InputAction> Inputs = new List<InputAction>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(InputAction action in MasterList)
        {
           Inputs.Add(action);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
