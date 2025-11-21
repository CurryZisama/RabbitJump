using UnityEngine;

// 必須コンポーネントとしてCharacterControllerを指定
[RequireComponent(typeof(CharacterController))]
public class SimpleCharacterController : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("歩く速さ")]
    public float moveSpeed = 5.0f;
    [Tooltip("振り向く速さ")]
    public float rotationSpeed = 10.0f;
    [Tooltip("ジャンプ力")]
    public float jumpPower = 5.0f;
    [Tooltip("重力の強さ")]
    public float gravity = 20.0f;

    [Header("アニメーション設定（任意）")]
    [Tooltip("Animatorコンポーネントがある場合はここに入れる")]
    public Animator animator;

    // 内部変数
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float verticalVelocity; // 垂直方向（重力・ジャンプ）の速度

    void Start()
    {
        // CharacterControllerコンポーネントを取得
        characterController = GetComponent<CharacterController>();

        // Animatorがアタッチされていない場合、自動で取得を試みる
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        // 地面に接地しているかどうかの判定
        if (characterController.isGrounded)
        {
            // --- 1. 入力の取得 ---
            // WASDキー または 矢印キー の入力を取得 (-1.0 ～ 1.0)
            float horizontalInput = Input.GetAxis("Horizontal"); // 横移動 (A/D, 左/右)
            float verticalInput = Input.GetAxis("Vertical");     // 前後移動 (W/S, 上/下)

            // --- 2. 移動ベクトルの作成 ---
            // カメラの向きに関係なく、画面上の「上」を奥、「右」を右として移動する場合
            Vector3 inputDirection = new Vector3(horizontalInput, 0, verticalInput);

            // 入力がある場合のみ移動処理を行う
            if (inputDirection.magnitude > 0.1f)
            {
                // 入力ベクトルを正規化（斜め移動でも速さが変わらないようにする）
                inputDirection.Normalize();

                // キャラクターの向きを進行方向へスムーズに回転させる
                Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // 移動方向を決定
                moveDirection = inputDirection * moveSpeed;

                // --- アニメーション制御 (走る) ---
                if (animator != null)
                {
                    // "Speed"というパラメータがある場合、移動速度を渡す
                    // Animator側で Blend Tree を組んでいる場合に有効
                    animator.SetFloat("Speed", moveDirection.magnitude);
                }
            }
            else
            {
                // 入力がない場合は停止
                moveDirection = Vector3.zero;

                // --- アニメーション制御 (待機) ---
                if (animator != null)
                {
                    animator.SetFloat("Speed", 0);
                }
            }

            // --- 3. ジャンプ処理 ---
            if (Input.GetButtonDown("Jump")) // Spaceキー
            {
                verticalVelocity = jumpPower;

                // ジャンプアニメーションがあればここでトリガー
                if (animator != null)
                {
                    // animator.SetTrigger("Jump"); // 必要であればコメントアウトを外す
                }
            }
            else
            {
                // 接地しているときは垂直速度をリセット（少し下向きに力をかけて接地を安定させる）
                verticalVelocity = -2f;
            }
        }

        // --- 4. 重力の適用 ---
        // 空中にいる間、重力分だけ下向きの速度を加算し続ける
        verticalVelocity -= gravity * Time.deltaTime;

        // 重力・ジャンプの速度を移動ベクトルに合成
        Vector3 finalMove = moveDirection;
        finalMove.y = verticalVelocity;

        // --- 5. 最終的な移動 ---
        // CharacterControllerを使って移動させる（Time.deltaTimeを掛けてフレームレート依存を防ぐ）
        characterController.Move(finalMove * Time.deltaTime);
    }
}