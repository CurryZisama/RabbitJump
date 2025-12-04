using UnityEngine;

public class tekimoveCO : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("移動速度 (マイナスにすると左に進みます)")]
    public float moveSpeed = 5.0f;

    [Header("消滅設定")]
    [Tooltip("このX座標を超えたら消去する")]
    public float destroyPosX = 20.0f;

    void Start()
    {
        // ★修正1: 速度のプラスマイナスで向く方向を変える
        // 正なら右(Vector3.right)、負なら左(Vector3.left)を向く
        Vector3 facingDir = (moveSpeed >= 0) ? Vector3.right : Vector3.left;
        transform.rotation = Quaternion.LookRotation(facingDir);
    }

    void Update()
    {
        // 1. 移動処理
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);

        // 2. 画面外での消去処理
        // ★修正2: 速度の向きによって判定条件を逆にする
        if (moveSpeed > 0)
        {
            // 右へ進む場合：指定ラインより「大きく」なったら消す
            if (transform.position.x > destroyPosX)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // 左へ進む場合：指定ラインより「小さく」なったら消す
            if (transform.position.x < destroyPosX)
            {
                Destroy(gameObject);
            }
        }
    }
}