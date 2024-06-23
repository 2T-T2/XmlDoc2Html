using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace XmlDoc2Html {
	///<summary>型ページ情報</summary>
	public class MethodPageInfo {
		///<summary>アセンブリ名</summary>
		public readonly string AssemblyName;
		///<summary>名前空間+型名+関数名+引数</summary>
		public readonly string FullName;
		///<summary>ページルート</summary>
		public readonly string PageRoot;

		///<value>関数名</value>
		public string Name {
			get {
				string a;
				int i = FullName.IndexOf('(');
				if ( i == -1 )
					a = FullName;
				else
					a = FullName.Substring(0, i);

				i = a.LastIndexOf(".");
				if ( i == -1 )
					return null;
				return a.Substring(i+1);
			}
		}
		///<value>名前空間+型名</value>
		public string ClassFullName {
			get {
				string a;
				int i = FullName.IndexOf('(');
				if ( i == -1 )
					a = FullName;
				else
					a = FullName.Substring(0, i);

				i = a.LastIndexOf(".");
				if ( i == -1 )
					return null;
				return a.Substring(0, i);
			}
		}
		///<value>名前空間</value>
		public string Namespace {
			get {
				int i = ClassFullName.LastIndexOf('.');
				if ( i == -1 )
					return "";
				return ClassFullName.Substring(0, i);
			}
		}

		///<value>使用例</value>
		public string Example { get; set; }
		///<value>サマリ</value>
		public string Summary { get; set; }
		///<value>補足説明</value>
		public string Remark { get; set; }
		///<value>戻り値</value>
		public string ReturnValue { get; set; }

		///<value>ページの位置</value>
		public string PageLocation {
			get {
				return Path.Combine(
					PageRoot,
					ClassFullName.Replace(".", "/")
				);
			}
		}
		///<value>ページのファイル名</value>
		public string PageFileName {
			get {
				string ClassName; 
				int i = ClassFullName.LastIndexOf('.');
				if ( i == -1 )
					ClassName = ClassFullName;
				else
					ClassName = ClassFullName.Substring(i+1);

				return FullName.Replace(ClassFullName+".", "").Replace("#ctor",ClassName) + ".html";
			}
		}

		private Dictionary<string, string> parameters = new Dictionary<string, string>();
		private Dictionary<string, string> exceptions = new Dictionary<string, string>();
		private Dictionary<string, string> typeparams   = new Dictionary<string, string>();

		///<summary>コンストラクタ</summary>
        /// <param name="assemblyName">アセンブリ名</param>
        /// <param name="fullName">名前空間+型名</param>
        /// <param name="pageRoot">ドキュメントのルート</param>
		public MethodPageInfo(string assemblyName, string fullName, string pageRoot) {
			this.AssemblyName = assemblyName;
			this.FullName = fullName;
			this.PageRoot = pageRoot;
		}

		///<summary>関数の引数情報を追加します</summary>
        /// <param name="name">名称</param>
        /// <param name="description">説明</param>
		public void AddParameters (string name, string description) { parameters.Add( name, description ); }

		///<summary>関数の例外情報を追加します</summary>
        /// <param name="name">名称</param>
        /// <param name="description">説明</param>
		public void AddExceptions (string name, string description) { exceptions.Add( name, description ); }

		///<summary>型に含まれるtypeparams情報を追加します</summary>
        /// <param name="name">名称</param>
        /// <param name="description">説明</param>
		public void AddTypeParams (string name, string description) { typeparams.Add( name, description ); }

		///<summary>Html文字列に変換します</summary>
		///<param name="cssPath">使用するcssのパス（デフォルト="style.css"）</param>
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

			if (Summary != null) {
				sb.Append(new String('\t',   indent)).Append("<h2 id='heading2-summary'>サマリ</h2>").AppendLine();
				sb.Append(new String('\t', indent++)).Append("<div id='division-summary'>").AppendLine();
				sb.Append(new String('\t',   indent)).Append(Summary).AppendLine();
				sb.Append(new String('\t', --indent)).Append("</div>").AppendLine();
			}
			if (Remark != null) {
				sb.Append(new String('\t',   indent)).Append("<h3 id='heading3-remark'>補足説明</h3>").AppendLine();
				sb.Append(new String('\t', indent++)).Append("<div id='division-remark'>").AppendLine();
				sb.Append(new String('\t',   indent)).Append(Remark).AppendLine();
				sb.Append(new String('\t', --indent)).Append("</div>").AppendLine();
			}
			if (typeparams.Count != 0) {
				sb.Append(new String('\t',   indent)).Append("<h2 id='heading2-typeparam'>型パラメーター</h2>").AppendLine();
				sb.Append(new String('\t', indent++)).Append("<table id='table-typeparam'>").AppendLine();
				foreach (KeyValuePair<string, string> item in typeparams)
					sb.Append(new String('\t',   indent)).Append("<tr><td>").Append(item.Key.Replace(FullName+".","")).Append("</td><td>").Append(item.Value.Replace('\n', ' ').Replace('\r', ' ')).Append("</td></tr>").AppendLine();
				sb.Append(new String('\t', --indent)).Append("</table>").AppendLine();
			}
			if (Example != null) {
				sb.Append(new String('\t',   indent)).Append("<h2 id='heading2-example'>例</h2>").AppendLine();
				sb.Append(new String('\t', indent++)).Append("<div id='division-example'>").AppendLine();
				sb.Append(new String('\t',   indent)).Append(Example).AppendLine();
				sb.Append(new String('\t', --indent)).Append("</div>").AppendLine();
			}
			if (parameters.Count != 0) {
				sb.Append(new String('\t',   indent)).Append("<h2 id='heading2-parameter'>引数</h2>").AppendLine();
				sb.Append(new String('\t', indent++)).Append("<table id='table-parameter'>").AppendLine();
				foreach (KeyValuePair<string, string> item in parameters)
					sb.Append(new String('\t',   indent)).Append("<tr><td>").Append(item.Key.Replace(FullName+".","")).Append("</td><td>").Append(item.Value.Replace('\n', ' ').Replace('\r', ' ')).Append("</td></tr>").AppendLine();
				sb.Append(new String('\t', --indent)).Append("</table>").AppendLine();
			}
			if (exceptions.Count != 0) {
				sb.Append(new String('\t',   indent)).Append("<h2 id='heading2-exception'>例外</h2>").AppendLine();
				sb.Append(new String('\t', indent++)).Append("<table id='table-exception'>").AppendLine();
				foreach (KeyValuePair<string, string> item in exceptions)
					sb.Append(new String('\t',   indent)).Append("<tr><td>").Append(item.Key.Replace(FullName+".","")).Append("</td><td>").Append(item.Value.Replace('\n', ' ').Replace('\r', ' ')).Append("</td></tr>").AppendLine();
				sb.Append(new String('\t', --indent)).Append("</table>").AppendLine();
			}
			if (ReturnValue != null) {
				sb.Append(new String('\t',   indent)).Append("<h2 id='heading2-returnValue'>戻り値</h2>").AppendLine();
				sb.Append(new String('\t', indent++)).Append("<div id='division-returnValue'>").AppendLine();
				sb.Append(new String('\t',   indent)).Append(ReturnValue).AppendLine();
				sb.Append(new String('\t', --indent)).Append("</div>").AppendLine();
			}
			sb.Append(new String('\t', --indent)).Append("</body>").AppendLine();
			sb.Append(new String('\t', --indent)).Append("</html>").AppendLine();

			return sb.ToString();
		}
	}
}
