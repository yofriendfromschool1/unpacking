Quick Unpack v2.2 readme
========================

Description
-----------
The program is intended for fast (in a few seconds) unpacking of packers and simple protectors:

!EP (EXE Pack)
32Lite
ABC Crypter
ACProtect (old versions)
AHPacker
Alex Protector
AnslymPacker
ap2k
AREA51 Cryptor
Armadillo (minimal protection)
AsdPack
AsCrypt
ASPack
ASProtect (old versions)
AT4RE aSm Protecter
AverCryptor
BamBam
BeroEXEPacker
CD-Cops
Crunch
CRYPToCRACk's PE Protector
CryptX
DalKrypt
DDeM
depack
DragonArmor
Escargot
ExE Evil
Exe32Pack
EXECrypt
ExeCryptor (old versions)
ExeFog
ExeSax
ExeShield
ExeStealth
eXPressor
EZIP
fEaRz Crypter
FishPE
FreeCryptor
FriCryptor
FSG
Fusion
G!X Protector
HidePE
HidePX
hmimys-Packer
ID Application Protector
JDPack
KByS
kkrunchy
LameCrypt
Manolo
MEW
Minke
MKFPack
MPress
NakedPacker
NeoLite
NME
NoNamePacker
NoodleCrypt
nPack
NsPack
Orien
Pack
PackMan
PECompact
PEDiminisher
PE-PACK
PEncrypt
Pepsi
Perplex PE-Protector
Pestil
PeTite
PEX
PI Cryptor
PKLite32
PollyBox
PolyEnE
Protection Plus
QrYPt0r NuTraL Poly
QuickPack
RCryptor
RLPack
Simple Pack
Sopelka
StealthPE
TeLock (not all versions)
The Best Cryptor
TheMida (minimal protection)
unnamed Scrambler
UPack
UPolyX
UProtector
UPX
USSR
VCrypt
VPack
VPacker
WindOfCrypt
WinKrypt
WinUPack
WWPack32
X-Crypter
XComp
XcR
XXPack
Yoda Crypter
Yoda Protector
YZPack
YZPacker
------------------------------
112 known packers and many others...

Quick Unpack tries to bypass all possible scramblers/obfuscators and restores redirected import. From the version 1.0 the opportunity of unpacking dll is added. From the version 2.0 the attach process feature added which allows to use Quick Unpack as a dumper and import recoverer. Scripts are also supported from version 2.0 which allows unpacking of more complicated protections. This makes Quick Unpack a unique software product which has no similar analogues in the world!

Use force unpacking tick. When the application is run QuickUnpack waits for the OEP breakpoint to trigger. But sometimes this breakpoint may be triggered several times but only the last one is the correct OEP. Using ForceMode option solves this problem. With this option after the application is run QuickUnpack counts breapoint hits and dumps the application only at the last stop. For DLL-files this option is always ticked and allows to restore relocs.

If you have any suggestions or found any bugs please tell me about them. You can post them on the official site: http://qunpack.ahteam.org or you can find me at CRACKLAB.rU forum or exetools.com forum or you can find me at irc://irc.cracklab.ru at #tport or irc://irc.street-creed.com at #tport

The license and rights
----------------------
The list of sources used in coding Quick Unpack:
stripper asprotect unpacker by syd
PEiDLL.dll by Bob
GenOEP.dll by Snaker
for other OEP finders the author is mentioned in the program
Lua scripting language from http://www.lua.org

History of the versions
-----------------------
v2.2
[!] fixed several bugs and made improvements like quickened import processing
[+] RDTSC delta also affects GetTickCount now
[+] updated Lua to 5.1.3
[+] updated UsAr's generic OEP finder
[+] modified Human's generic OEP finder to find OEP in DLLs
[+] modified deroko's generic OEP finder to find OEP in DLLs
[+] added Save log feature to the main menu
[+] added support of different languages, just create your own lng-file and enable it at the preferences window
[+] added delphi initialization table restoration. Be sure to turn this on only for Delphi programs
[+] added several new functions and variables for the scripts

v2.1
[!] fixed many bugs like crash on some applications while restoration of resources
[!] multithreaded applications are now handled properly
[+] added ability to set end of module when tracing import functions. When a reference to import is found it's analysed if it leads to some space outside of the module (not to trace some internal functions). But some packers redirect import to the last section. This option is intended to aid this problem. This is RVA
[+] added ability to put import table at given RVA instead of adding extra section
[+] added ability to set RDTSC delta for RDTSC hook (see more on rdtsc_delta in Scripts.eng.txt)
[+] Load libraries only option added to import recovery methods. this option doesn't actually recover import it just puts 1 import function from every loaded DLL into the import table. thus dump will be loaded with all the necessary libraries and will use old addresses for import functions which were set by a protector. this option can be used if import redirection is too complicated but the dump will stop working after service pack or some other patch installation
[+] Execute functions while tracing import option is added. by default while tracing import functions are not executed but some protectors need result of these functions to operate correctly so this option can be used
[+] Process call xxx/jmp xxx option is added. some protectors change import calls and jumps from call [xxx]/jmp [xxx] to call xxx/jmp xxx. this option is intended to work also with these redirections
[+] added several new functions and variables for the scripts
[+] UsAr's generic OEP finder now supports DLLs
[+] new Vista manifest added

v2.0 final
[!] fixed many bugs like missed import functions
[!] fixed several driver bugs like the one which didn't allow to pass some exceptions
[!] improved export feature now supports invalid functions
[!] many improvements (like 256x256 icon for Vista, thanks to Feuerrader :)) and optimizations (like better memory handling)
[!] now Force.dll doesn't use GenOEP.dll, though some code was borrowed
[+] added so long-waited ability to use scripts. before using scripts it's strongly recommended to read the manual (Scripts.eng.txt file). script examples may be taken from Scripts folder (*.txt files), scripting language Lua manual also can be found there (Lua Manual.html), which parser was embedded in the program. BTW I know that Step button doesn't work like a charm but I wasn't able to make it better
[+] passing parameters to the application added
[+] import list from imprec feature added (now Quick Unpack supports both export and import of import functions in imprec-compatible files this allows to edit some functions or add new ones. keep in mind this option works with normally created files but if you put some garbage or format this file in unusual manner this may cause crash :) I was too lazy to parse the file with care)
[+] attach process feature added (this option allows to choose any module in a process for unpacking and has some features. if in processes listbox a process name is a full path with name you can attach to this process. if it is only name of the file you don't have enough rights to attach. you can't specify the OEP, the instruction the program was stopped is treated as the OEP. to use attach process feature one should load the program in any debugger and manually get to the OEP, when attach to that process with Quick Unpack. keep in mind that for smart import recovery you don't need the program to run, it can just be left in the debugger standing at the breakpoint. but to use smart import recovery with tracer you should put it in the infinite loop (EB FE) and run the program because the tracer uses current thread for tracing. if the program was put in the infinite loop don't forget to restore these two bytes in the dump. when attached tracing import is unreliable and very slow, so it's not recommended to use it). this feature allows to use Quick Unpack as a dumper and import recoverer (my attempt to replace PETools and ImpRec with one program :))
[+] imprec plugin support added (this feature allows to use imprec tracer plugins in Quick Unpack to restore import functions. keep in mind when using attach to process feature the program must be run for the tracer to work)
[+] added UsAr's generic OEP finder. I modified it a bit
[+] added Human's generic OEP finder. I modified it a bit
[+] added deroko's generic OEP finder. I modified if a bit and took the GUI from Human's generic OEP finder. it's sometimes slow but rather powerful and be warned that this finder uses driver and the driver is unloadable till next reboot. uses deroko's Dream of every reverser engine so incompatible with win2k3 and kaspersky. for more information about this engine visit http://deroko.phearless.org
[-] no more old non-generic OEP finders

v1.0 final
Unfortunately Feuerrader has left this project, so from now on I'm (Archer) is the only developer.

[!] several bugs fixed like possible tls corruption when removing sections or possible program crash when reading false relocations
[!] identifies dll according to the flag in the header instead of the extension
[!] improved import tracer doesn't fail to trace emulated functions
[+] overlay append option added
[+] ability to disassemble import functions (if some function was emulated you may use this to identify the function)
[+] ability to add new library (allows to use import functions from the new library when editing import functions. also while loading the library the program stands at the OEP, so you can use your own library to do something with the import or with the program)
[+] ability to edit import functions (allows to fix some import functions by hand. the edit window supports typing function's name on the keyboard along with the choosing it in the listbox)
[+] option to add suspicious functions (allows to add possibly emulated functions to the import table to fix them by hand later using new feature above. be warned that false functions may be also added and they must be removed)
[-] no more imprec.dll (this method was rather buggy, others methods are more powerful, so I decided to remove this one)

v1.0 RC1
[!] Sections are truncated properly
[+] New feature - resource rebuilder and tracer (restores redirected import)! Now it handles simple protectors, ex. Yoda Protector
[!] Critical errors are now reported
[!] Get unloaded properly after crash
[!] Some bugfix and optimized code
[!] Fixed ImpRec method
[!] Fixed Force Mode
[!] The driver has been rewritten with improved rdtsc emulation and changed breakpoints   

v1.0 beta8 [!] Bug fixed with Always on top option
	   [!] Bug fixed with BSOD and unterminated processes
	   [!] Optimized import recovery by syd's method
	   [+] Added export import tree to imprec
	   [+] Updated engine.sys
	   [!] Bug fixed with dll unpacking
	   [!] Bug fixed with old upx versions on dll
	   [!] Kill target button works correctly now
	   [!] View of import table was changed
	   [!] Find object button added

v1.0 beta7 [+] Improved interface
	   [+] Fixed small bugs

v1.0 beta6 [!] Internal build

v1.0 beta5 [+] Support of unpacking UPX 2.0
           [+] New signature list
           [+] New engine from stripper 2.13 b9

v1.0 beta4 internal [!] bugfixes

v1.0 [+] the opportunity of unpacking dlls
     [+] the Opportunity of using plug-ins and own OEP finders is added

v0.7 [!] Based on updated stripper 2.11 rc3 engine => new features
     [+] Force mode added v0.6 [!] Final build
v0.5 [+] Quick Unpack now uses new engine and works through external tracer engine.sys. No debug API is used
     [+] Dump and PE header are very clean after unpacking
     [+] Cool OEP finder for packers
v0.43. [+] The opportunites of ImpREC.dll were added. It will be useful, I think
       [+] Protection from IsDebuggerPresent was added (a tick "Hide unpacker")
       [!] This version is last, I think, as the opportunities of Debug API are fully used

v0.41. [!] Fixed the bug with the unpacked programs compatiblity on the different OS (the bug with RestoreLastError)

v0.4 final. Final bild. All the mistakes were fixed

v0.3. [+] The engine working with the dump is ñompletely re-coded
	  Now the engine from PE Tools by NEOx [uinC] is used
      [+] The system of "clever" dump rebuilding and optimizing its size is supported
	  The engine of the dump rebuilding from PE Tools by NEOx [uinC] was used
      [!] Picture of Quick Unpack was removed :) 
      [+] Packers with damaged headers (e.g. FSG) are supported
      [+] The bugs in the operations with PE files were fixed

v0.2. The first version. All as is

Greetz and thanks
-----------------
tPORt, AHTeam, TSRh, KpTeam, REVENGE, CRACKL@B, ICU members, CoaxCable^CPH, Wild-Wolf and all CPH members on #cph, LaFarge [ICU] for nice music, syd, lord_Phoenix, NCRangeR, Grim Fandango, Aster!x, MozgC, Sten, PolishOX, NEOx, WELL, GPcH, newborn, Funbit, sl0n, Ms-Rem, Bad_guy, dj-siba, =TS=, UsAr, Human, deroko, Errins, Bronco, HandMill, Executioner, 4kusN!ck, BiT-H@ck, Hellsp@wn, HoBleen...