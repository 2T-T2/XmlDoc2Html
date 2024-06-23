using System.IO;
using System.Xml;
using System.Linq;

namespace XmlDoc2Html {
	///<summary>
	///</summary>
	public static class App {
		private static System.Reflection.Assembly Assembly {
			get {
				return System.Reflection.Assembly.GetAssembly(typeof(App));
			}
		}
		private static string styleSheetPath = Path.Combine(Path.GetDirectoryName(Assembly.Location), "XmlDoc2Html", "style.css");

		///<summary>
		///</summary>
		///<param name="args">
		///コマンドライン引数
		///</param>
		public static void Main(string[] args) {
			if (args.Contains("/help")) {
				WriteHelpText();
				return;
			}

			Arguments arguments = new Arguments( args );

			if (arguments.InputCheck().Count() != 0) {
				foreach (var msg in arguments.InputCheck())
					System.Console.WriteLine( msg );
				return;
			}

			if (!Directory.Exists(Path.GetDirectoryName(styleSheetPath))) {
				Directory.CreateDirectory(Path.GetDirectoryName(styleSheetPath));
			}

			if (!File.Exists(styleSheetPath)) {
				using(Stream strm = Assembly.GetManifestResourceStream("style.css")) {
				using (FileStream dest = File.Create( styleSheetPath ))
					strm.CopyTo(dest);
				}
			}

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(arguments.Input);

			createAssemblyTopPage(xmlDoc, arguments.Output);
			createTypePage(xmlDoc, arguments.Output);
			createMethodPage(xmlDoc, arguments.Output);
			createPropertyPage(xmlDoc, arguments.Output);
			createFieldPage(xmlDoc, arguments.Output);
		}

		/// <summary>フィールドページを作成</summary>
		/// <param name="xmlDoc">XML ドキュメント</param>
		/// <param name="outRootDir">ルート出力フォルダ</param>
		public static void createFieldPage(XmlDocument xmlDoc, string outRootDir) {
			string asmName = xmlDoc.SelectNodes("doc/assembly/name")[0].InnerText;

			foreach( XmlNode member in xmlDoc.SelectNodes("doc/members/member") ) {
				string[] name = member.Attributes["name"].Value.Split(':');
				switch( name[0] ) {
					case "F":
						// コンストラクターや演算子などの特殊なメソッドが含まれます。
						FieldPageInfo fieldPageInfo = new FieldPageInfo(asmName, name[1], outRootDir);

						// 説明
						fieldPageInfo.Summary = member.SelectNodes("summary")[0].InnerXml.Replace("            ", "");
						// 補足説明
						foreach( XmlNode remark in member.SelectNodes("remarks"))
							fieldPageInfo.Remark = remark.InnerXml.Replace("            ", "");
						// 例
						foreach( XmlNode example in member.SelectNodes("example"))
							fieldPageInfo.Example = example.InnerXml.Replace("            ", "");

						File.WriteAllText(Path.Combine(fieldPageInfo.PageLocation, fieldPageInfo.PageFileName), fieldPageInfo.ToHtml());
					break;
				}
			}

		}

		private static void WriteHelpText() {
			System.Console.WriteLine("====== 必須引数 ======");
			System.Console.WriteLine("1.入力XMLファイル");
			System.Console.WriteLine("====== オプション ======");
			System.Console.WriteLine("/output:<フォルダ>");
			System.Console.WriteLine("	デフォルト有：out");
			System.Console.WriteLine("	出力先フォルダを指定します。");
			System.Console.WriteLine("/help");
			System.Console.WriteLine("	ヘルプ表示。");
			System.Console.WriteLine("====== Tips ======");
			System.Console.WriteLine(styleSheetPath);
			System.Console.WriteLine(" を編集すると適応させるスタイルシートを変更できます");
			System.Console.WriteLine(" を削除して実行するとデフォルト状態のstyle.cssが作成されます");
		}

		/// <summary>プロパティページを作成</summary>
		/// <param name="xmlDoc">XML ドキュメント</param>
		/// <param name="outRootDir">ルート出力フォルダ</param>
		public static void createPropertyPage(XmlDocument xmlDoc, string outRootDir) {
			string asmName = xmlDoc.SelectNodes("doc/assembly/name")[0].InnerText;

			foreach( XmlNode member in xmlDoc.SelectNodes("doc/members/member") ) {
				string[] name = member.Attributes["name"].Value.Split(':');
				switch( name[0] ) {
					case "P":
						// コンストラクターや演算子などの特殊なメソッドが含まれます。
						PropertyPageInfo propertyPageInfo = new PropertyPageInfo(asmName, name[1], outRootDir);

						// 説明
						propertyPageInfo.Value = member.SelectNodes("value")[0].InnerXml.Replace("            ", "");
						// 補足説明
						foreach( XmlNode remark in member.SelectNodes("remarks"))
							propertyPageInfo.Remark = remark.InnerXml.Replace("            ", "");
						// 例
						foreach( XmlNode example in member.SelectNodes("example"))
							propertyPageInfo.Example = example.InnerXml.Replace("            ", "");

						File.WriteAllText(Path.Combine(propertyPageInfo.PageLocation, propertyPageInfo.PageFileName), propertyPageInfo.ToHtml());
					break;
				}
			}

		}

		/// <summary>関数ページを作成</summary>
		/// <param name="xmlDoc">XML ドキュメント</param>
		/// <param name="outRootDir">ルート出力フォルダ</param>
		public static void createMethodPage(XmlDocument xmlDoc, string outRootDir) {
			string asmName = xmlDoc.SelectNodes("doc/assembly/name")[0].InnerText;

			foreach( XmlNode member in xmlDoc.SelectNodes("doc/members/member") ) {
				string[] name = member.Attributes["name"].Value.Split(':');
				switch( name[0] ) {
					case "M":
						// コンストラクターや演算子などの特殊なメソッドが含まれます。
						MethodPageInfo methodPageInfo = new MethodPageInfo(asmName, name[1], outRootDir);

						// サマリ
						methodPageInfo.Summary = member.SelectNodes("summary")[0].InnerXml.Replace("            ", "");
						// 補足説明
						foreach( XmlNode remark in member.SelectNodes("remarks"))
							methodPageInfo.Remark = remark.InnerXml.Replace("            ", "");
						// 型パラメータ
						foreach( XmlNode typeparam in member.SelectNodes("typeparam"))
							methodPageInfo.AddTypeParams(typeparam.Attributes["name"].Value, typeparam.InnerXml.Replace("            ", ""));
						// 例
						foreach( XmlNode example in member.SelectNodes("example"))
							methodPageInfo.Example = example.InnerXml.Replace("            ", "");
						// 戻り値
						foreach( XmlNode returnNode in member.SelectNodes("returns"))
							methodPageInfo.ReturnValue = returnNode.InnerXml.Replace("            ", "");
						// 引数
						foreach( XmlNode param in member.SelectNodes("param"))
							methodPageInfo.AddParameters(param.Attributes["name"].Value, param.InnerXml.Replace("            ", ""));
						// 例外
						foreach( XmlNode exception in member.SelectNodes("exception"))
							methodPageInfo.AddExceptions(exception.Attributes["cref"].Value, exception.InnerXml.Replace("            ", ""));
						File.WriteAllText(Path.Combine(methodPageInfo.PageLocation, methodPageInfo.PageFileName), methodPageInfo.ToHtml());
					break;
				}
			}

		}

		/// <summary>型ページを作成</summary>
		/// <param name="xmlDoc">XML ドキュメント</param>
		/// <param name="outRootDir">ルート出力フォルダ</param>
		public static void createTypePage(XmlDocument xmlDoc, string outRootDir) {
			string asmName = xmlDoc.SelectNodes("doc/assembly/name")[0].InnerText;

			TypePageInfo typePageInfo = null;

			foreach( XmlNode member in xmlDoc.SelectNodes("doc/members/member") ) {
				string[] name = member.Attributes["name"].Value.Split(':');
				switch( name[0] ) {
					case "T":
						// クラス、インターフェイス、構造体、列挙型、またはデリゲートの型です。
						if (typePageInfo != null) {
							File.WriteAllText(Path.Combine(typePageInfo.PageLocation, TypePageInfo.PageFileName), typePageInfo.ToHtml());
							File.Copy(styleSheetPath, Path.Combine(typePageInfo.PageLocation, "style.css"), true);
						}

						typePageInfo = new TypePageInfo(asmName, name[1], outRootDir);
						// サマリ
						typePageInfo.Summary = member.SelectNodes("summary")[0].InnerXml.Replace("            ", "");
						// 補足説明
						foreach( XmlNode remark in member.SelectNodes("remarks"))
							typePageInfo.Remark = remark.InnerXml.Replace("            ", "");
						// 型パラメータ
						foreach( XmlNode typeparam in member.SelectNodes("typeparam"))
							typePageInfo.AddTypeParams(typeparam.Attributes["name"].Value, typeparam.InnerXml.Replace("            ", ""));
						// 例
						foreach( XmlNode example in member.SelectNodes("example"))
							typePageInfo.Example = example.InnerXml.Replace("            ", "");
						// 出力先の作成
						Directory.CreateDirectory(typePageInfo.PageLocation);
						break;
					case "F":
						// フィールド
						typePageInfo.AddFields(name[1], member.SelectNodes("summary")[0].InnerXml.Replace("            ", ""));
						break;
					case "M":
						// コンストラクターや演算子などの特殊なメソッドが含まれます。
						typePageInfo.AddMethods(name[1], member.SelectNodes("summary")[0].InnerXml.Replace("            ", ""));
						break;
					case "P":
						// インデクサーまたはその他のインデックス付きプロパティが含まれます。
						typePageInfo.AddProperties(name[1], member.SelectNodes("value")[0].InnerXml.Replace("            ", ""));
						break;
					case "E":
						// イベント
						typePageInfo.AddEvents(name[1], member.SelectNodes("summary")[0].InnerXml.Replace("            ", ""));
						break;
				}
			}
			if (typePageInfo != null) {
				File.WriteAllText(Path.Combine(typePageInfo.PageLocation, TypePageInfo.PageFileName), typePageInfo.ToHtml());
				File.Copy(styleSheetPath, Path.Combine(typePageInfo.PageLocation, "style.css"), true);
			}
		}

		/// <summary>アセンブリトップページを作成</summary>
		/// <param name="xmlDoc">XML ドキュメント</param>
		/// <param name="outRootDir">ルート出力フォルダ</param>
		public static void createAssemblyTopPage(XmlDocument xmlDoc, string outRootDir) {
			string asmName = xmlDoc.SelectNodes("doc/assembly/name")[0].InnerText;

			AssemblyTopPageInfo assemblyTopPageInfo = new AssemblyTopPageInfo(asmName, outRootDir);

			foreach( XmlNode member in xmlDoc.SelectNodes("doc/members/member") ) {
				string[] name = member.Attributes["name"].Value.Split(':');
				switch( name[0] ) {
					case "T":
						assemblyTopPageInfo.AddTypes(name[1], member.SelectNodes("summary")[0].InnerXml.Replace("            ", ""));
					break;
				}
			}
			File.WriteAllText(Path.Combine(assemblyTopPageInfo.PageLocation, AssemblyTopPageInfo.PageFileName), assemblyTopPageInfo.ToHtml());
			File.Copy(styleSheetPath, Path.Combine(assemblyTopPageInfo.PageLocation, "style.css"), true);
		}

	}
}
