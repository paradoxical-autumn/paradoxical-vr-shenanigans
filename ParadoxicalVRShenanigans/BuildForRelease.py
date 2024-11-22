PACK_ID = "ParadoxicalVRShenanigans"
PACK_NAME = "Paradoxical VR Shenanigans"
PACK_ICON = "..\Icons\icon.ico"
PACK_AUTHORS = "paradoxical autumn"

import os

response = os.system("choice /M \"Are you sure you want to start building?\"")

if response == 2:
    exit()

with open("ParadoxicalVRShenanigans.csproj", "r") as fp:
    lines = fp.readlines()

target_attr_line = ""

for line in lines:
    if "AssemblyVersion" in line:
        target_attr_line = line

target_attr_line = target_attr_line.strip()
target_attr_line = target_attr_line.lstrip("<AssemblyVersion>")
target_attr_line = target_attr_line.rstrip("</AssemblyVersion>")

os.system("dotnet publish -c Release --self-contained -r win-x64 -o .\publish")
os.system("vpk download github --repoUrl https://github.com/paradoxical-autumn/paradoxical-vr-shenanigans")
os.system(f"vpk pack -u \"{PACK_ID}\" -v {target_attr_line} --packTitle \"{PACK_NAME}\" --icon \"{PACK_ICON}\" --packAuthors \"{PACK_AUTHORS}\" -p .\publish -e {PACK_ID}.exe")
