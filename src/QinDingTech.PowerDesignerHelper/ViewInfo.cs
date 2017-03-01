using System;
using System.Collections.Generic;

namespace QinDingTech.PowerDesignerHelper
{
	public class ViewInfo
	{
		public ViewInfo()
		{
			Columns = new List<ViewColumnInfo>();
		}

		/// <summary>
		/// 视图ID
		/// </summary>
		public string ViewId { get; set; }

		/// <summary>
		/// 视图对象id
		/// </summary>
		public string ObjectId { get; set; }

		/// <summary>
		/// 视图名
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 视图代码=>数据库中的视图名
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
		/// 视图SQL
		/// </summary>
		public string ViewSqlQuery { get; set; }

		/// <summary>
		/// 注释
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// 描述
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 标签化的SQL查询
		/// </summary>
		public string TaggedSqlQuery { get; set; }

		/// <summary>
		/// 视图列集合.
		/// </summary>
		public List<ViewColumnInfo> Columns { get; private set; }
	}
}