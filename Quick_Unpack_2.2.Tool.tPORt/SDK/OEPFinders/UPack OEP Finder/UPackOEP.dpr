// Parts of code written by dj-siba
library UPackOEP;

uses windows, pelib;

var hFile:DWORD;
    e_lfanew:DWORD;
    EXE:WORD;
    i,ad:integer;
    OEP:DWORD;
    bread:dword;
  fjmp1, fjmp2:cardinal;
  jmpbytes: DWORD;

CONST
MAX_SECTION_NUMBER= $10;


TYPE
IMAGE_DIR_ITEM=record
                 VirtualAddress:DWORD;
                 Size:DWORD;
               end;

IMAGE_FILE_HEADER=record
                         Machine:WORD;
                         NumberOfSections:WORD;
                         TimeDateStamp:DWORD;
                         PointerToSymbolTable:DWORD;
                         NumberOfSymbols:DWORD;
                         SizeOfOptionalHeader:WORD;
                         Characteristics:WORD;
                  end;

IMAGE_OPTIONAL_HEADER=record
                             Magic:WORD;
                             MajorLinkerVersion:BYTE;
                             MinorLinkerVersion:BYTE;
                             SizeOfCode:DWORD;
                             SizeOfInitializedData:DWORD;
                             SizeOfUninitializedData:DWORD;
                             AddressOfEntryPoint:DWORD;
                             BaseOfCode:DWORD;
                             BaseOfData:DWORD;
                             ImageBase:DWORD;
                             SectionAlignment:DWORD;
                             FileAlignment:DWORD;
                             MajorOperatingSystemVersion:WORD;
                             MinorOperatingSystemVersion:WORD;
                             MajorImageVersion:WORD;
                             MinorImageVersion:WORD;
                             MajorSubsystemVersion:WORD;
                             MinorSubsystemVersion:WORD;
                             Win32VersionValue:DWORD;
                             SizeOfImage:DWORD;
                             SizeOfHeaders:DWORD;
                             CheckSum:DWORD;
                             Subsystem:WORD;
                             DllCharacteristics:WORD;
                             SizeOfStackReserve:DWORD;
                             SizeOfStackCommit:DWORD;
                             SizeOfHeapReserve:DWORD;
                             SizeOfHeapCommit:DWORD;
                             LoaderFlags:DWORD;
                             NumberOfRvaAndSizes:DWORD;
                             IMAGE_DIRECTORY_ENTRIES:record
                                                    _EXPORT:IMAGE_DIR_ITEM;
                                                    IMPORT:IMAGE_DIR_ITEM;
                                                    RESOURCE:IMAGE_DIR_ITEM;
                                                    EXCEPTION:IMAGE_DIR_ITEM;
                                                    SECURITY:IMAGE_DIR_ITEM;
                                                    BASERELOC:IMAGE_DIR_ITEM;
                                                    DEBUG:IMAGE_DIR_ITEM;
                                                    COPYRIGHT:IMAGE_DIR_ITEM;
                                                    GLOBALPTR:IMAGE_DIR_ITEM;
                                                    TLS:IMAGE_DIR_ITEM;
                                                    CONFIG:IMAGE_DIR_ITEM;
                                                    BOUND_IMPORT:IMAGE_DIR_ITEM;
                                                    IAT:IMAGE_DIR_ITEM;
                                                     end;
                             DUMB:ARRAY [1..24] OF BYTE;
                      end;

SECTION=record
               Name:ARRAY [1..8] OF CHAR;
               VirtualSize:DWORD;
               VirtualAddress:DWORD;
               SizeOfRawData:DWORD;
               PointerToRawData:DWORD;
               PointerToRelocations:DWORD;
               PointerToLinenumbers:DWORD;
               NumberOfRelocations:WORD;
               NumberOfLinenumbers:WORD;
               Characteristics:DWORD;
        end;
VAR
    PE_HEADER:record
                 IMAGE_NT_SIGNATURE:DWORD;
                 FILE_HEADER:IMAGE_FILE_HEADER;
                 OPTIONAL_HEADER:IMAGE_OPTIONAL_HEADER;
          end;
                 SECTION_HEADER:ARRAY [1..MAX_SECTION_NUMBER] of SECTION;


Function RVA2Offset(RVA:DWORD):DWORD;
 var i:integer;
     VirtAddr,VA2,szRawData,ptrRawData:DWORD;
 begin
  for i:=1 to PE_HEADER.FILE_HEADER.NumberOfSections do
   begin
    VirtAddr:=SECTION_HEADER[i].VirtualAddress;
    szRawData:=SECTION_HEADER[i].SizeOfRawData;
    ptrRawData:=SECTION_HEADER[i].PointerToRawData;
    if RVA>=VirtAddr then
     begin
      VA2:=VirtAddr+szRawData;
      if RVA<VA2 then
       begin
        RVA:=RVA-VirtAddr;
        RVA:=RVA+ptrRawData;
       end;
     end;
   end;
  RVA2Offset:=RVA;
 end;


function ShortFinderName:pchar; cdecl;
begin
result:='Upack 0.xx OEP Finder by FEUERRADER';
end;


function GetOEPNow(filname:pchar):DWORD; cdecl;
var
    BRK, EPreal, EP, imagebase,SizeOfImage:cardinal;
    num,epsec:integer;
    cLEN:cardinal;
    COD: ARRAY[1..$1000] of byte;  // $154
    numsec:word;

    si:STARTUPINFO;
    pi:PROCESS_INFORMATION;
    stub: array[1..$2000] of byte;
    stubber:cardinal;
    readed: dword;

begin
result:=0;
if filname='' then
   begin
result:=0;
     Exit;
   end;
 hFile:=CreateFileA(filname, GENERIC_READ + GENERIC_WRITE, FILE_SHARE_READ + FILE_SHARE_WRITE, NIL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, 0);
   if hFile=INVALID_HANDLE_VALUE then
    begin
result:=0;
     Exit;
    end;
  ReadFile(hFile,EXE,2,bread,NIL);
   if EXE<>$5A4D then
    begin
result:=0;
     CloseHandle(hFile);
     Exit;
    end;
   SetFilePointer(hFile,$3C,NIL,FILE_BEGIN);
   ReadFile(hFile,e_lfanew,4,bread,NIL);
   SetFilePointer(hFile,e_lfanew,NIL,FILE_BEGIN);
   ReadFile(hFile,PE_HEADER,SizeOF(PE_HEADER),bread,NIL);
   if PE_HEADER.IMAGE_NT_SIGNATURE<>$00004550 then
    begin
result:=0;
      CloseHandle(hFile);
      Exit;
    end;
   for i:=1 to PE_HEADER.FILE_HEADER.NumberOfSections Do
   ReadFile(hFile,SECTION_HEADER[i],SizeOF(Section),bread,NIL);

   epreal:=PE_HEADER.OPTIONAL_HEADER.AddressOfEntryPoint+PE_HEADER.OPTIONAL_HEADER.ImageBase;
   ImageBase:=PE_HEADER.OPTIONAL_HEADER.ImageBase;
   OEP:=PE_HEADER.OPTIONAL_HEADER.AddressOfEntryPoint+PE_HEADER.OPTIONAL_HEADER.ImageBase;
   ImageBase:=PE_HEADER.OPTIONAL_HEADER.ImageBase;
   EP := RVA2Offset(OEP - ImageBase);
   SizeofImage:=PE_HEADER.OPTIONAL_HEADER.SizeOfImage;
   numsec:=PE_HEADER.FILE_HEADER.NumberOfSections;

   SetFilePointer(hFile,$30,NIL,FILE_BEGIN);
   ReadFile(hFile,COD,SizeOF(COD),bread,NIL);
   cLEN:=bread;

//Upack 0.30 ?
if (cod[1]=$E9)and
   (cod[6]=$42)and
   (cod[7]=$79)and
   (cod[8]=$44) then begin

   SetFilePointer(hFile,$31,NIL,FILE_BEGIN);
   ReadFile(hFile,fjmp1,4,bread,NIL);

   fjmp1:=fjmp1+imagebase+$1030+5;


   CloseHandle(hFile);
   GetStartupInfo(si);
   CreateProcess(filname,nil,nil,nil,FALSE,NORMAL_PRIORITY_CLASS,nil,nil,si,pi);
   WaitForInputIdle(pi.hProcess,INFINITE);
   SuspendThread(pi.hThread);
   stubber:=GlobalAlloc(GMEM_FIXED+GMEM_ZEROINIT,$500);
   ReadProcessMemory(pi.hProcess,pointer(fjmp1),pointer(stubber),$500,readed);
   TerminateProcess(pi.hProcess,0);
   CopyMemory(@stub,pointer(stubber),$500);
   GlobalFree(stubber);

   for i:=1 to $500 do
   if ((stub[i]=$0F)
   and (stub[i+1]=$84)) then begin
      CopyMemory(@fjmp2,@(stub[i+2]),4);
      ad:=i;
      end;

  fjmp1:=fjmp2+ad+fjmp1+5;


  result:=fjmp1;
  Exit;

end else begin
//Upack 0.10 ?
   SetFilePointer(hFile,ep,NIL,FILE_BEGIN);
   ReadFile(hFile,COD,SizeOF(COD),bread,NIL);

    repeat
       i:=i+1;
    until ((COD[i]=$0F) and
           (COD[i+1]=$84)) or (i>=cLEN);


  fjmp1:=i+OEP+1;

   SetFilePointer(hFile,RVA2Offset(fjmp1 - ImageBase),NIL,FILE_BEGIN);
   ReadFile(hFile,fjmp2,4,bread,NIL);

  fjmp2:=fjmp1+fjmp2+4;

  result:=fjmp2;
  CloseHandle(hFile);
  Exit;

end;


end;

exports GetOEPNow,
        ShortFinderName;



begin
end.
