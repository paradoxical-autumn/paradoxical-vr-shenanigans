import dotenv
from datetime import datetime, timezone

PACK_ID = r"ParadoxicalVRShenanigans"
PACK_NAME = r"Paradoxical VR Shenanigans"
PACK_ICON = r"..\Icons\icon.ico"
PACK_AUTHORS = r"paradoxical autumn"

import os

dotenv.load_dotenv()

response = os.system("choice /M \"are you sure you wanna continue?\"")

assigned_time = datetime.now(timezone.utc)
time_ver_str = fr"{assigned_time.year}.{assigned_time.month}{assigned_time.day}.{assigned_time.hour}{assigned_time.minute}"
print(f"using assigned time: {time_ver_str}")
os.system("dotnet publish -c Release --self-contained -r win-x64 -o ./publish")

if response == 2:
    exit()

response = os.system("choice /M \"do you wanna continue to packing and uploading?\"")

if response == 2:
    exit()

os.system("vpk download github --repoUrl https://github.com/paradoxical-autumn/paradoxical-vr-shenanigans")
os.system(f"vpk pack -u \"{PACK_ID}\" -v {time_ver_str} --packTitle \"{PACK_NAME}\" --icon \"{PACK_ICON}\" --packAuthors \"{PACK_AUTHORS}\" -p ./publish -e {PACK_ID}.exe")
os.system(rf"vpk upload github --repoUrl https://github.com/paradoxical-autumn/paradoxical-vr-shenanigans --token {os.environ['GITHUB_TOKEN']}")
