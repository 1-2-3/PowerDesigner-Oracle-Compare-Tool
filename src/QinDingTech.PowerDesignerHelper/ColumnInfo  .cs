using System;

namespace QinDingTech.PowerDesignerHelper
{
	public class ColumnInfo
	{
		/// <summary>
		/// 所属表
		/// </summary>
		public TableInfo OwnerTable { get; private set; }

		public ColumnInfo(TableInfo ownerTable)
		{
			OwnerTable = ownerTable;
		}

		/// <summary>
		/// 是否主键
		/// </summary>
		public bool IsPrimaryKey
		{
			get
			{
				PdmKey theKey = OwnerTable.PrimaryKey;
				return theKey != null && theKey.ColumnObjCodes.Contains(ColumnId);
			}
		}

		/// <summary>
		/// 列标识
		/// </summary>
		public string ColumnId { get; set; }

		/// <summary>
		/// 对象Id,全局唯一.
		/// </summary>
		public string ObjectId { get; set; }

		/// <summary>
		/// 列名
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 列代码，对应数据库表字段名
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

		public String ShowJpaColumnType()
		{
			if (DataType.ToUpper().Contains("VARCHAR"))
			{
				return "String";
			}
			if (DataType.ToUpper().Contains("INT"))
			{
				return "Long";
			}
			if (DataType.ToUpper().Contains("DATE"))
			{
				return "Date";
			}
		    if (DataType.ToUpper().Contains("TIMESTAMP"))
		    {
                return "Date";
            }
			if (DataType.ToUpper().Contains("NUMERIC"))
			{
				return "BigDecimal";
			}
			if (DataType.ToUpper().Contains("TEXT"))
			{
				return "String";
			}
			else
			{
				return "NA: "+ DataType.ToUpper();
			}
		}

		/// <summary>
		/// 数据长度
		/// </summary>
		public string Length { get; set; }

		/// <summary>
		/// 是否自增量
		/// </summary>
		public bool Identity { get; set; }

		/// <summary>
		/// 是否可空
		/// </summary>
		public bool Mandatory { get; set; }

		/// <summary>
		/// 扩展属性
		/// </summary>
		public string ExtendedAttributesText { get; set; }

		/// <summary>
		/// 物理选项
		/// </summary>
		public string PhysicalOptions { get; set; }

		/// <summary>
		/// 精度
		/// </summary>
		public string Precision { get; set; }

		/// <summary>
		/// 描述
		/// </summary>
		public string Description { get; set; }
	}
}