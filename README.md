# QuestReplacer

PC版アバターをベースに簡易Quest対応する作業を楽にしたりするツールです
Switch Platformと連動しないことにしたので、Switch Platformしなくても見た目を確認できたり、PCとPCの切り替えに使えたりします

## できること

- Materialへの参照を差し替える
- Meshへの参照を差し替える
- 差し替え元と差し替え先の対応関係を記録して、自動で設定する
- 差し替え用のMaterialがない場合に自動生成する
  - Shaderを差し替えたMaterialが生成されますが、色味の調整は必要です
- 差し替えはいつでも手動実行できます

## やりたいこと

- 既存のボーンやGameObjectをEditorOnlyにする
- AnimatorやAniamtionClipへの参照を差し替える
  - マテリアル差し替えアニメーション用だけど大変そうなのでいったんは後回し

## やらないこと

- 既存のボーンやGameObjectに刺さっているコンポーネントを改変する

## できないこと

- 既存のMaterialやMeshを改変する
- 既存のボーンやGameObjectにコンポーネントを追加・削除する
