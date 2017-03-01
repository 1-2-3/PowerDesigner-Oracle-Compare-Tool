using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QinDingTech.PowerDesignerHelper
{
    /// <summary>
    /// 存储过程信息
    /// </summary>
    public class ProcedureInfo
    {
        /// <summary>
		/// 存储过程名
		/// </summary>
		public string Name { get; set; }

        /// <summary>
        /// 存储过程代码=>数据库中的存储过程名
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 存储过程定义=>数据库中的存储过程定义代码
        /// </summary>
        public string Text { get; set; }
    }
}
