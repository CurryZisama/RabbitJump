using System.Collections.Generic; // Listを使うために必要

// シーン間でデータを渡すための静的クラス
// このスクリプトはオブジェクトにアタッチする必要はありません
public static class GameResultData
{
    // ここに順位データ(List<int>)を一時保存します
    // staticがついているのでシーンが変わっても中身は消えません
    public static List<int> FinalRank;
}