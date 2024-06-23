using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace XmlDoc2Html {
    ///<summary>
    /// コマンドライン引数クラス
    ///</summary>
    public class Arguments {
        private readonly string[] args;
        
        /// <value>
        /// 引数の個数
        /// </value>
        public int Count {
            get {
                return this.args.Length;
            }
        }
    
    
        #region 必須引数
        private const int IDX__INPUT = 0;
        private readonly int idxInput;
        /// <value>
        /// 入力XML
        /// </value>
        public string Input {
            get {
                return args[idxInput];
            }
        }
        #endregion
    
        #region 省略可能引数
        /// <value>
        /// ディレクトリ
        /// </value>
        public string Output {
            get {
                    string a = args.Where(it => it.StartsWith("/output:")).FirstOrDefault();
                    if ( a == null ) return "out";
                    return a.Split(':')[1];
            }
        }
        #endregion
    
        #region フラグオプション
        #endregion
    
        ///<summary>
        /// コンストラクタ
        ///</summary>
        /// <param name="args">
        /// コマンドライン引数
        /// </param>
        public Arguments(string[] args) {
            this.args = args;
            if ( args.Length == 0 ) return;
            // ================ 必須引数 ================
           this.idxInput = IDX__INPUT;
        }
        ///<summary>
        /// 入力規制チェック
        ///</summary>
        ///<returns>
        ///エラーメッセージ列挙子
        ///</returns>
        public IEnumerable<string> InputCheck() {
            List<string> errorMessages = new List<string>();
            if ( !File.Exists(Input) ) errorMessages.Add(string.Format("入力XMLファイルが見つかりませんでした: {0}", Input));
            return errorMessages;
        }
    }
}
