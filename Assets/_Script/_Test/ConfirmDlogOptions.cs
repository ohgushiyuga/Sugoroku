// ファイル名: ConfirmDialogOptions.cs
using System;

public class ConfirmDialogOptions
{
    // ダイアログの種類に応じて読み込むプレハブのパス
    public string PrefabPath { get; set; }

    // 表示するテキスト
    public string Title { get; set; }
    public string Message { get; set; } // Messageも追加するとより汎用的

    // ボタンが押されたときに実行する処理
    public Action OnOK { get; set; }
    public Action OnCancel { get; set; }
}