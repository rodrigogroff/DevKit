
:loop

cd "C:\Users\Administrator\source\repos\qa_template"

git stash push --include-untracked
git stash drop
git reset --hard
git pull

ECHO n|COMP "C:\Users\Administrator\source\repos\qa_template\back\back_version.txt" "C:\Pipeline\back_version.txt" > %ERRORLEVEL% 

if %ERRORLEVEL% EQU 0 (echo OK) else (
dotnet publish -c Release -r win-x64 --output "C:\Pipeline\Backend\nano_v1" "C:\Users\Administrator\source\repos\qa_template\Back\Master\Master.csproj"
iisreset /stop
xcopy "C:\Pipeline\Backend\nano_v1" "C:\inetpub\nanoServer" /E /D /S /I /Q /Y /F
iisreset /start
)

timeout /t 5

xcopy "C:\Users\Administrator\source\repos\qa_template\back\back_version.txt" "C:\Pipeline\back_version.txt" /Y

ECHO n|COMP "C:\Users\Administrator\source\repos\qa_template\front\nano_v1\front_version.txt" "C:\Pipeline\front_version.txt" > %ERRORLEVEL% 

if %ERRORLEVEL% EQU 0 (echo OK) else (
cd "C:\Users\Administrator\source\repos\qa_template\front\nano_v1"
call npm i
call npm run build
xcopy ".\src\static\js\server.js" .\dist /E /D /S /I /Q /Y /F
xcopy .\src\static\html .\dist /E /D /S /I /Q /Y /F
xcopy .\src\static\css .\dist\src\static\css /E /D  /S /I /Q /Y /F
xcopy .\src\static\img .\dist\src\static\img /E /D /S /I /Q /Y /F
xcopy .\src\static\vendor .\dist\src\static\vendor /E /D /S /I /Q /Y /F
xcopy "C:\Users\Administrator\source\repos\qa_template\front\nano_v1\front_version.txt" "C:\Pipeline\front_version.txt" /Y
net stop NanoJS
net start NanoJS
)

timeout /t 5
goto loop
