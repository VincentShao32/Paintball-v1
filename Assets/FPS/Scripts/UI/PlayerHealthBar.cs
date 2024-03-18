using PaintWars.FPS.Game;
using PaintWars.FPS.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class PlayerHealth : MonoBehaviour
    {
        [Tooltip("Text component displaying current health")]
        public TextMeshProUGUI currentHealth;

        Health m_PlayerHealth;

        void Start()
        {
            PlayerCharacterController playerCharacterController =
                GameObject.FindObjectOfType<PlayerCharacterController>();


            m_PlayerHealth = playerCharacterController.GetComponent<Health>();

        }

        void Update()
        {
            // update health bar value
            currentHealth.text = m_PlayerHealth.CurrentHealth.ToString();
        }
    }
}