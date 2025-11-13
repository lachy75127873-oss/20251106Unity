using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
    [Header("점프 힘")]
    public float jumpForce = 20f;   // 원하는 만큼 높이
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider JumpingTarget)
    {
        // 이 콜라이더가 달려있는 객체의 "리짓바디 주인"을 잡아온다
        Rigidbody rb = JumpingTarget.attachedRigidbody;
        if (rb == null) return;                  // 리짓바디 없으면 패스
        if (!rb.CompareTag("Player")) return;    // 진짜 Player

            // 기존 위쪽 속도는 없애고 위로 쏘기
            Vector3 vel = rb.velocity;
            vel.y = 0f;             // 위로 날릴 때 기존 y속도 초기화(선택)
            rb.velocity = vel;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            if (anim != null)
            {
                anim.SetTrigger("JumpTrigger");
            }
    }
}