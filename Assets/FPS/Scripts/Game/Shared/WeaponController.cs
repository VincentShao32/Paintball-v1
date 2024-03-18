using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PaintWars.FPS.Game
{
    public enum WeaponShootType
    {
        Manual,
        Automatic
    }

    [System.Serializable]
    public struct CrosshairData
    {
        [Tooltip("The image that will be used for this weapon's crosshair")]
        public Sprite CrosshairSprite;

        [Tooltip("The size of the crosshair image")]
        public int CrosshairSize;

        [Tooltip("The color of the crosshair image")]
        public Color Crosshair;
    }
    public class WeaponController : MonoBehaviour
    {

        [Header("Internal References")]
        [Tooltip("The root object for the weapon, this is what will be deactivated when the weapon isn't active")]
        public GameObject WeaponRoot;

        [Tooltip("Tip of the weapon, where the projectiles are shot")]
        public Transform WeaponMuzzle;

        [Header("Shoot Parameters")]
        [Tooltip("The type of weapon wil affect how it shoots")]
        public WeaponShootType ShootType;

        [Tooltip("The projectile prefab")]
        public ProjectileBase ProjectilePrefab;

        [Tooltip("Minimum duration between two shots")]
        public float DelayBetweenShots = 0.5f;

        [Tooltip("Angle for the cone in which the bullets will be shot randomly (0 means no spread at all)")]
        public float BulletSpreadAngle = 0f;

        [Tooltip("Amount of bullets per shot")]
        public int BulletsPerShot = 1;

        [Tooltip("Force that will push back the weapon after each shot")]
        [Range(0f, 2f)]
        public float RecoilForce = 1;

        [Tooltip("Ratio of the default FOV that this weapon applies while aiming")]
        [Range(0f, 1f)]
        public float AimZoomRatio = 1f;

        [Tooltip("Translation to apply to weapon arm when aiming with this weapon")]
        public Vector3 AimOffset;

        [Header("Ammo Parameters")]
        [Tooltip("Should the player manually reload")]
        public bool AutomaticReload = true;

        [Tooltip("Number of bullets in a clip")]
        public int ClipSize = 30;

        [Tooltip("Amount of ammo reloaded per second")]
        public float AmmoReloadRate = 1f;

        [Tooltip("Delay after the last shot before starting to reload")]
        public float AmmoReloadDelay = 2f;

        [Tooltip("Maximum amount of ammo in the gun")]
        public int MaxAmmo = 8;
        bool m_WantsToShoot = false;

        float m_CurrentAmmo;
        Vector3 m_LastMuzzlePosition;

        public GameObject Owner { get; set; }
        public GameObject SourcePrefab { get; set; }

        public float m_LastTimeShot = -5.0f;

        public int GetCurrentAmmo() => Mathf.FloorToInt(m_CurrentAmmo);

        public bool IsReloading { get; private set; }

        public float CurrentAmmoRatio { get; private set; }
        public bool IsWeaponActive { get; private set; }
        public Vector3 MuzzleWorldVelocity { get; private set; }


        void Awake()
        {
            m_CurrentAmmo = MaxAmmo;
            m_LastMuzzlePosition = WeaponMuzzle.position;
            m_LastTimeShot = -5;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_LastTimeShot = -5;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateAmmo();
            if (Time.deltaTime > 0)
            {
                MuzzleWorldVelocity = (WeaponMuzzle.position - m_LastMuzzlePosition) / Time.deltaTime;
                m_LastMuzzlePosition = WeaponMuzzle.position;
            }
        }

        public bool HandleShootInputs(bool inputDown, bool inputHeld)
        {
            m_WantsToShoot = inputDown || inputHeld;
            switch (ShootType)
            {
                case WeaponShootType.Manual:
                    if (inputDown)
                    {
                        return TryShoot();
                    }
                    return false;
                case WeaponShootType.Automatic:
                    if (inputHeld)
                    {
                        return TryShoot();
                    }
                    return false;
                default:
                    return false;
            }
        }

        void UpdateAmmo()
        {
            if (AutomaticReload && m_LastTimeShot + AmmoReloadDelay < Time.time && m_CurrentAmmo < MaxAmmo)
            {
                m_CurrentAmmo += AmmoReloadRate * Time.deltaTime;

                m_CurrentAmmo = Mathf.Clamp(m_CurrentAmmo, 0, MaxAmmo);


            }

            if (MaxAmmo == Mathf.Infinity)
            {
                CurrentAmmoRatio = 1f;
            }
            else
            {
                CurrentAmmoRatio = m_CurrentAmmo / MaxAmmo;
            }
        }

        public void ShowWeapon(bool show)
        {
            WeaponRoot.SetActive(show);

            IsWeaponActive = show;
        }



        public bool TryShoot()
        {
            if (m_CurrentAmmo >= 1f && m_LastTimeShot + DelayBetweenShots < Time.time)
            {
                HandleShoot();
                m_CurrentAmmo -= 1f;
                return true;
            }

            return false;
        }

        public void EnemyShoot()
        {
            if (m_LastTimeShot > Time.time)
            {
                m_LastTimeShot = Time.time;
            }
            if (m_LastTimeShot + DelayBetweenShots < Time.time)
            {
                HandleShoot();
                Debug.Log("Enemy shot");
            }
        }

        void HandleShoot()
        {
            for (int i = 0; i < BulletsPerShot; i++)
            {
                Vector3 shotDirection = GetShotDirectionWithinSpread(WeaponMuzzle);
                ProjectileBase newProjectile = Instantiate(ProjectilePrefab, WeaponMuzzle.position,
                    Quaternion.LookRotation(shotDirection));
                newProjectile.Shoot(this);
            }
            m_LastTimeShot = Time.time;
        }

        public Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
        {
            float spreadAngleRatio = BulletSpreadAngle / 180f;
            Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,
                spreadAngleRatio);

            return spreadWorldDirection;
        }

        void Reload()
        {

        }
    }

}
