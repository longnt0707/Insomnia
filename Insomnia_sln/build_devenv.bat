@echo off

SET SlnName=""
For %%i In (*.sln) Do SET SlnName=%%~ni
echo %SlnName%.sln

call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\VC\Auxiliary\Build\vcvarsall.bat" x64
REM call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\VC\Auxiliary\Build\vcvarsall.bat" x64
REM call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcvarsall.bat" x64

devenv %SlnName%.sln /build "Release|Any CPU" /out %SlnName%_build.log
