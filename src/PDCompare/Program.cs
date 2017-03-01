using Oracle.DataAccess.Client;
using QinDingTech.PowerDesignerHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PDCompare
{
    class Program
    {
        // xcopy "$(SolutionDir)lib\ODP64\NeedCopyToAppBin\*.*" "$(TargetDir)" /y /c
        static void Main(string[] args)
        {
            if(args.Length < 2)
            {
                Console.WriteLine("PDCompare 用于比对 Power Designer 物理模型(.pdm 文件) 与 Oracle 数据库表结构是否一致。");
                Console.WriteLine("");
                Console.WriteLine("使用方法为：");
                Console.WriteLine("    PDCompare <pdm文件路径> <Oracle连接字符串> [/summary] [/compareScript]");
                Console.WriteLine("");
                Console.WriteLine("    指定 /summary 可选参数将只显示汇总信息。");
                Console.WriteLine("    指定 /compareScript 可选参数将比对存储过程和视图。");
                Console.WriteLine("");
                Console.WriteLine(@"    示例：PDCompare ""d:\temp\ZenFramework.pdm"" ""User ID=ZEN;Password=forget;Pooling=true;Data Source=(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = 172.16.4.45)(PORT = 1521)))(CONNECT_DATA =(SERVICE_NAME = onlyorcl)))""");
                return;
            }

            string connStr = args[1]; // "User ID=ZEN;Password=000000;Pooling=true;Data Source=(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521)))(CONNECT_DATA =(SERVICE_NAME = jcltdb)))";
            string pdmPath = args[0]; // @"Z:\工作资料\代码碎片\Oracle\数据库最佳实践\ZenFramework.pdm";

            bool onlyShowSummary = (from arg in args where arg == "/summary" select arg).Any(); // 是否只显示汇总结果
            bool compareScript = (from arg in args where arg == "/compareScript" select arg).Any(); // 是否比对存储过程和视图

            Log log = new Log();

            // 读取 PD 表结构
            PdmModel pdm = FetchPdmSchema(pdmPath);

            // 获取所有表名
            IList<string> dbTableNames = FetchAllTableNameList(connStr);

            // 获取表列定义
            IDictionary<string, IList<DbColumn>> colDict = FetchAllColumnDict(connStr);

            //
            // 对比表名是否一致
            //
            if(onlyShowSummary == false)
            {
                log.Title("1) 检查表名是否一致...");
            }

            // 是否有 PD 有而 DB 没有的表名
            IList<string> dbLackTables = new List<string>();
            IList<string> identicalTables = new List<string>(); // 两边都有的表名列表
            foreach (var pdTable in pdm.Tables)
            {
                bool isFound = (from t in dbTableNames where t.Nvl().ToUpper() == pdTable.Code.Nvl().ToUpper() select t).Any();

                if(isFound == false)
                {
                    dbLackTables.Add(pdTable.Code.Nvl().ToUpper());
                }
                else
                {
                    identicalTables.Add(pdTable.Code.Nvl().ToUpper());
                }
            }

            if (onlyShowSummary == false)
            {
                if (dbLackTables.Any())
                {
                    log.Error1("> PD 里有而数据库里没有的表：");
                    foreach (var dbLackTable in dbLackTables)
                    {
                        log.Error2(dbLackTable);
                    }
                }
            }

            // 是否有 DB 里有而 PD 里没有的表
            IList<string> pdLackTables = new List<string>();
            foreach (var dbTableName in dbTableNames)
            {
                bool isFound = (from t in pdm.Tables where t.Code.Nvl().ToUpper() == dbTableName.Nvl().ToUpper()
                                select t.Code.Nvl().ToUpper()).Any();

                if(isFound == false)
                {
                    pdLackTables.Add(dbTableName);
                }
            }

            if (onlyShowSummary == false)
            {
                if (pdLackTables.Any())
                {
                    log.Error1("> 数据库里有而 PD 里没有的表：");
                    foreach (var pdLackTable in pdLackTables)
                    {
                        log.Error2(pdLackTable);
                    }
                }

                if (dbLackTables.Any() == false && pdLackTables.Any() == false)
                {
                    log.Sucess1("√ 完全一致");
                }
            }

            //
            // 对比列定义是否一致
            //
            if (onlyShowSummary == false)
            {
                log.Title("");
                log.Title("2) 检查列定义是否一致...");
            }
                
            bool colsAllIdentical = true; // 是否所有列定义都一致
            int unIdenticalColCount = 0; // 不一致的列数量
            foreach (var tableName in identicalTables)
            {
                IList<ColumnInfo> pdCols = (from t in pdm.Tables where t.Code.Nvl().ToUpper() == tableName select t).First().Columns;
                IList<DbColumn> dbCols = colDict[tableName];

                //
                // 对比列名是否一致
                //
                // 是否有 pd 里有而数据库里没有的列
                IList<string> dbLackCols = new List<string>();
                IList<ColumnInfo> identicalCols = new List<ColumnInfo>(); // PD 和数据库中都有的列
                foreach (var pdCol in pdCols)
                {
                    bool isFound = (from t in dbCols where pdCol.Code.Nvl().ToUpper() == t.ColName.Nvl().ToUpper()
                                    select t).Any();

                    if(isFound == false)
                    {
                        dbLackCols.Add(pdCol.Code.Nvl().ToUpper());
                    }
                    else
                    {
                        identicalCols.Add(pdCol);
                    }
                }

                if (dbLackCols.Any())
                {
                    colsAllIdentical = false;
                    unIdenticalColCount += dbLackCols.Count;

                    if (onlyShowSummary == false)
                    {
                        log.Error1(string.Format("> 表 [{0}] PD 里有而数据库里没有的列：", tableName));
                        foreach (var dbLackCol in dbLackCols)
                        {
                            log.Error2(dbLackCol);
                        }
                    }
                }

                // 是否有数据库里有而 PD 里没有的列
                IList<string> pdLackCols = new List<string>();
                foreach (var dbCol in dbCols)
                {
                    bool isFound = (from t in pdCols where t.Code.Nvl().ToUpper() == dbCol.ColName.Nvl().ToUpper()
                                    select t).Any();

                    if(isFound == false)
                    {
                        pdLackCols.Add(dbCol.ColName.Nvl().ToUpper());
                    }
                }

                if (pdLackCols.Any())
                {
                    colsAllIdentical = false;
                    unIdenticalColCount += pdLackCols.Count();

                    if (onlyShowSummary == false)
                    {
                        log.Error1(string.Format("> 表 [{0}] 数据库里有而 PD 里没有的列：", tableName));
                        foreach (var pdLackCol in pdLackCols)
                        {
                            log.Error2(pdLackCol);
                        }
                    }
                }

                // 对比列数据类型是否一致
                IList<string> notIdenticalCols = new List<string>();
                foreach (var pdCol in identicalCols)
                {
                    var dbCol = (from t in dbCols
                                 where pdCol.Code.Nvl().ToUpper() == t.ColName.Nvl().ToUpper()
                                 select t).First();

                    if (dbCol.DataType.Nvl().ToUpper() != pdCol.DataType.Nvl().ToUpper())
                    {
                        notIdenticalCols.Add(string.Format("{0}{1} <=> {2}", 
                            pdCol.Code.Nvl().ToUpper().PadRight(30, ' '), 
                            pdCol.DataType.Nvl().ToUpper().PadRight(14,' '), dbCol.DataType.Nvl().ToUpper()));
                    }
                }

                if (notIdenticalCols.Any())
                {
                    colsAllIdentical = false;
                    unIdenticalColCount += notIdenticalCols.Count;

                    if (onlyShowSummary == false)
                    {
                        log.Error1(string.Format("> 表 [{0}] 数据类型不一致的列：", tableName));
                        foreach (var notIdenticalCol in notIdenticalCols)
                        {
                            log.Error2(notIdenticalCol);
                        }
                    }
                }
            }

            if (colsAllIdentical)
            {
                if (onlyShowSummary == false)
                {
                    log.Sucess1("√ 完全一致");
                }
            }

            IList<string> dbLackSpList = new List<string>(); // PD里有而数据库里没有的存储过程列表
            IList<string> pdLackSpList = new List<string>(); // 数据库里有而PD里没有的存储过程列表
            IList<string> unIdenticalSpList = new List<string>(); // 代码不一致的存储过程列表

            IList<string> dbLackViewList = new List<string>(); // PD里有而数据库里没有的视图列表
            IList<string> pdLackViewList = new List<string>(); // 数据库里有而PD里没有的视图列表
            IList<string> unIdenticalViewList = new List<string>(); // 代码不一致的视图列表

            if (compareScript == true)
            {
                //
                // 比对存储过程和视图
                //
                if(onlyShowSummary == false)
                {
                    log.Title("");
                    log.Title("3) 检查存储过程是否一致...");
                }

                IDictionary<string, string> dbSpDict = FetchProcedureDict(connStr);
                IList<ProcedureInfo> existsSpList = new List<ProcedureInfo>(); // 两边都有的存储过程

                // 是否有PD里有而数据库里没有的存储过程
                foreach (ProcedureInfo procedure in pdm.Procedures)
                {
                    bool isFound = (from t in dbSpDict.Keys
                                    where t.Nvl().ToUpper() == procedure.Code.Nvl().ToUpper()
                                    select t).Any();

                    if(isFound == false)
                    {
                        dbLackSpList.Add(procedure.Code.Nvl().ToUpper());
                    }
                    else
                    {
                        existsSpList.Add(procedure);
                    }
                }

                // 是否有数据库里有而PD里没有的存储过程
                foreach (var spName in dbSpDict.Keys)
                {
                    bool isFound = (from t in pdm.Procedures
                                    where t.Code.Nvl().ToUpper() == spName.Nvl().ToUpper()
                                    select t).Any();

                    if(isFound == false)
                    {
                        pdLackSpList.Add(spName.Nvl().ToUpper());
                    }
                }

                // 是否有代码不一致的存储过程
                foreach (var sp in existsSpList)
                {
                    string dbText = Util.CompressSqlText(dbSpDict[sp.Code.Nvl().ToUpper()]);
                    // 容错
                    if (dbText.Nvl().StartsWith("procedure"))
                    {
                        dbText = "create or replace " + dbText;
                    }

                    string spText = Util.CompressSqlText(sp.Text);
                    if(dbText != spText)
                    {
                        unIdenticalSpList.Add(sp.Code.Nvl().ToUpper());
                    }
                }

                if (onlyShowSummary == false)
                {
                    if (dbLackSpList.Any())
                    {
                        log.Error1("> PD 里有而数据库里没有的存储过程：");
                        foreach (var dbLackSp in dbLackSpList)
                        {
                            log.Error2(dbLackSp);
                        }
                    }

                    if (pdLackSpList.Any())
                    {
                        log.Error1("> 数据库里有而 PD 里没有的存储过程：");
                        foreach (var pdLackSp in pdLackSpList)
                        {
                            log.Error2(pdLackSp);
                        }
                    }

                    if (unIdenticalSpList.Any())
                    {
                        log.Error1("> 代码不一致的存储过程：");
                        foreach (var unIdenticalSp in unIdenticalSpList)
                        {
                            log.Error2(unIdenticalSp);
                        }
                    }

                    if (dbLackSpList.Count == 0 && pdLackSpList.Count == 0 && unIdenticalSpList.Count == 0)
                    {
                        log.Sucess1("√ 完全一致");
                    }
                }

                //
                // 比对视图是否一致
                //
                if (onlyShowSummary == false)
                {
                    log.Title("");
                    log.Title("4) 检查视图是否一致...");
                }

                IDictionary<string, string> dbViewDict = FetchViewDict(connStr);
                IList<ViewInfo> existsViewList = new List<ViewInfo>(); // 两边都有的视图

                // 是否有PD里有而数据库里没有的视图
                foreach (ViewInfo view in pdm.Views)
                {
                    bool isFound = (from t in dbViewDict.Keys
                                    where t.Nvl().ToUpper() == view.Code.Nvl().ToUpper()
                                    select t).Any();

                    if (isFound == false)
                    {
                        dbLackViewList.Add(view.Code.Nvl().ToUpper());
                    }
                    else
                    {
                        existsViewList.Add(view);
                    }
                }

                // 是否有数据库里有而PD里没有的视图
                foreach (var viewName in dbViewDict.Keys)
                {
                    bool isFound = (from t in pdm.Views
                                    where t.Code.Nvl().ToUpper() == viewName.Nvl().ToUpper()
                                    select t).Any();

                    if (isFound == false)
                    {
                        pdLackViewList.Add(viewName.Nvl().ToUpper());
                    }
                }

                // 是否有代码不一致的视图
                foreach (var view in existsViewList)
                {
                    string dbText = Util.CompressSqlText(dbViewDict[view.Code.Nvl().ToUpper()]);

                    string pdText = Util.CompressSqlText(view.ViewSqlQuery);
                    // 容错
                    pdText = new Regex("^.+? as ").Replace(pdText, "");

                    if (dbText.Nvl().EndsWith(";"))
                    {
                        dbText = dbText.Remove(dbText.Length - 1);
                    }

                    if (pdText.Nvl().EndsWith(";"))
                    {
                        pdText = pdText.Remove(pdText.Length - 1);
                    }

                    if (dbText != pdText)
                    {
                        unIdenticalViewList.Add(view.Code.Nvl().ToUpper());
                    }
                }

                if (onlyShowSummary == false)
                {
                    if (dbLackViewList.Any())
                    {
                        log.Error1("> PD 里有而数据库里没有的视图：");
                        foreach (var dbLackView in dbLackViewList)
                        {
                            log.Error2(dbLackView);
                        }
                    }

                    if (pdLackViewList.Any())
                    {
                        log.Error1("> 数据库里有而 PD 里没有的视图：");
                        foreach (var pdLackView in pdLackViewList)
                        {
                            log.Error2(pdLackView);
                        }
                    }

                    if (unIdenticalViewList.Any())
                    {
                        log.Error1("> 代码不一致的视图：");
                        foreach (var unIdenticalView in unIdenticalViewList)
                        {
                            log.Error2(unIdenticalView);
                        }
                    }

                    if (dbLackViewList.Count == 0 && pdLackViewList.Count == 0 && unIdenticalViewList.Count == 0)
                    {
                        log.Sucess1("√ 完全一致");
                    }
                }
            }

            //FileInfo pdmFileInfo = new FileInfo(pdmPath);
            //string logFileDir = Environment.CurrentDirectory + "\\比对记录\\";
            //Directory.CreateDirectory(logFileDir);
            //string logFilePath = logFileDir
            //    + pdmFileInfo.Name.Replace(pdmFileInfo.Extension, string.Empty) 
            //    + "_" + DateTime.Now.ToString("yyyyMMdd") +  ".txt";

            if (onlyShowSummary == false)
            {
                //log.Title(" * 以上内容已输出到 " + logFilePath);

                Console.WriteLine("");
                Console.WriteLine("=== 按任意键退出 ===");
                Console.ReadKey();
            }

            //log.SaveLogFile(logFilePath);

            if (onlyShowSummary == true)
            {
                if(dbLackTables.Count == 0 && pdLackTables.Count == 0 && colsAllIdentical == true
                    && dbLackSpList.Count == 0 && pdLackSpList.Count == 0 && unIdenticalSpList.Count == 0
                    && dbLackViewList.Count == 0 && pdLackViewList.Count == 0 && unIdenticalViewList.Count == 0)
                {
                    log.Sucess1("√ 完全一致");
                }
                else
                {
                    if (compareScript)
                    {
                        log.Error1(string.Format("{0} 个表名不一致。{1} 个列定义不一致。{2} 个存储过程不一致。{3} 个视图不一致。",
                            dbLackTables.Count + pdLackTables.Count, 
                            unIdenticalColCount,
                            dbLackSpList.Count + pdLackSpList.Count + unIdenticalSpList.Count,
                            dbLackViewList.Count + pdLackViewList.Count + unIdenticalViewList.Count));
                    }
                    else
                    {
                        log.Error1(string.Format("{0} 个表名不一致。{1}个列定义不一致。",
                            dbLackTables.Count + pdLackTables.Count, unIdenticalColCount));
                    }
                    
                }
            }
        }

        private static PdmModel FetchPdmSchema(string pdmPath)
        {
            PdmFileReader reader = new PdmFileReader();
            PdmModel model = reader.ReadFromFile(pdmPath);

            return model;
        }

        /// <summary>
        /// 获取所有表名
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        private static IList<string> FetchAllTableNameList(string connStr)
        {
            OracleDataAdapter da = new OracleDataAdapter("select * from tabs t order by t.TABLE_NAME", connStr);
            DataSet tablesDs = new DataSet();
            da.Fill(tablesDs);

            IList<string> result = new List<string>();
            for (int i = 0; i < tablesDs.Tables[0].Rows.Count; i++)
            {
                result.Add(tablesDs.Tables[0].Rows[i]["TABLE_NAME"].ToString());
            }

            return result;
        }

        /// <summary>
        /// 获取所有表的列定义
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        private static IDictionary<string, IList<DbColumn>> FetchAllColumnDict(string connStr)
        {
            OracleDataAdapter da = new OracleDataAdapter(@"select c.TABLE_NAME,
                                                                  c.COLUMN_NAME,
                                                                  case
                                                                    when c.DATA_TYPE = 'DATE' then
                                                                     c.DATA_TYPE
                                                                    when c.DATA_TYPE = 'NUMBER' and c.DATA_SCALE > 0 then
                                                                     c.DATA_TYPE || '(' || c.DATA_PRECISION || ',' || c.DATA_SCALE || ')'
                                                                    when c.DATA_TYPE = 'NUMBER' and c.DATA_SCALE <= 0 then
                                                                     c.DATA_TYPE || '(' || c.DATA_PRECISION || ')'
                                                                    when c.DATA_TYPE = 'CLOB' then
                                                                     c.DATA_TYPE
                                                                    else
                                                                     c.DATA_TYPE || '(' || c.CHAR_COL_DECL_LENGTH || ')'
                                                                  end DATE_TYPE_TEXT
                                                             from cols c
                                                            order by c.TABLE_NAME, c.COLUMN_ID", connStr);
            DataSet ds = new DataSet();
            da.Fill(ds);

            DataTable dt = ds.Tables[0];

            IDictionary<string, IList<DbColumn>> result = new Dictionary<string, IList<DbColumn>>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string tableName = dt.Rows[i]["TABLE_NAME"].ToString();
                string colName = dt.Rows[i]["COLUMN_NAME"].ToString();
                string dataType = dt.Rows[i]["DATE_TYPE_TEXT"].ToString();

                if(result.ContainsKey(tableName) == false)
                {
                    result.Add(tableName, new List<DbColumn>());
                }

                result[tableName].Add(new DbColumn
                {
                    ColName = colName,
                    DataType = dataType
                });
            }

            return result;
        }

        /// <summary>
        /// 获取数据库中的存储过程定义。
        ///     Key: 存储过程名
        ///     Value: 存储过程代码
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        private static IDictionary<string, string> FetchProcedureDict(string connStr)
        {
            OracleDataAdapter da = new OracleDataAdapter(@"select *
                                                             FROM user_source t
                                                            where t.TYPE = 'PROCEDURE'
                                                            order by t.name, t.line", connStr);
            DataSet ds = new DataSet();
            da.Fill(ds);

            DataTable dt = ds.Tables[0];
            IDictionary<string, string> result = new Dictionary<string, string>();

            var rows = ds.Tables[0].AsEnumerable();
            var spLines = (from r in rows group r by r["NAME"].ToString() into g select g);

            foreach (var sp in spLines)
            {
                StringBuilder text = new StringBuilder();
                foreach (var line in sp)
                {
                    text.AppendLine(line["TEXT"].ToString());
                }

                result.Add(sp.Key, text.ToString());
            }

            return result;
        }

        private static IDictionary<string, string> FetchViewDict(string connStr)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();

            OracleDataAdapter da = new OracleDataAdapter(@"select * from user_views t", connStr);

            da.SelectCommand.InitialLONGFetchSize = -1; // 不加这个 LONG 类型读不出来
            DataSet ds = new DataSet();
            da.Fill(ds);

            DataTable dt = ds.Tables[0];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow r = dt.Rows[i];
                result.Add(r["VIEW_NAME"].ToString(), r["TEXT"].ToString());
            }

            return result;
        }
    }    

    /// <summary>
    /// 数据库列定义
    /// </summary>
    class DbColumn
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string ColName { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }
    }

    class Log
    {
        private StringBuilder _log = new StringBuilder();
        private static readonly string INDENT = "    ";

        public void Title(string arg)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(arg);
            _log.AppendLine(arg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void Error1(string arg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(INDENT + arg);
            _log.AppendLine(INDENT + arg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void Error2(string arg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(INDENT + INDENT + arg);
            _log.AppendLine(INDENT + INDENT + arg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void Text1(string arg)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(INDENT + arg);
            _log.AppendLine(INDENT + arg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void Sucess1(string arg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(INDENT + arg);
            _log.AppendLine(INDENT + arg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void SaveLogFile(string logFilePath)
        {
            using (var logFile = File.CreateText(logFilePath))
            {
                logFile.Write(_log.ToString());
                logFile.Flush();
                logFile.Close();
            } 
        }
    }
}
