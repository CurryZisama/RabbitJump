using UnityEngine;
using UnityEngine.InputSystem; // Input Systemを使用

[RequireComponent(typeof(CharacterController))]
public class SimpleCharacterController : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("連打1回あたりの瞬発力（速さ）")]
    public float dashSpeed = 8.0f; // 少し速めにしてステップ感を出す
    [Tooltip("ブレーキの強さ（値が大きいほどすぐ止まる）")]
    public float brakePower = 15.0f;

    [Header("ジャンプ・重力")]
    public float jumpPower = 5.0f;
    public float gravity = 20.0f;

    [Header("アニメーション設定（任意）")]
    public Animator animator;

    // 内部変数
    private CharacterController characterController;
    private float currentBackwardSpeed = 0f; // 現在の後退速度
    private float verticalVelocity; // 重力・ジャンプ用

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (characterController.isGrounded)
        {
            // --- 1. 入力判定（押した瞬間だけ反応） ---
            bool dashInput = false;
            bool jumpInput = false;

            if (Keyboard.current != null)
            {
                // Qキー または 下矢印 を「押した瞬間」だけ true
                if (Keyboard.current.qKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
                {
                    dashInput = true;
                }

                // スペースキー
                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    jumpInput = true;
                }
            }

            // --- 2. 連打移動ロジック ---
            if (dashInput)
            {
                // ボタンを押した瞬間、速度をセット（上書き）
                currentBackwardSpeed = dashSpeed;
            }

            // 毎フレーム、速度を減速させる（0になるまでブレーキをかける）
            currentBackwardSpeed = Mathf.MoveTowards(currentBackwardSpeed, 0, brakePower * Time.deltaTime);

            // --- アニメーション（速度がある時だけ動かす） ---
            if (animator != null)
            {
                animator.SetFloat("Speed", currentBackwardSpeed);
            }

            // --- 3. ジャンプ処理 ---
            if (jumpInput)
            {
                verticalVelocity = jumpPower;
            }
            else
            {
                verticalVelocity = -2f; // 接地安定化
            }
        }

        // --- 4. 最終的な移動計算 ---

        // 重力適用
        verticalVelocity -= gravity * Time.deltaTime;

        // 後ろ方向(Vector3.back) に 現在の速度を掛ける
        Vector3 finalMove = Vector3.back * currentBackwardSpeed;

        // 上下方向の速度を適用
        finalMove.y = verticalVelocity;

        // 移動実行
        characterController.Move(finalMove * Time.deltaTime);
    }
}