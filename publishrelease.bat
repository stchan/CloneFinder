echo off
if EXIST "%ProgramFiles%\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat" (
    call "%ProgramFiles%\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"
    echo on
    msbuild CloneFinder.sln  -t:Rebuild -p:Configuration=Release -p:Platform=x64 -r
    if NOT EXIST "publish\signed" mkdir "publish\signed"
    del /q "publish\signed\*"
    copy /y  "publish\x64\CloneFinder_x64_*.msi" "publish\signed\"
    cd "publish\signed\"
    signtool sign /sha1 %CodeSignHash% /t http://time.certum.pl /fd sha256 /v CloneFinder_x64_*.msi
) ELSE (
    echo Could not set build tools environment.
    exit 1
)