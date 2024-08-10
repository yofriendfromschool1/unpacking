import os
import pathlib
import base64
#credits to xxvolt on dc for giving me his old source i implemented some things in here like the search function for the token

os.system("cls")

# Asks u for the file name like source_prepared.exe after that drag that program into the cmd
filename = input("Filename please: ")
pysilon_filepath = input("Drag your file here: ")

def copy_file_to_current_folder():
    current_path = pathlib.Path(__file__).parent.absolute()
    command = f'xcopy "{pysilon_filepath}" "{current_path}"'
    os.system(command)

def extract_pyc_and_process():
    os.system(f"python p.py {filename}")
    extracted_pyc_path = f"{filename}_extracted/source_prepared.pyc"
    os.system(f"pycdas.exe {extracted_pyc_path} -o output.txt")

def add_mti_marker(text):
    return text.replace("MTI", "\nMTI")

def get_encrypted_token(file_path):
    # Extract the encrypted token from the output file
    with open(file_path, "r", encoding="utf-8") as file:
        lines = file.readlines()

    # Locate the line containing the token
    for i, line in enumerate(lines):
        if "STORE_NAME" in line and "bot_tokens" in line:
            if i >= 2:
                return lines[i - 2].strip()
    return None

def main():
    copy_file_to_current_folder()
    extract_pyc_and_process()

    # look trough the output file to retrieve the token
    raw_token = get_encrypted_token("output.txt")
    if raw_token:
        raw_token = raw_token[53:]
        print("Raw token: " + raw_token)

        # Reverse the token, decode it, and display the result
        reversed_token = raw_token[::-1]
        decoded_token = base64.b64decode(reversed_token).decode("utf-8")

        os.system("cls")
        print(decoded_token)

        with open("decodedtoken.txt", "w", encoding="utf-8") as file:
            file.write(decoded_token)

        os.system(f'del /S /Q "{filename}"')
        os.system(f'rmdir /S /Q "{filename}_extracted"')
        os.system("del /S /Q output.txt")

if __name__ == "__main__":
    main()
