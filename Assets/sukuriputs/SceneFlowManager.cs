using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowManager : MonoBehaviour
{
    [Header("シーン名の設定")]
    [Tooltip("タイトルシーンの名前")]
    [SerializeField] private string titleSceneName = "Title";   // 変更: Title

    [Tooltip("レース（ゲーム）シーンの名前")]
    [SerializeField] private string raceSceneName = "TestSecn"; // 変更: TestSecn

    [Tooltip("リザルトシーンの名前")]
    [SerializeField] private string resultSceneName = "Risult"; // 変更: Risult

    // --- ボタンなどで呼び出すためのメソッド ---

    /// <summary>
    /// レースを開始する（タイトル画面のスタートボタンなどで使用）
    /// </summary>
    public void GoToRaceScene()
    {
        SceneManager.LoadScene(raceSceneName);
    }

    /// <summary>
    /// タイトルに戻る（リザルト画面の戻るボタンなどで使用）
    /// </summary>
    public void ReturnToTitle()
    {
        SceneManager.LoadScene(titleSceneName);
    }

    /// <summary>
    /// もう一度レースをする（リザルト画面のリトライボタンなどで使用）
    /// </summary>
    public void RetryRace()
    {
        SceneManager.LoadScene(raceSceneName);
    }

    // --- スクリプトから呼び出すためのメソッド ---

    /// <summary>
    /// レースが終了したときに呼ぶ。順位データを保存してリザルトへ移動。
    /// </summary>
    /// <param name="finalRanking">確定した順位配列 (例: {1, 0, 3, 2})</param>
    public void FinishRaceAndGoToResult(int[] finalRanking)
    {
        // 1. データを保存場所(GameResultData)に渡す
        // ※GameResultDataクラスが存在する必要があります
        GameResultData.FinalRank = finalRanking;

        // 2. リザルトシーンを読み込む
        SceneManager.LoadScene(resultSceneName);
    }

    // デバッグ用：キーボードのスペースキーでタイトルに戻る（リザルト画面などで便利）
    private void Update()
    {
        // リザルトシーンにいるときだけ有効にするなどの判定を入れても良い
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // 現在のシーンがリザルトならタイトルへ（簡易実装）
            if (SceneManager.GetActiveScene().name == resultSceneName)
            {
                ReturnToTitle();
            }
        }
    }
}