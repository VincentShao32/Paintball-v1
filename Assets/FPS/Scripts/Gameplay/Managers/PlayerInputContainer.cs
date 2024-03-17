using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintWars.FPS.Game;
using UnityEngine.InputSystem;
using System;

namespace PaintWars.FPS.Gameplay
{
    public class PlayerInputContainer : MonoBehaviour
    {
        [Tooltip("Sensitivity multiplier for moving the camera around")]
        public float LookSensitivity = 1f;

        [Tooltip("Additional sensitivity multiplier for WebGL")]
        public float WebglLookSensitivityMultiplier = 0.25f;

        [Tooltip("Used to flip the vertical input axis")]
        public bool InvertYAxis = false;

        [Tooltip("Used to flip the horizontal input axis")]
        public bool InvertXAxis = false;

        bool m_FireInputWasHeld;
        public Vector3 characterMovementTransform { get; set; }
        public Vector2 lookVector { get; set; }
        public bool jumped { get; set; }
        public bool shot { get; private set; }
        public bool shootHeld { get; private set; }
        public bool reload { get; private set; }
        public bool switchWeapon { get; private set; }

        [Tooltip("Jump force of the player")]
        public float jumpForce;

        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }

        // Update is called once per frame
        void Update()
        {

        }

        // LateUpdate is called once per frame after all other update functions
        void LateUpdate()
        {

        }

        /// <summary>
        /// Determines if the PlayerInputHandler can process input. 
        /// </summary>
        /// <returns>True if the handler can process input, false otherwise</returns>
        private bool CanProcessInput()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }

        /*
            INPUT SYSTEM ACTION METHODS
        */

        /// <summary>
        /// This is called from PlayerInput; when a joystick or arrow keys have been pushed
        /// Creates a new Vector3 with the input vector and stores it in characterMovementTransform. 
        /// </summary>
        /// <param name="context">The input value from PlayerInput. Supposed to be a Vector2 for movement.</param>
        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            characterMovementTransform = new Vector3(input.x, 0, input.y);
        }
        /// <summary>
        /// This is called form PlayerInput when the mouse moves. Stores the input vector in lookVector. 
        /// </summary>
        /// <param name="context">The input value from PlayerINput. Supposed to be a Vector2 for look.</param>
        public void OnLook(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            Console.WriteLine("Input detected");

            lookVector = new Vector2(input.x * (InvertXAxis ? -1 : 1), input.y * (InvertYAxis ? -1 : 1)) * LookSensitivity;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                characterMovementTransform += Vector3.up * jumpForce;
                Debug.Log("jumped");
                jumped = true;
            }
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
            if (context.action.IsPressed())
            {
                shootHeld = true;
            }
            else
            {
                shootHeld = false;
            }
            if (context.started)
            {
                shot = true;
            }
            else
            {
                shot = false;
            }
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                reload = true;
            }
        }

        public void OnSwitchWeapon(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                switchWeapon = true;
            }
            else
            {
                switchWeapon = false;
            }
        }
        public void resetReload()
        {
            reload = false;
        }
        public void resetShot()
        {
            shot = false;
        }

        public void resetJump()
        {
            jumped = false;
        }
    }

}