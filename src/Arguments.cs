using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace XmlDoc2Html {
    ///<summary>
    /// �R�}���h���C�������N���X
    ///</summary>
    public class Arguments {
        private readonly string[] args;
        
        /// <value>
        /// �����̌�
        /// </value>
        public int Count {
            get {
                return this.args.Length;
            }
        }
    
    
        #region �K�{����
        private const int IDX__INPUT = 0;
        private readonly int idxInput;
        /// <value>
        /// ����XML
        /// </value>
        public string Input {
            get {
                return args[idxInput];
            }
        }
        #endregion
    
        #region �ȗ��\����
        /// <value>
        /// �f�B���N�g��
        /// </value>
        public string Output {
            get {
                    string a = args.Where(it => it.StartsWith("/output:")).FirstOrDefault();
                    if ( a == null ) return "out";
                    return a.Split(':')[1];
            }
        }
        #endregion
    
        #region �t���O�I�v�V����
        #endregion
    
        ///<summary>
        /// �R���X�g���N�^
        ///</summary>
        /// <param name="args">
        /// �R�}���h���C������
        /// </param>
        public Arguments(string[] args) {
            this.args = args;
            if ( args.Length == 0 ) return;
            // ================ �K�{���� ================
           this.idxInput = IDX__INPUT;
        }
        ///<summary>
        /// ���͋K���`�F�b�N
        ///</summary>
        ///<returns>
        ///�G���[���b�Z�[�W�񋓎q
        ///</returns>
        public IEnumerable<string> InputCheck() {
            List<string> errorMessages = new List<string>();
            if ( !File.Exists(Input) ) errorMessages.Add(string.Format("����XML�t�@�C����������܂���ł���: {0}", Input));
            return errorMessages;
        }
    }
}
