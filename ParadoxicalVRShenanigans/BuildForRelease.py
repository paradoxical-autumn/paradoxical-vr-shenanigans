import dotenv

PACK_ID = r"ParadoxicalVRShenanigans"
PACK_NAME = r"Paradoxical VR Shenanigans"
PACK_ICON = r"..\Icons\icon.ico"
PACK_AUTHORS = r"paradoxical autumn"

import os

dotenv.load_dotenv()

response = os.system("choice /M \"are you sure you wanna continue?\"")

if response == 2:
    exit()

os.system('\"C:/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin/MSBuild.exe\" -p:Configuration=Release;OutputPath=./publish')

with open("version.cs", "r") as fp:
    lines = fp.readlines()

target_attr_line = ""

for line in lines:
    if "AssemblyVersion" in line:
        target_attr_line = line

target_attr_line = target_attr_line.strip()
target_attr_line = target_attr_line.lstrip('[assembly: AssemblyVersion("')
target_attr_line = target_attr_line.rstrip('")]')

print(f"found version {target_attr_line}")
response = os.system("choice /M \"do you wanna continue to packing and uploading?\"")

if response == 2:
    exit()

os.system("vpk download github --repoUrl https://github.com/paradoxical-autumn/paradoxical-vr-shenanigans")
os.system(f"vpk pack -u \"{PACK_ID}\" -v {target_attr_line} --packTitle \"{PACK_NAME}\" --icon \"{PACK_ICON}\" --packAuthors \"{PACK_AUTHORS}\" -p ./publish -e {PACK_ID}.exe")
os.system(rf"vpk upload github --repoUrl https://github.com/paradoxical-autumn/paradoxical-vr-shenanigans --token {os.environ['GITHUB_TOKEN']}")
