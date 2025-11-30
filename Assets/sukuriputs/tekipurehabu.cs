using UnityEngine;
using System.Collections;

public class tekipurehabu : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("出現させる敵のプレハブ")]
    public GameObject enemyPrefab;

    [Tooltip("何秒ごとに出現させるか")]
    public float spawnInterval = 2.0f;

    [Header("出現範囲 (Z座標)")]
    [Tooltip("Z座標の最小値")]
    public float zMin = -5.0f;
    [Tooltip("Z座標の最大値")]
    public float zMax = 5.0f;

    // 内部変数
    private float timer;

    void Start()
    {
        // ゲーム開始時に即座に1体出す場合はここでも呼ぶ
        // SpawnEnemy();
    }

    void Update()
    {
        // 時間を計測
        timer += Time.deltaTime;

        // 指定した時間が経過したら
        if (timer >= spawnInterval)
        {
            SpawnEnemy(); // 敵を出現させる
            timer = 0f;   // タイマーをリセット
        }
    }

    // 敵を出現させる関数
    void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("プレハブが設定されていません！インスペクターで設定してください。");
            return;
        }

        // Z座標をランダムに決定 (zMin から zMax の間)
        float randomZ = Random.Range(zMin, zMax);

        // 出現位置を作成
        // XとYはこのスクリプトがついているオブジェクトの位置を使い、Zだけランダムにする
        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, randomZ);

        // プレハブを生成 (位置: spawnPosition, 回転: プレハブの元の回転)
        Instantiate(enemyPrefab, spawnPosition, enemyPrefab.transform.rotation);
    }

    // エディタ上で出現範囲を赤い線で表示する（デバッグ用）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // 範囲を示す線を引く
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y, zMin);
        Vector3 endPos = new Vector3(transform.position.x, transform.position.y, zMax);
        Gizmos.DrawLine(startPos, endPos);
    }
}