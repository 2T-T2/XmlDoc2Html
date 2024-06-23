using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace XmlDoc2Html {
	///<summary>型ページ情報</summary>
	public class PropertyPageInfo {
		///<value>ページのファイル名</value>
		public string PageFileName {
			get {
				return Name + ".html";
			}
		}

		///<summary>アセンブリ名</summary>
		public readonly string AssemblyName;
		///<summary>名前空間+型名+プロパティ名</summary>
		public readonly string FullName;
		///<summary>ページルート</summary>
		public readonly string PageRoot;

		///<value>プロパティ名</value>
		public string Name {
			get {
				int i = FullName.LastIndexOf('.');
				if ( i == -1 )
					return FullName;
				return FullName.Substring(i+1);
			}
		}
		///<value>名前空間+クラス名</value>
		public string ClassFullName {
			get {
				int i = FullName.LastIndexOf('.');
				if ( i == -1 )
					return null;
				return FullName.Substring(0, i);
			}
		}
		///<value>名前空間</value>
		public string Namespace {
			get {
				int i = ClassFullName.LastIndexOf('.');
				if ( i == -1 )
					return null;
				return ClassFullName.Substring(0, i);
			}
		}
		///<value>使用例</value>
		public string Example { get; set; }
		///<value>説明</value>
		public string Value { get; set; }
		///<value>補足説明</value>
		public string Remark { get; set; }
		///<value>ページの位置</value>
		public string PageLocation {
			get {
				return Path.Combine(
					PageRoot,
					ClassFullName.Replace('.', '/')
				);
			}
		}

		///<summary>コンストラクタ</summary>
        /// <param name="assemblyName">アセンブリ名</param>
        /// <param name="fullName">名前空間+型名</param>
        /// <param name="pageRoot">ドキュメントのルート</param>
		public PropertyPageInfo(string assemblyName, string fullName, string pageRoot) {
			this.AssemblyName = assemblyName;
			this.FullName = fullName;
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
			sb.Append(new String('\t', ++indent)).Append("<h1>").Append(FullName).Append("</h1>").AppendLine();

			sb.Append(new String('\t',   indent)).Append("<hr>").AppendLine();

			if (Value != null) {
				sb.Append(new String('\t',   indent)).Append("<h2 id='heading2-value'>説明</h2>").AppendLine();
				sb.Append(new String('\t', indent++)).Append("<div id='division-value'>").AppendLine();
				sb.Append(new String('\t',   indent)).Append(Value).AppendLine();
				sb.Append(new String('\t', --indent)).Append("</div>").AppendLine();
			}
			if (Remark != null) {
				sb.Append(new String('\t',   indent)).Append("<h3 id='heading3-remark'>補足説明</h3>").AppendLine();
				sb.Append(new String('\t', indent++)).Append("<div id='division-remark'>").AppendLine();
				sb.Append(new String('\t',   indent)).Append(Remark).AppendLine();
				sb.Append(new String('\t', --indent)).Append("</div>").AppendLine();
			}
			if (Example != null) {
				sb.Append(new String('\t',   indent)).Append("<h2 id='heading2-example'>例</h2>").AppendLine();
				sb.Append(new String('\t', indent++)).Append("<div id='division-example'>").AppendLine();
				sb.Append(new String('\t',   indent)).Append(Example).AppendLine();
				sb.Append(new String('\t', --indent)).Append("</div>").AppendLine();
			}

			sb.Append(new String('\t', --indent)).Append("</body>").AppendLine();
			sb.Append(new String('\t', --indent)).Append("</html>").AppendLine();

			return sb.ToString();
		}
	}
}
