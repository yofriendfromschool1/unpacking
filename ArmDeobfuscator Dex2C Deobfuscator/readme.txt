So, i was just messing with an apk and found out it was using Arm Pro (t.me/arm_pro) Dex2C, that obfuscates the classes using XOR and deobfuscate it in runtime and executes it by an lib (libarm_protect.so)

Then i made this pretty quick by analysing the lib so i can deobfuscate, modify it, and then just run the deobfuscator again as it will obfuscate because it uses XOR, so i can just replace in the apk and boom.

USAGE:
- Download the zip and extract it
- Get the classes in the assets of the apk
- Put the classes in the "InputDexFiles" folder
- Execute
- Check DeobfuscatedDexFiles Folder

Pretty simple tbh, i dont consider it a big thing just wanted to release this

Ass Source code: src.cpp