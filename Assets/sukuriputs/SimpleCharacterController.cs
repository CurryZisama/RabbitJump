using UnityEngine;
using UnityEngine.InputSystem; // Input Systemパッケージが必要です
using System.Collections; // IEnumeratorを使うために必要
using System.Collections.Generic; // Listを使うために必要

// 必須コンポーネントとしてCharacterControllerとRigidbodyを指定
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class SimpleCharacterController : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("連打1回あたりの加速量（これを小さく、Maxを大きくすると連打ゲーになります）")]
    public float dashSpeed = 3.0f; // 加算式にするため、少し小さめの値を推奨
    [Tooltip("最高速度制限")]
    public float maxSpeed = 20.0f; // ★追加：これ以上は速くならない上限
    [Tooltip("ブレーキの強さ（値が大きいほどすぐ止まる）")]
    public float brakePower = 10.0f; // 減速も少し緩やかにして慣性を残す

    [Header("ジャンプ・重力")]
    public float jumpPower = 5.0f;
    public float gravity = 20.0f;

    [Header("プレイヤー設定")]
    [Tooltip("1:Qキー, 2:Rキー, 3:Uキー, 4:Pキー")]
    public int PlayerNumber = 1;

    [Header("スタン（停止）設定")]
    [Tooltip("障害物に当たった時の停止時間（秒）")]
    public float stunDuration = 1.0f;
    [Tooltip("スタン中に変化させる色")]
    public Color stunColor = Color.red;
    [Tooltip("点滅の間隔（秒）")]
    public float blinkInterval = 0.1f;

    [Header("アニメーション設定（任意）")]
    public Animator animator;

    // 内部変数
    private CharacterController characterController;
    private Rigidbody rb;
    private float currentBackwardSpeed = 0f; // 現在の後退速度
    private float verticalVelocity; // 重力・ジャンプ用

    // スタン状態（動けない状態）かどうか
    private bool isStunned = false;

    // 色変更・点滅用
    private Renderer[] myRenderers;
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();
    private readonly string[] colorPropertyNames = { "_Color", "_BaseColor" };

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // 自分のレンダラー（見た目）を取得して、元の色を覚えておく
        myRenderers = GetComponentsInChildren<Renderer>();
        foreach (var r in myRenderers)
        {
            if (r is ParticleSystemRenderer) continue;

            foreach (var propName in colorPropertyNames)
            {
                if (r.material.HasProperty(propName))
                {
                    if (!originalColors.ContainsKey(r))
                    {
                        originalColors.Add(r, r.material.GetColor(propName));
                    }
                    break;
                }
            }
        }
    }

    void Update()
    {
        // 1. スタン中なら操作を受け付けない
        if (isStunned)
        {
            currentBackwardSpeed = 0f;

            verticalVelocity -= gravity * Time.deltaTime;
            Vector3 stunMove = new Vector3(0, verticalVelocity, 0);
            characterController.Move(stunMove * Time.deltaTime);

            if (animator != null) animator.SetFloat("Speed", 0);
            return;
        }

        if (characterController.isGrounded)
        {
            // --- 2. 入力判定 ---
            bool dashInput = false;
            bool jumpInput = false;

            if (Keyboard.current != null)
            {
                if (PlayerNumber == 1 && Keyboard.current.qKey.wasPressedThisFrame) dashInput = true;
                if (PlayerNumber == 2 && Keyboard.current.rKey.wasPressedThisFrame) dashInput = true;
                if (PlayerNumber == 3 && Keyboard.current.uKey.wasPressedThisFrame) dashInput = true;
                if (PlayerNumber == 4 && Keyboard.current.pKey.wasPressedThisFrame) dashInput = true;
            }

            // --- 3. 連打移動ロジック（加算式に変更） ---
            if (dashInput)
            {
                // ★修正：速度を上書き(=)ではなく、加算(+=)して連打の恩恵を作る
                currentBackwardSpeed += dashSpeed;

                // 最高速度を超えないように制限
                if (currentBackwardSpeed > maxSpeed)
                {
                    currentBackwardSpeed = maxSpeed;
                }
            }

            // 減速処理
            currentBackwardSpeed = Mathf.MoveTowards(currentBackwardSpeed, 0, brakePower * Time.deltaTime);

            if (animator != null)
            {
                animator.SetFloat("Speed", currentBackwardSpeed);
            }

            // --- 4. ジャンプ処理 ---
            if (jumpInput)
            {
                verticalVelocity = jumpPower;
            }
            else
            {
                verticalVelocity = -2f;
            }
        }

        // --- 5. 最終的な移動計算 ---
        verticalVelocity -= gravity * Time.deltaTime;

        Vector3 finalMove = Vector3.back * currentBackwardSpeed;
        finalMove.y = verticalVelocity;

        characterController.Move(finalMove * Time.deltaTime);
    }

    // --- 外部から呼ばれるスタン機能 ---
    public void Stun(float duration = -1f)
    {
        if (!isStunned)
        {
            float timeToStun = (duration < 0) ? stunDuration : duration;
            StartCoroutine(StunCoroutine(timeToStun));
        }
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        currentBackwardSpeed = 0f; // 停止
        Debug.Log($"<color=red>Player{PlayerNumber} is Stunned!</color>");

        // --- 点滅ループ（色切り替え） ---
        float timer = 0f;
        bool useStunColor = true; // 最初は赤色

        while (timer < duration)
        {
            // すべてのレンダラーの色を「スタン色」か「元の色」に切り替える
            foreach (var kvp in originalColors)
            {
                Renderer r = kvp.Key;
                Color originalCol = kvp.Value;
                Color targetColor = useStunColor ? stunColor : originalCol;

                foreach (var propName in colorPropertyNames)
                {
                    if (r.material.HasProperty(propName))
                    {
                        r.material.SetColor(propName, targetColor);
                        break;
                    }
                }
            }

            // 次回のためにフラグを反転
            useStunColor = !useStunColor;

            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        // --- 復帰処理 ---
        isStunned = false;

        // 色を完全に元に戻す
        foreach (var kvp in originalColors)
        {
            Renderer r = kvp.Key;
            Color col = kvp.Value;

            foreach (var propName in colorPropertyNames)
            {
                if (r.material.HasProperty(propName))
                {
                    r.material.SetColor(propName, col);
                    break;
                }
            }
        }
    }

    // --- 衝突判定 ---
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("hito"))
        {
            Stun();
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("hito"))
        {
            Stun();
        }
    }
}