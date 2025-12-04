using UnityEngine;
using System.Collections; // コルーチン用

public class ObstacleSpawner : MonoBehaviour
{
    [Header("生成設定")]
    [Tooltip("生成する障害物のプレハブ")]
    public GameObject obstaclePrefab;

    [Tooltip("生成する間隔（秒）")]
    public float spawnInterval = 2.0f;

    [Header("生成範囲エリア (Vector3)")]
    [Tooltip("生成エリアの中心座標（オフセット）")]
    public Vector3 spawnCenter = new Vector3(0, 0.5f, 100f);

    [Tooltip("生成エリアのサイズ (幅, 高さ, 奥行き)")]
    public Vector3 spawnSize = new Vector3(20f, 0f, 190f);

    [Header("障害物の動作設定（生成時に上書き）")]
    [Tooltip("障害物の移動速度 (tekimoveCOの数値を上書きします)")]
    public float obstacleSpeed = 5.0f;

    [Tooltip("障害物が消滅するX座標 (tekimoveCOの数値を上書きします)")]
    public float obstacleDestroyX = 20.0f;

    [Header("エディタ表示")]
    [Tooltip("範囲表示の色")]
    public Color gizmoColor = new Color(1, 0, 0, 0.5f);

    private void Start()
    {
        // 定期的に生成するコルーチンを開始
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        // ゲーム中ずっとループ
        while (true)
        {
            SpawnObstacle();
            // 指定した秒数待つ
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnObstacle()
    {
        if (obstaclePrefab == null)
        {
            Debug.LogWarning("ObstacleSpawner: 障害物のプレハブが設定されていません");
            return;
        }

        // Vector3の範囲内でランダムな位置を計算
        // Centerから Size/2 の範囲でプラスマイナスする
        float randomX = Random.Range(-spawnSize.x / 2, spawnSize.x / 2);
        float randomY = Random.Range(-spawnSize.y / 2, spawnSize.y / 2);
        float randomZ = Random.Range(-spawnSize.z / 2, spawnSize.z / 2);

        // 中心位置（オフセット）を加算
        Vector3 spawnPos = spawnCenter + new Vector3(randomX, randomY, randomZ);

        // プレハブを生成し、生成されたオブジェクトを変数に格納
        GameObject newObstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);

        // ★追加: 生成した障害物のスクリプト(tekimoveCO)を取得して、スポナーの設定値を渡す
        tekimoveCO tekiScript = newObstacle.GetComponent<tekimoveCO>();
        if (tekiScript != null)
        {
            tekiScript.moveSpeed = obstacleSpeed;
            tekiScript.destroyPosX = obstacleDestroyX;
        }
    }

    // Unityエディタのシーンビューに範囲を表示する機能
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // Vector3を使ったので描画が非常にシンプルになります
        // ワイヤーフレーム（枠線）
        Gizmos.DrawWireCube(spawnCenter, spawnSize);

        // 分かりやすく塗りつぶしの箱も薄く描画
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.2f);
        Gizmos.DrawCube(spawnCenter, spawnSize);
    }
}