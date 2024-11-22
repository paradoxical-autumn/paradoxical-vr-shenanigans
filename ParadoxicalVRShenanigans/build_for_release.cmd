@echo off
set /P "appver=Enter app version (X.Y.Z): "
echo on
dotnet publish -c Release --self-contained -r win-x64 -o .\publish
vpk pack -u ParadoxicalVRShenanigans -v %appver% -p .\publish -e ParadoxicalVRShenanigans.exe