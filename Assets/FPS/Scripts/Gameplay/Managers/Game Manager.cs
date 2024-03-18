using System.Collections;
using System.Collections.Generic;
using PaintWars.FPS.DataStructures;
using TMPro;
using UnityEngine;

namespace PaintWars.FPS.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [Tooltip("Total number of enemies to spawn")]
        public int TotalEnemyCount;
        [Tooltip("Number of enemies to spawn at once")]
        public int EnemiesAtOnce;
        [Tooltip("Maximum Z value where enemies can spawn")]
        public float MaxZ;
        [Tooltip("Minimum Z value where enemies can spawn")]
        public float MinZ;
        [Tooltip("Maximum X value where enemies can spawn")]
        public float MaxX;
        [Tooltip("Minimum X value where enemies can spawn")]
        public float MinX;
        [Tooltip("Number of enemy 1's to spawn")]
        public int Enemy1Count;
        [Tooltip("Number of enemy 2's to spawn")]
        public int Enemy2Count;
        [Tooltip("Enemy 1 prefab")]
        public Enemy enemy1;
        [Tooltip("Enemy 2 prefab")]
        public Enemy enemy2;
        [Tooltip("Text to display if the player wins")]

        public TextMeshProUGUI WinText;
        public TextMeshProUGUI EnemiesDefeated;
        public TextMeshProUGUI LoseText;

        CustomQueue<Vector3> enemyPositionQueue;

        int currentEnemyCount = 0;
        PlayerCharacterController m_Player;


        float groundYVal = 1.98f;
        // Start is called before the first frame update
        void Start()
        {
            WinText.enabled = false;
            LoseText.enabled = false;
            enemyPositionQueue = new CustomQueue<Vector3>();
            for (int i = 0; i < TotalEnemyCount; i++)
            {
                Vector3 newPos = new Vector3(UnityEngine.Random.Range(MinX, MaxX), groundYVal, UnityEngine.Random.Range(MinZ, MaxZ));
                enemyPositionQueue.Enqueue(newPos);
            }
            m_Player = FindObjectOfType<PlayerCharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log(currentEnemyCount);
            Debug.Log(enemyPositionQueue.isEmpty());
            while (currentEnemyCount < EnemiesAtOnce)
            {
                if (!enemyPositionQueue.isEmpty())
                {
                    Debug.Log("tried instaniating");
                    Vector3 pos = enemyPositionQueue.Dequeue();
                    Enemy instance;
                    if (TotalEnemyCount - enemyPositionQueue.size() < Enemy1Count)
                    {
                        instance = Instantiate(enemy1, pos, Quaternion.identity);

                    }
                    else
                    {
                        instance = Instantiate(enemy2, pos, Quaternion.identity);

                    }
                    instance.m_Health.OnDie += OnEnemyDie;
                    currentEnemyCount++;
                }
                else
                {
                    break;
                }
            }
            EnemiesDefeated.text = "Enemies Defeated: " + (TotalEnemyCount - (enemyPositionQueue.size() + currentEnemyCount));

            if (currentEnemyCount == 0)
            {
                DisplayWinText();
            }
            if (m_Player.isDead)
            {
                DisplayLoseText();
            }
        }

        void OnEnemyDie()
        {
            currentEnemyCount--;
        }
        void DisplayWinText()
        {
            WinText.enabled = true;
        }
        void DisplayLoseText()
        {
            LoseText.enabled = true;
        }
    }

}

