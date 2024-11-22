set /P "appver=Enter app version (X.Y.Z): "
dotnet publish -c Release --self-contained -r win-x64 -o .\publish
vpk pack -u xyz.its-autumn.pvrs -v %appver% -p .\publish -e ParadoxicalVRShenanigans.exe