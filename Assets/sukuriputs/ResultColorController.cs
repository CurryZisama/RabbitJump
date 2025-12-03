using UnityEngine;
using System.Collections; // コルーチンを使うために追加
using UnityEngine.InputSystem; // ★新しいInput Systemを使うために追加

public class ResultColorController : MonoBehaviour
{
    [Header("色とオブジェクトの設定")]
    [Tooltip("プレイヤーごとの定義色 (Element 0: P1, Element 1: P2...)")]
    [SerializeField] private Color[] playerColors;

    [Tooltip("順位を表示するオブジェクトのRenderer (Element 0: 1位の場所...)")]
    [SerializeField] private Renderer[] rankObjects;

    [Header("ジャンプ設定")]
    [Tooltip("ジャンプの高さ")]
    [SerializeField] private float jumpHeight = 1.0f;
    [Tooltip("ジャンプにかかる時間（秒）")]
    [SerializeField] private float jumpDuration = 0.5f;

    [Header("デバッグ")]
    [SerializeField] private bool debugOnStart = false;

    // プレイヤーIDごとのGameObjectを保持する配列 (index 0 = 1P, 1 = 2P...)
    private GameObject[] playerObjectsMap = new GameObject[4];

    // ジャンプ中かどうかのフラグ (連打防止用)
    private bool[] isJumping = new bool[4];

    private void Start()
    {
        if (debugOnStart)
        {
            // デバッグ用: P2(1位), P4(2位), P1(3位), P3(4位)
            int[] dummyResults = { 1, 3, 0, 2 };
            SetResultColors(dummyResults);
        }
        else
        {
            if (GameResultData.FinalRank != null && GameResultData.FinalRank.Count == 4)
            {
                SetResultColors(GameResultData.FinalRank.ToArray());
            }
        }
    }

    private void Update()
    {
        // キーボードが接続されていない場合は処理しない
        if (Keyboard.current == null) return;

        // ★新しい Input System でのキー入力検知に変更
        // Qキー (1P)
        if (Keyboard.current.qKey.wasPressedThisFrame) TryJump(0);
        // Rキー (2P)
        if (Keyboard.current.rKey.wasPressedThisFrame) TryJump(1);
        // Uキー (3P)
        if (Keyboard.current.uKey.wasPressedThisFrame) TryJump(2);
        // Pキー (4P)
        if (Keyboard.current.pKey.wasPressedThisFrame) TryJump(3);
    }

    /// <summary>
    /// リザルトデータを受け取って色を更新し、操作対象をマッピングする
    /// </summary>
    public void SetResultColors(int[] rankedPlayerIndices)
    {
        if (rankedPlayerIndices.Length != 4 || rankObjects.Length < 4)
        {
            Debug.LogError("プレイヤー数またはRankObjectの設定が足りません");
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            // iは順位(0=1位)。rankedPlayerIndices[i]はその順位のプレイヤーID。
            int currentPlayerIndex = rankedPlayerIndices[i];

            if (currentPlayerIndex >= 0 && currentPlayerIndex < playerColors.Length)
            {
                // 色を変更
                rankObjects[i].material.color = playerColors[currentPlayerIndex];

                // ★重要: ジャンプ用に「このプレイヤーIDはこのオブジェクト」と記憶しておく
                playerObjectsMap[currentPlayerIndex] = rankObjects[i].gameObject;

                // ジャンプフラグを初期化
                isJumping[currentPlayerIndex] = false;
            }
        }
    }

    /// <summary>
    /// ジャンプを試みるメソッド
    /// </summary>
    private void TryJump(int playerIndex)
    {
        // まだオブジェクトが割り当てられていない、または既にジャンプ中なら何もしない
        if (playerObjectsMap[playerIndex] == null || isJumping[playerIndex]) return;

        StartCoroutine(JumpCoroutine(playerIndex));
    }

    /// <summary>
    /// 実際にジャンプさせるコルーチン（アニメーション）
    /// </summary>
    private IEnumerator JumpCoroutine(int playerIndex)
    {
        isJumping[playerIndex] = true; // ジャンプ開始フラグ

        GameObject targetObj = playerObjectsMap[playerIndex];
        Vector3 startPos = targetObj.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            // 時間経過(0.0 ～ 1.0)
            float t = elapsedTime / jumpDuration;

            // 放物線のような動きを作る (sinカーブの0～π部分を使用)
            // t=0のとき0, t=0.5のとき1, t=1のとき0 になる
            float heightFactor = Mathf.Sin(t * Mathf.PI);

            // 位置を更新
            targetObj.transform.position = startPos + Vector3.up * (heightFactor * jumpHeight);

            elapsedTime += Time.deltaTime;
            yield return null; // 1フレーム待機
        }

        // 最後に位置をきっちり元に戻す（ズレ防止）
        targetObj.transform.position = startPos;

        isJumping[playerIndex] = false; // ジャンプ終了
    }
}