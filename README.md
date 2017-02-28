# PowerDesigner-Oracle-Compare-Tool
Compare PowerDesigner physical model (the pdm file) with Oracle database objects.

用于比对 PowerDesigner 物理模型（pdm文件）与 Oracle 数据库对象之间的差异。可比对的对象目前支持表、视图和存储过程。

解析 pdm 文件的功能使用的是 PowerDesignerAutomation（https://github.com/QindingTech/PowerDesignerAutomation） 项目中的 QinDingTech.PowerDesignerHelper。

PDCompare 是一个控制台项目，命令行使用方法如下：

    PDCompare <pdm文件路径> <Oracle连接字符串> [/summary] [/compareScript]

    指定 /summary 可选参数将只显示汇总信息。
    指定 /compareScript 可选参数将比对存储过程和视图。

    示例：PDCompare "d:\temp\ZenFramework.pdm" "User ID=ZEN;Password=forget;Pooling=true;Data Source=(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = 172.16.4.45)(PORT = 1521)))(CONNECT_DATA =(SERVICE_NAME = onlyorcl)))" /compareScript

（代码写的有点乱，有10个人以上使用我就重构代码好吗( ＊＿＊ ) 

程序使用效果截图：

