using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // シーン遷移に必要

public class GoalManager : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("シーン遷移管理スクリプト（ヒエラルキーにあるSceneFlowManagerをセット）")]
    [SerializeField] private SceneFlowManager sceneFlowManager;

    [Tooltip("リザルトシーン名（SceneFlowManagerを使わない場合に使用）")]
    [SerializeField] private string resultSceneName = "Risult";

    [Header("ゴール状況")]
    [Tooltip("ゴールした順にプレイヤー番号が追加されます")]
    public List<int> finishedRanking = new List<int>();

    // ゲーム開始時にデータをリセット
    void Start()
    {
        // エラー対策: GameResultData.FinalRank が null の場合は新しく作成
        if (GameResultData.FinalRank == null)
        {
            GameResultData.FinalRank = new List<int>();
        }
        else
        {
            GameResultData.FinalRank.Clear();
        }

        finishedRanking.Clear();
    }

    // 衝突判定 (Trigger)
    void OnTriggerEnter(Collider other)
    {
        // ぶつかった相手が SimpleCharacterController を持っているか確認
        SimpleCharacterController player = other.GetComponent<SimpleCharacterController>();

        if (player != null)
        {
            // プレイヤー番号を取得 (例: 1, 2, 3, 4)
            int pNum = player.PlayerNumber;

            // プレイヤー番号が 1～4 で来るため、配列のインデックス(0～3)に合わせて -1 します
            int pNumIndex = pNum - 1;

            // まだリストに含まれていなければ追加（二重ゴール防止）
            if (!finishedRanking.Contains(pNumIndex))
            {
                finishedRanking.Add(pNumIndex);

                int rank = finishedRanking.Count;
                Debug.Log($"<color=yellow>Player {pNum} (Index:{pNumIndex}) Finished! Rank: {rank}</color>");

                // 以前あった停止処理（player.enabled = false; など）を削除しました
                // ゴール後も速度や操作には一切干渉せず、順位検知のみを行います

                // ★★★ 4人全員ゴールしたか判定 ★★★
                if (finishedRanking.Count >= 4)
                {
                    GoToResult();
                }
            }
        }
    }

    // リザルト画面へ移動する処理
    void GoToResult()
    {
        Debug.Log("全員ゴールしました！リザルトへ移動します。");

        // 安全策：nullなら作成
        if (GameResultData.FinalRank == null)
        {
            GameResultData.FinalRank = new List<int>();
        }

        // リストの内容を保存（コピーを作成して渡す）
        GameResultData.FinalRank = new List<int>(finishedRanking);

        // シーン遷移
        if (sceneFlowManager != null)
        {
            // .ToArray() を付けて配列にして渡す
            sceneFlowManager.FinishRaceAndGoToResult(GameResultData.FinalRank.ToArray());
        }
        else
        {
            // 直接遷移する場合
            SceneManager.LoadScene(resultSceneName);
        }
    }
}