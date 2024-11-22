@echo off
echo building...
dotnet publish -c Release -r win-x64 -o .\publish
echo done. find it in "publish"
