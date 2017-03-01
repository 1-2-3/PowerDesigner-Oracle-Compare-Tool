using System;
using System.IO;
using System.Xml;

namespace QinDingTech.PowerDesignerHelper
{
	public class PdmFileReader
	{
		//private const string A = "attribute", C = "collection", O = "object";

		//private const string CClasses = "c:Classes";
		//private const string OClass = "o:Class";

		//private const string CAttributes = "c:Attributes";
		//private const string OAttribute = "o:Attribute";

		private const string CTables = "c:Tables";
		//private const string OTable = "o:Table";

		//private const string CColumns = "c:Columns";
		//private const string OColumn = "o:Column";

		//private const string CPrimaryKey = "c:PrimaryKey";

		private const string CViews = "c:Views";
        //private const string OView = "o:View";

        private const string CProcedures = "c:Procedures";

        /// <summary>
        /// 读取指定Pdm文件的实体集合
        /// </summary>
        /// <param name="pdmFile">Pdm文件名(全路径名)</param>
        /// <returns>实体集合</returns>
        public PdmModel ReadFromFile(string pdmFile)
		{
            //加载文件. 使用只读形式打开PDM文件，这样就不会因为已经使用PowerDesigner打开PDM文件而导致抛异常了
            FileStream fs = new FileStream(pdmFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            var xmlDoc = new XmlDocument();
			xmlDoc.Load(fs);
			//必须增加xml命名空间管理，否则读取会报错.
			var xmlnsManager = new XmlNamespaceManager(xmlDoc.NameTable);
			xmlnsManager.AddNamespace("a", "attribute");
			xmlnsManager.AddNamespace("c", "collection");
			xmlnsManager.AddNamespace("o", "object");
			PdmModel theModel = new PdmModel();

			//读取所有表节点
			XmlNodeList xnTablesList = xmlDoc.SelectNodes("//" + CTables, xmlnsManager);
			if (xnTablesList != null)
				foreach (XmlNode xmlTables in xnTablesList)
				{
					foreach (XmlNode xnTable in xmlTables.ChildNodes)
					{
						//排除快捷对象.
						if (xnTable.Name != "o:Shortcut")
						{
							theModel.Tables.Add(GetTable(xnTable));
						}
					}
				}
			//读取所有视图节点.
			XmlNodeList xnViewsList = xmlDoc.SelectNodes("//" + CViews, xmlnsManager);
			if (xnViewsList != null)
				foreach (XmlNode xmlViews in xnViewsList)
				{
					foreach (XmlNode xnView in xmlViews.ChildNodes)
					{
						theModel.Views.Add(GetView(xnView));
					}
				}

            // 读取所有存储过程节点
            XmlNodeList xnProcedureList = xmlDoc.SelectNodes("//" + CProcedures, xmlnsManager);
            if(xnProcedureList != null)
            {
                foreach (XmlNode xnProcedures in xnProcedureList)
                {
                    foreach (XmlNode xnProcedure in xnProcedures.ChildNodes)
                    {
                        theModel.Procedures.Add(GetProcedure(xnProcedure));
                    }
                }
            }

            return theModel;
		}

        private ProcedureInfo GetProcedure(XmlNode xnProcedure)
        {
            ProcedureInfo procedure = new ProcedureInfo();
            XmlElement xe = (XmlElement)xnProcedure;

            XmlNodeList xnTProperty = xe.ChildNodes;
            foreach (XmlNode xnP in xnTProperty)
            {
                switch (xnP.Name)
                {
                    case "a:Name":
                        procedure.Name = xnP.InnerText;
                        break;

                    case "a:Code":
                        procedure.Code = xnP.InnerText;
                        break;

                    case "a:Procedure.Text":
                        procedure.Text = xnP.InnerText;
                        break;
                }
            }
            return procedure;
        }

        //初始化"o:View"的节点
        private ViewInfo GetView(XmlNode xnView)
		{
			ViewInfo theView = new ViewInfo();
			XmlElement xe = (XmlElement)xnView;
			theView.ViewId = xe.GetAttribute("Id");
			XmlNodeList xnTProperty = xe.ChildNodes;
			foreach (XmlNode xnP in xnTProperty)
			{
				switch (xnP.Name)
				{
					case "a:ObjectID":
						theView.ObjectId = xnP.InnerText;
						break;

					case "a:Name":
						theView.Name = xnP.InnerText;
						break;

					case "a:Code":
						theView.Code = xnP.InnerText;
						break;

					case "a:CreationDate":
						theView.CreationDate = String2DateTime(xnP.InnerText);
						break;

					case "a:Creator":
						theView.Creator = xnP.InnerText;
						break;

					case "a:ModificationDate":
						theView.ModificationDate = String2DateTime(xnP.InnerText);
						break;

					case "a:Modifier":
						theView.Modifier = xnP.InnerText;
						break;

					case "a:Comment":
						theView.Comment = xnP.InnerText;
						break;

					case "a:Description":
						theView.Description = xnP.InnerText;
						break;

					case "a:View.SQLQuery":
						theView.ViewSqlQuery = xnP.InnerText;
						break;

					case "a:TaggedSQLQuery":
						theView.TaggedSqlQuery = xnP.InnerText;
						break;

					case "c:Columns":
						InitColumns(xnP, theView);
						break;
				}
			}
			return theView;
		}

		//初始化"o:Table"的节点
		private TableInfo GetTable(XmlNode xnTable)
		{
			TableInfo mTable = new TableInfo();
			XmlElement xe = (XmlElement)xnTable;
			mTable.TableId = xe.GetAttribute("Id");
			XmlNodeList xnTProperty = xe.ChildNodes;
			foreach (XmlNode xnP in xnTProperty)
			{
				switch (xnP.Name)
				{
					case "a:ObjectID":
						mTable.ObjectId = xnP.InnerText;
						break;

					case "a:Name":
						mTable.Name = xnP.InnerText;
						break;

					case "a:Code":
						mTable.Code = xnP.InnerText;
						break;

					case "a:CreationDate":
						mTable.CreationDate = String2DateTime(xnP.InnerText);
						break;

					case "a:Creator":
						mTable.Creator = xnP.InnerText;
						break;

					case "a:ModificationDate":
						mTable.ModificationDate = String2DateTime(xnP.InnerText);
						break;

					case "a:Modifier":
						mTable.Modifier = xnP.InnerText;
						break;

					case "a:Comment":
						mTable.Comment = xnP.InnerText;
						break;

					case "a:PhysicalOptions":
						mTable.PhysicalOptions = xnP.InnerText;
						break;

					case "c:Columns":
						InitColumns(xnP, mTable);
						break;

					case "c:Keys":
						InitKeys(xnP, mTable);
						break;

					case "c:PrimaryKey":
						InitPrimaryKey(xnP, mTable);
						break;

					case "a:Description":
						mTable.Description = xnP.InnerText;
						break;
				}
			}
			return mTable;
		}

		//PDM文件中的日期格式采用的是当前日期与1970年1月1日8点之差的秒树来保存.
		private DateTime _baseDateTime = new DateTime(1970, 1, 1, 8, 0, 0);

		private DateTime String2DateTime(string dateString)
		{
			Int64 theTicker = Int64.Parse(dateString);
			return _baseDateTime.AddSeconds(theTicker);
		}

		//初始化"c:Columns"的节点
		private void InitColumns(XmlNode xnColumns, TableInfo pTable)
		{
			foreach (XmlNode xnColumn in xnColumns)
			{
				pTable.AddColumn(GetColumn(xnColumn, pTable));
			}
		}

		//初始化"c:Columns"的节点
		private void InitColumns(XmlNode xnColumns, ViewInfo pView)
		{
			foreach (XmlNode xnColumn in xnColumns)
			{
				pView.Columns.Add(GetColumn(xnColumn, pView));
			}
		}

		//初始化c:Keys"的节点
		private void InitKeys(XmlNode xnKeys, TableInfo pTable)
		{
			foreach (XmlNode xnKey in xnKeys)
			{
				pTable.AddKey(GetKey(xnKey, pTable));
			}
		}

		//初始化c:PrimaryKey"的节点
		private void InitPrimaryKey(XmlNode xnPrimaryKey, TableInfo pTable)
		{
			pTable.PrimaryKeyRefCode = GetPrimaryKey(xnPrimaryKey);
		}

		private static Boolean ConvertToBooleanPg(Object obj)
		{
			if (obj != null)
			{
				string mStr = obj.ToString();
				mStr = mStr.ToLower();
				if ((mStr.Equals("y") || mStr.Equals("1")) || mStr.Equals("true"))
				{
					return true;
				}
			}
			return false;
		}

		private ColumnInfo GetColumn(XmlNode xnColumn, TableInfo ownerTable)
		{
			ColumnInfo mColumn = new ColumnInfo(ownerTable);
			XmlElement xe = (XmlElement)xnColumn;
			mColumn.ColumnId = xe.GetAttribute("Id");
			XmlNodeList xnCProperty = xe.ChildNodes;
			foreach (XmlNode xnP in xnCProperty)
			{
				switch (xnP.Name)
				{
					case "a:ObjectID":
						mColumn.ObjectId = xnP.InnerText;
						break;

					case "a:Name":
						mColumn.Name = xnP.InnerText;
						break;

					case "a:Code":
						mColumn.Code = xnP.InnerText;
						break;

					case "a:CreationDate":
						mColumn.CreationDate = String2DateTime(xnP.InnerText);
						break;

					case "a:Creator":
						mColumn.Creator = xnP.InnerText;
						break;

					case "a:ModificationDate":
						mColumn.ModificationDate = String2DateTime(xnP.InnerText);
						break;

					case "a:Modifier":
						mColumn.Modifier = xnP.InnerText;
						break;

					case "a:Comment":
						mColumn.Comment = xnP.InnerText;
						break;

					case "a:DataType":
						mColumn.DataType = xnP.InnerText;
						break;

					case "a:Length":
						mColumn.Length = xnP.InnerText;
						break;

					case "a:Identity":
						mColumn.Identity = ConvertToBooleanPg(xnP.InnerText);
						break;

					case "a:Mandatory":
						mColumn.Mandatory = ConvertToBooleanPg(xnP.InnerText);
						break;

					case "a:PhysicalOptions":
						mColumn.PhysicalOptions = xnP.InnerText;
						break;

					case "a:ExtendedAttributesText":
						mColumn.ExtendedAttributesText = xnP.InnerText;
						break;

					case "a:Precision":
						mColumn.Precision = xnP.InnerText;
						break;
				}
			}
			return mColumn;
		}

		private ViewColumnInfo GetColumn(XmlNode xnColumn, ViewInfo ownerView)
		{
			ViewColumnInfo mColumn = new ViewColumnInfo(ownerView);
			XmlElement xe = (XmlElement)xnColumn;
			mColumn.ViewColumnId = xe.GetAttribute("Id");
			XmlNodeList xnCProperty = xe.ChildNodes;
			foreach (XmlNode xnP in xnCProperty)
			{
				switch (xnP.Name)
				{
					case "a:ObjectID":
						mColumn.ObjectId = xnP.InnerText;
						break;

					case "a:Name":
						mColumn.Name = xnP.InnerText;
						break;

					case "a:Code":
						mColumn.Code = xnP.InnerText;
						break;

					case "a:CreationDate":
						mColumn.CreationDate = String2DateTime(xnP.InnerText);
						break;

					case "a:Creator":
						mColumn.Creator = xnP.InnerText;
						break;

					case "a:ModificationDate":
						mColumn.ModificationDate = String2DateTime(xnP.InnerText);
						break;

					case "a:Modifier":
						mColumn.Modifier = xnP.InnerText;
						break;

					case "a:Comment":
						mColumn.Comment = xnP.InnerText;
						break;

					case "a:DataType":
						mColumn.DataType = xnP.InnerText;
						break;

					case "a:Length":
						mColumn.Length = xnP.InnerText;
						break;

					case "a:Precision":
						mColumn.Description = xnP.InnerText;
						break;
				}
			}
			return mColumn;
		}

		private string GetPrimaryKey(XmlNode xnKey)
		{
			XmlElement xe = (XmlElement)xnKey;
			if (xe.ChildNodes.Count <= 0) return "";
			XmlElement theKp = (XmlElement)xe.ChildNodes[0];
			return theKp.GetAttribute("Ref");
		}

		private void InitKeyColumns(XmlNode xnKeyColumns, PdmKey key)
		{
			XmlElement xe = (XmlElement)xnKeyColumns;
			XmlNodeList xnKProperty = xe.ChildNodes;
			foreach (XmlNode xnP in xnKProperty)
			{
				string theRef = ((XmlElement)xnP).GetAttribute("Ref");
				key.AddColumnObjCode(theRef);
			}
		}

		private PdmKey GetKey(XmlNode xnKey, TableInfo ownerTable)
		{
			PdmKey mKey = new PdmKey(ownerTable);
			XmlElement xe = (XmlElement)xnKey;
			mKey.KeyId = xe.GetAttribute("Id");
			XmlNodeList xnKProperty = xe.ChildNodes;
			foreach (XmlNode xnP in xnKProperty)
			{
				switch (xnP.Name)
				{
					case "a:ObjectID":
						mKey.ObjectId = xnP.InnerText;
						break;

					case "a:Name":
						mKey.Name = xnP.InnerText;
						break;

					case "a:Code":
						mKey.Code = xnP.InnerText;
						break;

					case "a:CreationDate":
						mKey.CreationDate = String2DateTime(xnP.InnerText);
						break;

					case "a:Creator":
						mKey.Creator = xnP.InnerText;
						break;

					case "a:ModificationDate":
						mKey.ModificationDate = String2DateTime(xnP.InnerText);
						break;

					case "a:Modifier":
						mKey.Modifier = xnP.InnerText;
						break;

					case "c:Key.Columns":
						InitKeyColumns(xnP, mKey);
						break;
				}
			}
			return mKey;
		}
	}
}