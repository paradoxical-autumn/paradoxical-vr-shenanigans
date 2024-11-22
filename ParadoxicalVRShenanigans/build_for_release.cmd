@echo off
set /P "appver=Enter app version (X.Y.Z): "
echo on
:: Compile
dotnet publish -c Release --self-contained -r win-x64 -o .\publish

:: Download older releases
vpk download github --repoUrl https://github.com/paradoxical-autumn/paradoxical-vr-shenanigans

:: Prepare current version.
vpk pack -u ParadoxicalVRShenanigans -v %appver% ^
--packTitle "Paradoxical VR Shenanigans" ^
--icon "..\Icons\icon.ico" ^
--packAuthors "paradoxical autumn" ^
-p .\publish ^
-e ParadoxicalVRShenanigans.exe

@echo off
echo Done. You should upload the files from "publish"
pause