# LoGiC.NET-Deobfuscator
## Deobfuscator for LoGiC.NET Obfuscator (https://github.com/AnErrupTion/LoGiC.NET).

### Usage:
- Drag & Drop Protected Application onto Deobfuscator.

### Supported Protections:
```
Int Encoding:
- Removal of Negative Instructions
- Math (Abs & Min)

String Encryption
Control Flow
Anti De4dot
Proxy Calls (Simplifier? Doesn't fix completely but simplfies them kek. Migth not even work, I don't really know.)
Anti Tamper
Watermark
Renamer
Junk
```

### Note:
- Deobfuscated application will most likely have some unreadable methods because of errors and or might not be able to run.

## Before Deobfuscation:
![bilde](https://user-images.githubusercontent.com/60292167/120541785-9e44ba80-c3ea-11eb-9236-ffcaa0947024.png)

## After Deobfuscation:
![bilde](https://user-images.githubusercontent.com/60292167/120541836-af8dc700-c3ea-11eb-8c78-dca727175111.png)

<br>
This is here so that if you want to use the proxy call simplifier you can uncomment it in Core/Deobfuscator.cs (Not tested with other protections.)

## Before Proxy Call Simplifier:
![bilde](https://user-images.githubusercontent.com/60292167/121065207-ce180780-c7c8-11eb-9f6a-ed008bb3f9f7.png)

## After Proxy Call Simplifier:
![bilde](https://user-images.githubusercontent.com/60292167/121065461-13d4d000-c7c9-11eb-84d2-cabc3a1e8a3d.png)

Should be pretty easy to fix this, but I cba.


## Credits:
0xd4d - <a href="https://github.com/0xd4d/dnlib">dnlib</a> (The Library I chose to use for making this deobfuscator.) <br>
MindSystemm - <a href="https://github.com/MindSystemm/SuperCalculator">Super-Calculator</a> (De4dot Blocks Cleaning Method.) <br>
AnErruption - <a href="https://github.com/AnErrupTion/LoGiC.NET">LoGiC.NET</a> (For Making LoGiC.NET Obfuscator.) <br>
DevT02 - <a href="https://github.com/DevT02/Junk-Remover">Junk-Remover</a> (Remove Useless Nops.) <br>
