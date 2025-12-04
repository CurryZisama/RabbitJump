using UnityEngine;

public class SCurveObstacle : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("Z軸方向（奥）に進むスピード")]
    public float moveSpeed = 5.0f;

    [Tooltip("S字の振幅（左右の揺れ幅）")]
    public float waveAmplitude = 3.0f;

    [Tooltip("S字の周波数（揺れる速さ）")]
    public float waveFrequency = 2.0f;

    [Header("消滅設定")]
    [Tooltip("生成されてから消えるまでの時間（秒）")]
    public float lifeTime = 10.0f;

    // 基準位置
    private Vector3 startPosition;
    // 経過時間
    private float timeElapsed = 0f;

    void Start()
    {
        // 生成された瞬間の位置を基準にする
        startPosition = transform.position;

        // 指定時間後に消滅（画面外判定の代わり）
        Destroy(gameObject, lifeTime);

        // 念のため向きをリセット（必要に応じて変更してください）
        // transform.rotation = Quaternion.LookRotation(Vector3.forward);
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        // Z軸：等速直線運動 (初期位置Z + スピード * 時間)
        float z = startPosition.z + (moveSpeed * timeElapsed);

        // X軸：Sin波による振動 (初期位置X + Sin(時間 * 周波数) * 振幅)
        // これで左右（X軸）にゆらゆらします
        float x = startPosition.x + Mathf.Sin(timeElapsed * waveFrequency) * waveAmplitude;

        // Y軸：そのまま（高さは変えない）
        float y = startPosition.y;

        // 位置を更新
        transform.position = new Vector3(x, y, z);

        // --- 応用: 進行方向を向かせたい場合 ---
        // 蛇行に合わせて体の向きを変えたい場合は、以下のコードを使ってください。
        /*
        float nextTime = timeElapsed + 0.1f; // 少し未来の時間
        float nextX = startPosition.x + Mathf.Sin(nextTime * waveFrequency) * waveAmplitude;
        float nextZ = startPosition.z + (moveSpeed * nextTime);
        Vector3 nextPos = new Vector3(nextX, y, nextZ);
        
        Vector3 direction = nextPos - transform.position;
        if (direction != Vector3.zero) 
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        */
    }
}