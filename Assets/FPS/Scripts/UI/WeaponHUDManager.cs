using System.Collections.Generic;
using PaintWars.FPS.Game;
using PaintWars.FPS.Gameplay;
using UnityEngine;

namespace PaintWars.FPS.UI
{
    public class WeaponHUDManager : MonoBehaviour
    {
        [Tooltip("UI panel containing the layoutGroup for displaying weapon ammo")]
        public RectTransform AmmoPanel;

        // [Tooltip("Prefab for displaying weapon ammo")]
        // public GameObject AmmoCounterPrefab;

        PlayerWeaponsManager m_PlayerWeaponsManager;
        List<AmmoCounter> m_AmmoCounters = new List<AmmoCounter>();

        void Start()
        {
            m_PlayerWeaponsManager = FindObjectOfType<PlayerWeaponsManager>();

            WeaponController activeWeapon = m_PlayerWeaponsManager.GetActiveWeapon();
            if (activeWeapon)
            {
                //AddWeapon(activeWeapon, m_PlayerWeaponsManager.ActiveWeaponIndex);
                //ChangeWeapon(activeWeapon);
            }

            // m_PlayerWeaponsManager.OnAddedWeapon += AddWeapon;
            m_PlayerWeaponsManager.OnSwitchedToWeapon += ChangeWeapon;
        }

        // void AddWeapon(WeaponController newWeapon, int weaponIndex)
        // {
        //     GameObject ammoCounterInstance = Instantiate(AmmoCounterPrefab, AmmoPanel);
        //     AmmoCounter newAmmoCounter = ammoCounterInstance.GetComponent<AmmoCounter>();

        //     newAmmoCounter.Initialize(newWeapon, weaponIndex);

        //     m_AmmoCounters.Add(newAmmoCounter);
        // }

        void ChangeWeapon(WeaponController weapon)
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(AmmoPanel);
        }
    }
}