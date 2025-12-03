// シーン間でデータを渡すための静的クラス
// このスクリプトはオブジェクトにアタッチする必要はありません
public static class GameResultData
{
    // ここに順位データ(int配列)を一時保存します
    // staticがついているのでシーンが変わっても中身は消えません
    public static int[] FinalRank;
}