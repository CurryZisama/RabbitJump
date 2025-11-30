using UnityEngine;

public class tekimoveCO : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("移動速度 (1秒間に進む距離)")]
    public float moveSpeed = 5.0f;

    [Header("消滅設定")]
    [Tooltip("このX座標を超えたら消去する")]
    public float destroyPosX = 20.0f;

    // --- 追加修正: 開始時に向きを調整 ---
    void Start()
    {
        // キャラクターの「正面（Z軸）」を、進行方向である「右（X軸）」に向けます。
        // これにより、カニ歩きではなく前を向いて歩くようになります。
        transform.rotation = Quaternion.LookRotation(Vector3.right);
    }

    void Update()
    {
        // ---------------------------------------------------------
        // 1. 移動処理
        // ---------------------------------------------------------
        // Vector3.right は (1, 0, 0) を意味します。
        // Space.Worldを指定することで、オブジェクトの回転に関わらず
        // ワールド座標の「右（+X）」へ移動します。
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);

        // ---------------------------------------------------------
        // 2. 画面外での消去処理
        // ---------------------------------------------------------
        // 現在のX座標が、設定した消去ラインを超えたら
        if (transform.position.x > destroyPosX)
        {
            // 自分自身をゲームから削除する
            Destroy(gameObject);
        }
    }
}