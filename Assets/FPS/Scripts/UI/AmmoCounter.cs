using TMPro;
using PaintWars.FPS.Game;
using PaintWars.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace PaintWars.FPS.UI
{
    public class AmmoCounter : MonoBehaviour
    {


        [Tooltip("Text for Bullet Counter")]
        public TextMeshProUGUI BulletCounter;

        PlayerWeaponsManager m_PlayerWeaponsManager;
        WeaponController m_Weapon;

        void Awake()
        {

        }

        void Start()
        {
            m_PlayerWeaponsManager = FindObjectOfType<PlayerWeaponsManager>();

        }
        void Update()
        {
            m_Weapon = m_PlayerWeaponsManager.GetActiveWeapon();

            if (m_Weapon)
            {
                BulletCounter.text = m_Weapon.GetCurrentAmmo().ToString();
            }
        }
    }
}