using System;
using System.Collections.Generic;

namespace QinDingTech.PowerDesignerHelper
{
	public class PdmKey
	{
		/// <summary>
		/// 关键字标识
		/// </summary>
		public string KeyId { get; set; }

		/// <summary>
		/// 对象Id
		/// </summary>
		public string ObjectId { get; set; }

		/// <summary>
		/// Key名
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Key代码,对应数据库中的Key.
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 创建日期
		/// </summary>
		public DateTime CreationDate { get; set; }

		/// <summary>
		/// 创建人
		/// </summary>
		public string Creator { get; set; }

		/// <summary>
		/// 修改日期
		/// </summary>
		public DateTime ModificationDate { get; set; }

		/// <summary>
		/// 修改人
		/// </summary>
		public string Modifier { get; set; }

		/// <summary>
		/// Key涉及的列
		/// </summary>
		public IList<ColumnInfo> Columns { get; private set; }

		public void AddColumn(ColumnInfo mColumn)
		{
			if (Columns == null)
				Columns = new List<ColumnInfo>();
			Columns.Add(mColumn);
		}

		private List<string> _ColumnObjCodes = new List<string>();

		/// <summary>
		/// Key涉及的列代码，根据辞可访问到列信息.对应列的ColumnId
		/// </summary>
		public List<string> ColumnObjCodes
		{
			get { return _ColumnObjCodes; }
		}

		public TableInfo OwnerTable { get; set; }

		public void AddColumnObjCode(string objCode)
		{
			_ColumnObjCodes.Add(objCode);
		}

		public PdmKey(TableInfo table)
		{
			OwnerTable = table;
		}
	}
}