using System.Collections.Generic;

namespace QinDingTech.PowerDesignerHelper
{
	public class PdmModel
	{
		public PdmModel()
		{
			Tables = new List<TableInfo>();
			Views = new List<ViewInfo>();
            Procedures = new List<ProcedureInfo>();
        }

		/// <summary>
		/// 表集合
		/// </summary>
		public IList<TableInfo> Tables { get; private set; }

		/// <summary>
		/// 视图集合
		/// </summary>
		public IList<ViewInfo> Views { get; private set; }

        /// <summary>
        /// 存储过程集合
        /// </summary>
        public IList<ProcedureInfo> Procedures { get; private set; }
	}
}