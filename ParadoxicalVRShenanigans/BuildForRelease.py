import dotenv
from datetime import datetime, timezone
import os

PACK_ID = r"ParadoxicalVRShenanigans"
PACK_NAME = r"Paradoxical VR Shenanigans"
PACK_ICON = r"..\Icons\icon.ico"
PACK_AUTHORS = r"paradoxical autumn"

def prompt(question: str):
    response = os.system(f"choice /M \"{question}\" ")

    if response == 2:
        exit()

dotenv.load_dotenv()

prompt("compile?")

assigned_time = datetime.now(timezone.utc)
time_ver_str = fr"{assigned_time.year}.{assigned_time.month}{str(assigned_time.day).rjust(2, "0")}.{assigned_time.hour}{str(assigned_time.minute).rjust(2, "0")}"
print(f"using assigned time: {time_ver_str}")
os.system("dotnet publish -c Release --self-contained -r win-x64 -o ./publish")

prompt("pack?")

os.system("vpk download github --repoUrl https://github.com/paradoxical-autumn/paradoxical-vr-shenanigans")
os.system(f"vpk pack -u \"{PACK_ID}\" -v {time_ver_str} --packTitle \"{PACK_NAME}\" --icon \"{PACK_ICON}\" --packAuthors \"{PACK_AUTHORS}\" -p ./publish -e {PACK_ID}.exe")

prompt("upload as a draft?")

os.system(rf"vpk upload github --repoUrl https://github.com/paradoxical-autumn/paradoxical-vr-shenanigans --token {os.environ['GITHUB_TOKEN']}")
