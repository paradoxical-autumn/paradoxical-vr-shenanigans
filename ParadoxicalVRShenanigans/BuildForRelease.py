import dotenv
from datetime import datetime, timezone
import os

PACK_ID = r"ParadoxicalVRShenanigans"
PACK_NAME = r"PVRS"
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
print(f"guessing assigned time: {time_ver_str}, this may be out by a minute if you compiled close to the changeover.")
os.system("dotnet publish -c Release --self-contained -r win-x64 -o ./publish")

print(f"\nit is strongly recommended to open the app at ./publish to check the build time before continuing and compare the reported version to the version listed below.\n    {time_ver_str}\n")
prompt("pack?")

os.system("vpk download github --repoUrl https://github.com/paradoxical-autumn/paradoxical-vr-shenanigans")
os.system(f"vpk pack -u \"{PACK_ID}\" -v {time_ver_str} --packTitle \"{PACK_NAME}\" --icon \"{PACK_ICON}\" --packAuthors \"{PACK_AUTHORS}\" -p ./publish -e {PACK_ID}.exe")

prompt("upload as a draft?")

os.system(rf"vpk upload github --repoUrl https://github.com/paradoxical-autumn/paradoxical-vr-shenanigans --token {os.environ['GITHUB_TOKEN']}")
