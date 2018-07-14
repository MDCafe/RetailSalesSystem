@echo off
set MYSQL_USER=root
set MYSQL_PASS="Good4Now!@#"	
set MYSQL_CONN=-u%MYSQL_USER% -p%MYSQL_PASS%
set MYSQL_DATA="C:\ProgramData\MySQL\MySQL Server 5.7\Data"
set MYSQL_BACK=D:\MySQLBackup
REM @echo "Before Mysql Command">"output.txt"
mysql %MYSQL_CONN% -ANe"FLUSH TABLES; SET GLOBAL innodb_fast_shutdown=0;" >> "D:\output.txt" 2>&1
REM @echo "Stopping MYSQL Service">>"output.txt"
net stop mysql57 >> "D:\output.txt" 2>&1
REM @echo "Copying files..">>"output.txt"
xcopy /s %MYSQL_DATA%\*.* %MYSQL_BACK%\ >> "D:\output.txt"
net start mysql57 >> "D:\output.txt" 2>&1
REM @echo "Service Started">>"output.txt"
