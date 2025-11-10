using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Walk,
    Jump,
    Run
}

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Animator _animator;
    //private PlayerState _playerState;

    [Header("Camera Settings, 1 = 1인팅, 그 외 = 3인칭")] 
    public Transform playerCamera;
    [SerializeField] private float cameraSensitivity = 1;
    [SerializeField]private float maxXLook = 85;
    [SerializeField]private float minXLook = -85;
    private float pitch;
    
    [Header("Move")]
    [SerializeField] private float _moveSpeed = 5f; //이동속도
    //[SerializeField] private float _turnSpeed = 5f; // 회전속도
    private Vector2 walkInput;
    private Vector2 mouseInput;
    private Vector3 moveLocal;
    private Vector3 moveWorld;
    
    [Header("Jump")]
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private LayerMask groundLayerMask;
    //private float _jumpTimer;
    private bool canJump;
    private bool isGrounded;
    
    [Header("Climb")]
    [SerializeField] private LayerMask climbLayerMask;

    private bool isClimb;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null) Debug.Log("no rigidbody");
        
        _animator = GetComponent<Animator>();
        if (_animator == null) Debug.Log("no animator");
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.InventoryUIOpen) return;
        
        LookInput();
        
        WalkInput();
        
        JumpInput();
        
        ClimbInput();
    }

    private void FixedUpdate()
    {
        Move();
        Jump();
        Climb();
    }

    
    
    
    private void LookInput()
    { 
        //시야 입력
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        
        //좌우 시야
        transform.Rotate(0f, mouseInput.x * cameraSensitivity, 0f);
        
        //상화 시야
        pitch -= mouseInput.y * cameraSensitivity;              
        pitch = Mathf.Clamp(pitch, minXLook, maxXLook);
        playerCamera.localRotation = Quaternion.Euler(pitch, 0, 0);

        //애니메이션 판정
        bool isMoving = walkInput.sqrMagnitude > 0.01f;
        if (_animator) _animator.SetBool("Walk", isMoving);
    }

    private void WalkInput()
    {
        //움직입 입력
        walkInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void JumpInput()
    {
        if(isClimb) return; 
        //점프키를 입력받고
        bool jumpInput = Input.GetButtonDown("Jump");
        //Debug.Log("jumpInput: " + jumpInput);
        
        //지면 검사
        isGrounded = IsGrounded();
        //Debug.Log("grounded: " + isGrounded);

        //검사
        if (jumpInput && isGrounded) canJump = true;
    }

    private void ClimbInput()
    {

        if (CanClimb() && Input.GetMouseButton(0))
        {
            isClimb = true;
        }
        else isClimb = false;
        //canClimb && player input시 y축 좌표값 추가
        //벽타기 중에 점프 불가 -> 점프 매서드에 추가
    }

    private bool IsGrounded()
    {
        Ray[] ray = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < ray.Length; i++)
        {
            if(Physics.Raycast(ray[i],0.9f, groundLayerMask)) return true;
        }
        
        return false;
    }

    private bool CanClimb()
    {
        Ray ray = 
             new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.forward);
        bool canClimb = Physics.Raycast(ray, 1f, climbLayerMask);
        
        return canClimb;

        //raycast로 벽 검사 && 접촉 여부 검사
    }

    private void Move()
    {
        moveLocal = new Vector3(walkInput.x, 0f, walkInput.y);
        moveWorld = transform.TransformDirection(moveLocal);
        
        Vector3 velocity = _rigidbody.velocity;
        Vector3 horizontal = moveWorld*_moveSpeed;
        
        _rigidbody.velocity = new Vector3(horizontal.x, velocity.y, horizontal.z);
    }

    private void Jump()
    {
        if (canJump)
        {
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
        canJump = false;
    }

    private void Climb()
    {
        if (isClimb)
        {
            Vector3 climbPos = transform.position + Vector3.up * _moveSpeed * Time.deltaTime;
            transform.position = climbPos;
        }
    }
    

}
