/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections;
using UnityEngine;

/// <summary>
/// Listens to the user's input, moves and animates Oppy accordingly.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class OppyCharacterController : MonoBehaviour
{
    /// <summary>
    /// The transform whose the forward vector for Oppy's motion
    /// </summary>
    [SerializeField] private Transform movementFrameOfReference;

    /// <summary>
    ///  The vertical speed that Oppy will have if the jump button is pressed
    /// </summary>
    [SerializeField] private float jumpSpeed = 4;

    /// <summary>
    ///  The vertical acceleration applied to Oppy if the jump button is kept pressed
    /// </summary>
    [SerializeField] private float keepPressedJumpAcceleration = 1;

    [SerializeField] private OVRInput.Button jumpButton;

    /// <summary>
    ///  The transform in front of which Oppy will be respawned
    /// </summary>
    [SerializeField] private Transform respawnTransform;

    [SerializeField] private float maximumLinearSpeed = 0.9f;
    [SerializeField] private float gravity = -9.8f;

    private Animator _animator;
    private CharacterController _characterController;

    private Vector3 _moveVelocity;
    private Quaternion _rotation;
    private Vector2 _motionInput;
    private bool _jumpRequested;
    private JumpingState _jumpingState = JumpingState.Grounded;

    private const float JumpDelay = 0.16f;

    private enum JumpingState
    {
        Grounded,
        JumpStarted,
        JumpedAndAirborne
    }

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        GetLocomotionInput();
        HandleLocomotion();
        HandleJumping();
        ApplyMotion();
    }

    public void Respawn()
    {
        if (_characterController == null)
        {
            _characterController = GetComponent<CharacterController>();
        }

        _characterController.enabled = false;
        transform.position = respawnTransform.position + respawnTransform.forward * 0.3f;
        _characterController.enabled = true;
    }

    private void GetLocomotionInput()
    {
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");
        Vector2 thumbstickAxis = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);
        _motionInput = new Vector2(hInput + thumbstickAxis.x, vInput + thumbstickAxis.y);
    }

    private void ApplyMotion()
    {
        _moveVelocity.y += gravity * Time.deltaTime;
        _characterController.Move(_moveVelocity * Time.deltaTime);
        if (Mathf.Abs(_motionInput.y) > 0 || Mathf.Abs(_motionInput.x) > 0)
        {
            transform.rotation = _rotation;
        }
    }

    private void HandleLocomotion()
    {
        bool noMovementInput = Mathf.Abs(_motionInput.y) == 0 && Mathf.Abs(_motionInput.x) == 0;
        _animator.SetBool("Running", !noMovementInput && _characterController.isGrounded);

        Vector3 motionForwardDirection =
            Vector3.ProjectOnPlane(movementFrameOfReference.forward, Vector3.up).normalized;
        Vector3 motionRightDirection = Vector3.ProjectOnPlane(movementFrameOfReference.right, Vector3.up).normalized;
        Vector3 motionDirection = (motionForwardDirection * _motionInput.y + motionRightDirection * _motionInput.x)
            .normalized;
        _rotation = transform.rotation;

        if (_characterController.isGrounded)
        {
            _moveVelocity = motionDirection * maximumLinearSpeed;
            Vector3 lerpedMoveDirection = Vector3.Lerp(transform.forward, motionDirection, 0.6f);
            _rotation = Quaternion.LookRotation(lerpedMoveDirection);
        }
    }

    private void HandleJumping()
    {
        bool jumpButtonDown = OVRInput.GetDown(jumpButton) || Input.GetButtonDown("Jump");
        bool jumpButtonPressed = OVRInput.Get(jumpButton) || Input.GetButton("Jump");

        if (_jumpRequested)
        {
            _moveVelocity.y = jumpSpeed;
            _jumpRequested = false;
        }

        if (_jumpingState == JumpingState.JumpStarted && !_characterController.isGrounded)
        {
            _jumpingState = JumpingState.JumpedAndAirborne;
        }

        if (_jumpingState != JumpingState.Grounded && jumpButtonPressed)
        {
            _moveVelocity.y += keepPressedJumpAcceleration * Time.deltaTime;
        }

        if (_jumpingState == JumpingState.Grounded && _characterController.isGrounded && jumpButtonDown)
        {
            _jumpingState = JumpingState.JumpStarted;
            StartCoroutine(RequestJumpAfterSeconds(JumpDelay));
            _animator.SetTrigger("Jumping");
        }
        else if (_characterController.isGrounded && _jumpingState == JumpingState.JumpedAndAirborne)
        {
            _animator.SetTrigger("Landed");
            _jumpingState = JumpingState.Grounded;
        }
    }

    private IEnumerator RequestJumpAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        _jumpRequested = true;
    }
}
