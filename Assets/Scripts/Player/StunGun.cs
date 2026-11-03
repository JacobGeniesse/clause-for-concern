using UnityEngine;
using UnityEngine.InputSystem;

public class StunGun : MonoBehaviour
{
    public InputActionAsset masterList; // input list 
    private InputAction fire; // fire the gun

    [Tooltip("Bullet Prefab")]
    [SerializeField] private StunProjectile projectilePrefab; // the bullet prefab

    [Tooltip("Position Bullets Spawn From.")]
    [SerializeField] private Transform muzzle; // where bullets spawn from

    [Tooltip("Speed the bullet will be launched at.")]
    [SerializeField] private int projectileSpeed = 25; // how fast the bullet flies
    private int stunDuration = 3; // how long enemies stay stunned

    [Tooltip("Length of time between firing.")]
    [SerializeField] private int fireCooldown = 2; // seconds between shots (slow fire rate)
    PlayerInventory inventory; //Ref to the player inventory

    private float nextFireTime = 0f; // the earliest time we are allowed to shoot again

    void Awake(){
        fire = masterList["Fire"]; // find the fire action before OnEnable runs
        inventory = GetComponentInParent<PlayerInventory>();
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
        if (inventory.GetCount(ItemType.Ammo) > 0){
        if (Minimap.IsOpen)
        {
            return;
        }

        if (inventory.ammoCount <= 0){
            return; // no ammo no shooting
        }
        if(Time.time < nextFireTime){
            return; // still cooling down no shooting
        }
        Shoot();
            Debug.Log("firing");
        }
    }
    private void Shoot()
    {
        StunProjectile shot = null;
        if (ObjectPool.instance != null)
        {
            GameObject staple = ObjectPool.instance.Create(projectilePrefab.gameObject, muzzle.position, muzzle.rotation);
            shot = staple.GetComponent<StunProjectile>();
        }
        else
        {
            Debug.LogWarning("No ObjectPool in scene! Add one to boost performance!");
            shot = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation); // spawns the bullets at the muzzle
        }
        shot.Fire(muzzle.forward * projectileSpeed, stunDuration); // fires it where the muzzle is facing

        inventory.RemoveItem(ItemType.Ammo, 1); // uses one ammo
        nextFireTime = Time.time + fireCooldown; // sets when we can shoot next
    }
}
