using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace XmlDoc2Html {
	///<summary>型ページ情報</summary>
	public class TypePageInfo {
		///<summary>ページのファイル名</summary>
		public static readonly string PageFileName = "index.html";

		///<summary>アセンブリ名</summary>
		public readonly string AssemblyName;
		///<summary>名前空間+型名</summary>
		public readonly string FullName;
		///<summary>ページルート</summary>
		public readonly string PageRoot;

		///<value>型名</value>
		public string Name {
			get {
				int i = FullName.LastIndexOf('.');
				if ( i == -1 )
					return FullName;
				return FullName.Substring(i+1);
			}
		}
		///<value>名前空間</value>
		public string Namespace {
			get {
				int i = FullName.LastIndexOf('.');
				if ( i == -1 )
					return "";
				return FullName.Substring(0, i);
			}
		}
		///<value>使用例</value>
		public string Example { get; set; }
		///<value>サマリ</value>
		///<remarks>サマリの補足説明</remarks>
		public string Summary { get; set; }
		///<value>補足説明</value>
		public string Remark { get; set; }
		///<value>ページの位置</value>
		public string PageLocation {
			get {
				return Path.Combine(
					PageRoot,
					Namespace.Replace('.', '/'),
					Name
				);
			}
		}

		private Dictionary<string, string> fields       = new Dictionary<string, string>();
		private Dictionary<string, string> methods      = new Dictionary<string, string>();
		private Dictionary<string, string> constructors = new Dictionary<string, string>();
		private Dictionary<string, string> properties   = new Dictionary<string, string>();
		private Dictionary<string, string> events       = new Dictionary<string, string>();
		private Dictionary<string, string> typeparams   = new Dictionary<string, string>();

		///<summary>コンストラクタ</summary>
        /// <param name="assemblyName">アセンブリ名</param>
        /// <param name="fullName">名前空間+型名</param>
        /// <param name="pageRoot">ドキュメントのルート</param>
		public TypePageInfo(string assemblyName, string fullName, string pageRoot) {
			this.AssemblyName = assemblyName;
			this.FullName = fullName;
			this.PageRoot = pageRoot;
		}

		///<summary>型に含まれるfields情報を追加します</summary>
        /// <param name="name">名称</param>
        /// <param name="summary">サマリ</param>
		public void AddFields (string name, string summary) { fields.Add( name, summary ); }

		///<summary>型に含まれるmethods情報を追加します(コンストラクタもこれに追加)</summary>
        /// <param name="name">名称</param>
        /// <param name="summary">サマリ</param>
		public void AddMethods (string name, string summary) {
			if (name.Contains("#ctor"))
				constructors.Add( name, summary );
			else
				methods.Add( name, summary );
		}

		///<summary>型に含まれるproperties情報を追加します</summary>
        /// <param name="name">名称</param>
        /// <param name="summary">サマリ</param>
		public void AddProperties (string name, string summary) { properties.Add( name, summary ); }

		///<summary>型に含まれるevents情報を追加します</summary>
        /// <param name="name">名称</param>
        /// <param name="summary">サマリ</param>
		public void AddEvents (string name, string summary) { events.Add( name, summary ); }

		///<summary>型に含まれるtypeparams情報を追加します</summary>
        /// <param name="name">名称</param>
        /// <param name="description">説明</param>
		public void AddTypeParams (string name, string description) { typeparams.Add( name, description ); }

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
			if (constructors.Count != 0) {
				sb.Append(new String('\t',   indent)).Append("<h2 id='heading2-constructor'>コンストラクタ</h2>").AppendLine();
				sb.Append(new String('\t', indent++)).Append("<table id='table-constructor'>").AppendLine();
				foreach (KeyValuePair<string, string> item in constructors)
					sb.Append(new String('\t',   indent)).Append("<tr><td><a href='").Append(item.Key.Replace(FullName+".#ctor",Name)).Append(".html").Append("'>").Append(item.Key.Replace(FullName+".#ctor",Name)).Append("</td><td>").Append(item.Value.Replace('\n', ' ').Replace('\r', ' ')).Append("</td></tr>").AppendLine();
				sb.Append(new String('\t', --indent)).Append("</table>").AppendLine();
			}
			if (fields.Count != 0) {
				sb.Append(new String('\t',   indent)).Append("<h2 id='heading2-field'>フィールド</h2>").AppendLine();
				sb.Append(new String('\t', indent++)).Append("<table id='table-field'>").AppendLine();
				foreach (KeyValuePair<string, string> item in fields.OrderBy( c => c.Key ))
					sb.Append(new String('\t',   indent)).Append("<tr><td><a href='").Append(item.Key.Replace(FullName+".", "")).Append(".html").Append("'>").Append(item.Key.Replace(FullName+".","")).Append("</a></td><td>").Append(item.Value.Replace('\n', ' ').Replace('\r', ' ')).Append("</td></tr>").AppendLine();
				sb.Append(new String('\t', --indent)).Append("</table>").AppendLine();
			}
			if (properties.Count != 0) {
				sb.Append(new String('\t',   indent)).Append("<h2 id='heading2-properties'>プロパティ</h2>").AppendLine();
				sb.Append(new String('\t', indent++)).Append("<table id='table-properties'>").AppendLine();
				foreach (KeyValuePair<string, string> item in properties.OrderBy( c => c.Key ))
					sb.Append(new String('\t',   indent)).Append("<tr><td><a href='").Append(item.Key.Replace(FullName+".", "")).Append(".html").Append("'>").Append(item.Key.Replace(FullName+".","")).Append("</a></td><td>").Append(item.Value.Replace('\n', ' ').Replace('\r', ' ')).Append("</td></tr>").AppendLine();
				sb.Append(new String('\t', --indent)).Append("</table>").AppendLine();
			}
			if (methods.Count != 0) {
				sb.Append(new String('\t',   indent)).Append("<h2 id='heading2-method'>メソッド</h2>").AppendLine();
				sb.Append(new String('\t', indent++)).Append("<table id='table-method'>").AppendLine();
				foreach (KeyValuePair<string, string> item in methods.OrderBy( c => c.Key ))
					sb.Append(new String('\t',   indent)).Append("<tr><td><a href='").Append(item.Key.Replace(FullName+".", "")).Append(".html").Append("'>").Append(item.Key.Replace(FullName+".","")).Append("</a></td><td>").Append(item.Value.Replace('\n', ' ').Replace('\r', ' ')).Append("</td></tr>").AppendLine();
				sb.Append(new String('\t', --indent)).Append("</table>").AppendLine();
			}
			if (events.Count != 0) {
				sb.Append(new String('\t',   indent)).Append("<h2 id='heading2-event'>イベント</h2>").AppendLine();
				sb.Append(new String('\t', indent++)).Append("<table id='table-event'>").AppendLine();
				foreach (KeyValuePair<string, string> item in events.OrderBy( c => c.Key ))
					sb.Append(new String('\t',   indent)).Append("<tr><td><a href='").Append(item.Key.Replace(FullName+".", "")).Append(".html").Append("'>").Append(item.Key.Replace(FullName+".","")).Append("</a></td><td>").Append(item.Value.Replace('\n', ' ').Replace('\r', ' ')).Append("</td></tr>").AppendLine();
				sb.Append(new String('\t', --indent)).Append("</table>").AppendLine();
			}
			sb.Append(new String('\t', --indent)).Append("</body>").AppendLine();
			sb.Append(new String('\t', --indent)).Append("</html>").AppendLine();

			return sb.ToString();
		}
	}
}
