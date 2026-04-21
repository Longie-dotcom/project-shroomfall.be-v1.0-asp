@echo off

echo ================================
echo   EF CORE MIGRATION TOOL
echo ================================

set /p name=Enter migration name: 

echo Creating migration...
dotnet ef migrations add %name% --project Infrastructure --startup-project API

echo Updating database...
dotnet ef database update --project Infrastructure --startup-project API

echo Done!
pause