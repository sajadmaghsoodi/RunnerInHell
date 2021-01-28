using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Animator _animator;
    [Space]
    [Space]
    [SerializeField] private float _normalSpeed = 12f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _jumpHeight = 3f;
    [SerializeField] private float _normalHeight = 3.4f;
    [SerializeField] private float _crouchHeight = 1.4f;
    [SerializeField] private float _crouchSpeedAdded = 5f;
    [SerializeField] private float _crouchTime = 4f;
    [Space]
    [Space]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Transform[] _wallCheck;
    [SerializeField] private LayerMask _wallMask;
    [SerializeField] private float _groundDistance;
    [SerializeField] private LayerMask _groundMask;

    bool _isGrounded;
    bool _isCrouched;
    bool _isWallWalking;
    float speed;
    Vector3 _velocity;
    Coroutine _crouchCoroutine;
    private void Start()
    {
        speed = _normalSpeed;
    }

    void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
        if(_isWallWalking)
        {
           _velocity.y = 0f;
        }
        else if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        else
        {

        }

        if (_isGrounded && !_isCrouched)
        {
            _animator.SetBool("Running", true);
        }
        else
        {
            _animator.SetBool("Running", false);
        }

        float x = Input.GetAxis("Horizontal");
        float z = 1;

        Vector3 move = transform.right * x + transform.forward * z;
        _controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && (_isGrounded || _isWallWalking))
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && _isGrounded)
        {
            if (_crouchCoroutine != null)
                StopCoroutine(_crouchCoroutine);

            _crouchCoroutine = StartCoroutine(CrouchCoroutine());
        }

        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }




    IEnumerator CrouchCoroutine()
    {
        //_animator.SetBool("Crouch", true);
        yield return new WaitForSeconds(0.2f);
        _isCrouched = true;
        float timer = 0;
        float transition = _crouchTime / 15;
        while (timer < transition)
        {
            timer += Time.deltaTime;
            _controller.height = Mathf.Lerp(_normalHeight, _crouchHeight, timer / transition);
            speed = _normalSpeed + (_crouchSpeedAdded * (timer / transition));
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds((timer / 15) * 13);

        timer = 0;
        while (timer < transition)
        {
            timer += Time.deltaTime;
            _controller.height = Mathf.Lerp(_crouchHeight, _normalHeight, timer / transition);
            speed = (_normalSpeed + _crouchSpeedAdded) - (_crouchSpeedAdded * (timer / transition));
            yield return new WaitForEndOfFrame();
        }
        _isCrouched = false;
       // _animator.SetBool("Crouch", false);
    }
}
