using System;

namespace QinDingTech.PowerDesignerHelper
{
	public class ViewColumnInfo
	{
		private readonly ViewInfo _viewInfo;

		public ViewInfo OwnerViewInfo
		{
			get { return _viewInfo; }
		}

		public ViewColumnInfo(ViewInfo view)
		{
			_viewInfo = view;
		}

		/// <summary>
		/// 列Id
		/// </summary>
		public string ViewColumnId { get; set; }

		/// <summary>
		/// 对象id
		/// </summary>
		public string ObjectId { get; set; }

		/// <summary>
		/// 列名
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 列代码=>表字段名
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
		/// 注释
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// 数据类型
		/// </summary>
		public string DataType { get; set; }

		/// <summary>
		/// 数据长度
		/// </summary>
		public string Length { get; set; }

		/// <summary>
		/// 描述
		/// </summary>
		public string Description { get; set; }
	}
}