using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace XmlDoc2Html {
	///<summary>型ページ情報</summary>
	public class AssemblyTopPageInfo {
        /// <summary>ファイル名</summary>
        public static readonly string PageFileName = "index.html"; 
		///<summary>アセンブリ名</summary>
		public readonly string AssemblyName;
		///<summary>ページルート</summary>
		public readonly string PageRoot;

		private Dictionary<string, string> types = new Dictionary<string, string>();

		///<summary>アセンブリに含まれる型情報を追加します</summary>
        /// <param name="name">名称</param>
        /// <param name="description">説明</param>
		public void AddTypes (string name, string description) { types.Add( name, description ); }

 		///<value>ページの位置</value>
		public string PageLocation {
			get {
				return Path.Combine(
					PageRoot
				);
			}
		}
		///<summary>コンストラクタ</summary>
        /// <param name="assemblyName">アセンブリ名</param>
        /// <param name="pageRoot">ドキュメントのルート</param>
		public AssemblyTopPageInfo(string assemblyName, string pageRoot) {
			this.AssemblyName = assemblyName;
			this.PageRoot = pageRoot;
		}

		///<summary>Html文字列に変換します</summary>
		///<param name="cssPath">使用するcssのパス（デフォルト="style.css"）</param>
        ///<returns><b>String</b>Html文字列</returns>
		public string ToHtml(string cssPath="style.css") {
			int indent = 0;
			StringBuilder sb = new StringBuilder("<html>").AppendLine();
			sb.Append(new String('\t', ++indent)).Append("<head>").AppendLine();
			sb.Append(new String('\t', ++indent)).Append("<title>").Append(AssemblyName).Append("</title>").AppendLine();
			sb.Append(new String('\t',   indent)).Append("<link rel=\"stylesheet\" href=\"").Append(cssPath).Append("\" />").AppendLine();
			sb.Append("<link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/highlight.js/9.15.10/styles/vs.min.css'>");
			sb.Append("<script src='https://cdnjs.cloudflare.com/ajax/libs/highlight.js/9.15.10/highlight.min.js'></script>");
			sb.Append("<script>hljs.initHighlightingOnLoad();</script>");
			sb.Append(new String('\t', --indent)).Append("</head>").AppendLine();

			sb.Append(new String('\t',   indent)).Append("<body>").AppendLine();
			sb.Append(new String('\t', ++indent)).Append("<h1>").Append(AssemblyName).Append("</h1>").AppendLine();

			sb.Append(new String('\t',   indent)).Append("<hr>").AppendLine();

			if (types.Count != 0) {
				sb.Append(new String('\t',   indent)).Append("<h2 id='heading2-type'>型</h2>").AppendLine();
				sb.Append(new String('\t', indent++)).Append("<table id='table-type'>").AppendLine();
				foreach (KeyValuePair<string, string> item in types)
					sb.Append(new String('\t',   indent)).Append("<tr><td><a href='").Append(item.Key.Replace(".", "/")).Append("/index.html").Append("'>").Append(item.Key).Append("</a></td><td>").Append(item.Value.Replace('\n', ' ').Replace('\r', ' ')).Append("</td></tr>").AppendLine();
				sb.Append(new String('\t', --indent)).Append("</table>").AppendLine();
			}
			sb.Append(new String('\t', --indent)).Append("</body>").AppendLine();
			sb.Append(new String('\t', --indent)).Append("</html>").AppendLine();

			return sb.ToString();
		}

   }
}