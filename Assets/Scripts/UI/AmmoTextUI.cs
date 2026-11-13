using UnityEngine;
using TMPro;
public class AmmoTextUI : MonoBehaviour
{
    [SerializeField] TMP_Text m_Text;
    [SerializeField] PlayerInventory playerInventory;

    private void OnEnable()
    {
       if(playerInventory == null)
        {
            Debug.Log("Ammo UI has no inventory assigned.");
            return;
        }
        playerInventory.OnAmmoChanged += UpdateAmmoText; // subscribe ammochange to the method update ammo 
        UpdateAmmoText(playerInventory.GetCount(ItemType.Ammo)); // get the ammount in the update ammo method
    }

    private void OnDisable()
    {
        if (playerInventory == null)
        {
            Debug.Log("Ammo UI has no inventory assigned.");
            return;
        }
        playerInventory.OnAmmoChanged -= UpdateAmmoText;
    }

    public void UpdateAmmoText(int ammoCount)
    {
        m_Text.text = ammoCount.ToString() + "/" + playerInventory.maxAmmo;
    }
}
