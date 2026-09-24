using UnityEngine;
using UnityEngine.InputSystem;

public class DevCheats : MonoBehaviour
{
    [SerializeField] private InputActionAsset masterList;
    private InputAction cheat;

    public bool cheating;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cheat = masterList["EnemyPassive"];
    }

    // Update is called once per frame
    void Update()
    {
        if (cheat.WasPressedThisFrame())
        {
            cheating = true;
        }
    }
}
