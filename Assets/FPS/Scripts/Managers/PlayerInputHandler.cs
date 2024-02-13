using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintWars.FPS.Game;
namespace PaintWars.FPS.Gameplay
{
    public class PlayerInputHandler : MonoBehaviour
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

        /// <summary>
        /// Gets the horizontal and vertical (forward and backward) inputs for the player
        /// </summary>
        /// <returns>A Vector3 representing the move, clamped to a magnitude of 1.</returns>
        public Vector3 GetMoveInput()
        {

            if (CanProcessInput())
            {
                Vector3 move = new Vector3(Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal), 0, Input.GetAxisRaw(GameConstants.k_AxisNameVertical));
                move = Vector3.ClampMagnitude(move, 1);
                return move;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// Gets the horizontal input from the player's mouse.
        /// </summary>
        /// <returns>Float of the horizontal delta of the mouse, representing horizontal input. </returns>
        public float GetLookInputsHorizontal()
        {
            return GetMouseLookAxis(GameConstants.k_MouseAxisNameHorizontal) * (InvertYAxis ? -1 : 1);
        }

        /// <summary>
        /// Gets the vertical input from the player's mouse.
        /// </summary>
        /// <returns>Float of the vertical delta of the mouse, representing vertical input.</returns>
        public float GetLookInputsVertical()
        {
            return GetMouseLookAxis(GameConstants.k_MouseAxisNameVertical) * (InvertXAxis ? -1 : 1);
        }

        /// <summary>
        /// Gets the mouse delta on the provided axis.
        /// </summary>
        /// <param name="mouseInputName">Desired axis to measure.</param>
        /// <returns>The delta of the mouse on the given axis. A positive value means the mouse is moving 
        /// right/down, while a negative value means the mouse is moving left/up. Returns 0 if the handler
        /// cannot process input. </returns>
        private float GetMouseLookAxis(string mouseInputName)
        {
            if (CanProcessInput())
            {
                float input = Input.GetAxisRaw(mouseInputName);
                if (InvertYAxis)
                {
                    input *= -1f;
                }
                input *= LookSensitivity;
                return input;
            }
            return 0f;
        }

        public bool GetSprintInputHeld()
        {
            if (CanProcessInput())
            {
                return Input.GetButton(GameConstants.k_ButtonNameSprint);
            }

            return false;
        }
    }

}
