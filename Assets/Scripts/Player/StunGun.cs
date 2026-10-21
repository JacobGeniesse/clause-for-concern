using UnityEngine;
using UnityEngine.InputSystem;

public class StunGun : MonoBehaviour
{
    public InputActionAsset masterList; // input list 
    private InputAction fire; // fire the gun
    [SerializeField] private StunProjectile projectilePrefab; // the bullet prefab
    [SerializeField] private Transform muzzle; // where bullets spawn from
    [SerializeField] private int projectileSpeed = 25; // how fast the bullet flies
    [SerializeField] private int stunDuration = 3; // how long enemies stay stunned
    [SerializeField] private int fireCooldown = 2; // seconds between shots (slow fire rate)
    [SerializeField] private int maxAmmo = 5; // most ammo you can hold
    [SerializeField] private int currentAmmo = 5; // ammo you start with

    private float nextFireTime = 0f; // the earliest time we are allowed to shoot again

    void Awake(){
        fire = masterList["Fire"]; // find the fire action before OnEnable runs
    }
    void OnEnable(){
        fire.Enable(); // turns on the fire action so it can send inputs
        fire.performed += OnFire; // calls onFire when the fire button is pressed 
    }
    void OnDisable(){
        fire.performed -= OnFire; // stop Onfire from being called on fire press
        fire.Disable(); // turns off the fire action so it cant send inputs
    }

    private void OnFire(InputAction.CallbackContext context){
        if (currentAmmo <= 0){
            return; // no ammo no shooting
        }
        if(Time.time < nextFireTime){
            return; // still cooling down no shooting
        }
        Shoot();
    }
    private void Shoot(){
        StunProjectile shot = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation); // spawns the bullets at the muzzle
    shot.Fire(muzzle.forward * projectileSpeed, stunDuration); // fires it where the muzzle is facing

        currentAmmo--; // uses one ammo
        nextFireTime = Time.time + fireCooldown; // sets when we can shoot next
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo); // adds ammo without going over max
    }
}
