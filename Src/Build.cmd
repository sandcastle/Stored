@echo off
pushd %~dp0

if exist Build goto Build
mkdir Build

:Build
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild Build.proj /m /nr:false /v:M /fl
if errorlevel 1 goto BuildFail
goto BuildSuccess

:BuildFail
echo.
echo *** BUILD FAILED ***
goto End

:BuildSuccess
echo.
echo **** BUILD SUCCESSFUL ***
goto end

:End
popd

pause