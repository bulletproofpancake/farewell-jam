using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FJ.Player
{
    public class ThirdPersonController : MonoBehaviour
    {
        // input fields
        private PlayerControls _playerControls;
        private InputAction _move;
        
        // movement fields
        private Rigidbody _rb;
        [SerializeField] private float movementForce = 1f;
        [SerializeField] private float maxSpeed = 5f;
        private Vector3 _forceDirection;

        [SerializeField] private Camera playerCamera;
        private Animator _animator;
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _playerControls = new PlayerControls();
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            _playerControls.Gameplay.Push.started += DoPush;
            _move = _playerControls.Gameplay.Move;
            _playerControls.Gameplay.Enable();
        }
        
        private void OnDisable()
        {
            _playerControls.Gameplay.Push.started -= DoPush;
            _playerControls.Gameplay.Disable();
        }

        private void FixedUpdate()
        {
            _forceDirection += _move.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * movementForce;
            _forceDirection += _move.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * movementForce;
            
            _rb.AddForce(_forceDirection, ForceMode.Impulse);
            _forceDirection = Vector3.zero;

            if (_rb.velocity.y < 0f)
                _rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

            Vector3 horizontalVelocity = _rb.velocity;
            horizontalVelocity.y = 0f;
            if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
                _rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * _rb.velocity.y;
            
            LookAt();
        }

        private void LookAt()
        {
            Vector3 direction = _rb.velocity;
            direction.y = 0f;
            
            if (_move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
                _rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
            else
                _rb.angularVelocity = Vector3.zero;
        }
        
        private Vector3 GetCameraForward(Camera playerCam)
        {
            Vector3 forward = playerCamera.transform.forward;
            forward.y = 0;
            return forward.normalized;
        }

        private Vector3 GetCameraRight(Camera playerCam)
        {
            Vector3 right = playerCamera.transform.right;
            right.y = 0;
            return right.normalized;
        }

        private void DoPush(InputAction.CallbackContext obj)
        {
            _animator.SetTrigger("attack");
        }
    }
}
