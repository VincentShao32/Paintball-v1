using System.Collections;
using System.Collections.Generic;
using PaintWars.FPS.Game;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("Weapon used by enemies")]
    public WeaponController m_Weapon;

    PlayerCharacterController m_PlayerCharacterController;
    public Health m_Health { get; set; }
    public Vector3 enemyToPlayerShotDirection { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        m_PlayerCharacterController = FindObjectOfType<PlayerCharacterController>();
        m_Health = GetComponent<Health>();
        m_Health.OnDie += OnDie;
        m_Weapon.Owner = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateShotDirection();
        transform.rotation = Quaternion.LookRotation(enemyToPlayerShotDirection);
        m_Weapon.EnemyShoot();
        Debug.Log("Current enemy health: " + m_Health.CurrentHealth);
    }

    void CalculateShotDirection()
    {
        enemyToPlayerShotDirection = m_PlayerCharacterController.transform.position - transform.position;
    }

    void OnDie()
    {
        Destroy(this.gameObject);
    }
}
