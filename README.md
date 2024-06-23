# XmlDoc2Html
C#, VB などのドキュメントコメントの出力ファイル(xml)をhtmlに変換する

# ビルド方法
```bat
build.bat release
```
出力先 → out\release\XmlDoc2Html.exe

# 使用方法
```bat
====== 必須引数 ======
1.入力XMLファイル
====== オプション ======
/output:<フォルダ>
        デフォルト有：out
        出力先フォルダを指定します。
/help
        ヘルプ表示。
====== Tips ======
XmlDoc2Html\style.css
 を編集すると適応させるスタイルシートを変更できます
 を削除して実行するとデフォルト状態のstyle.cssが作成されます
```
## 使用例
```bat
xmldoc2html hogedoc.xml /output:out
```
# memo
xmldoc2html.exe 配置ディレクトリにXmlDoc2Htmlフォルダが作成されます。
xmldoc2html.exe 配置ディレクトリにXmlDoc2Html\style.cssファイルが作成されます。

