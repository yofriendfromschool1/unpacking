import os,sys,inspect,re,dis,json,types

# READ THIS IF UR DEALING WITH PYARMOR RENAME THIS TO "code.py" AND THE OTHER "code.py" to something else so this will work

hexaPattern = re.compile(r'\b0x[0-9A-F]+\b')
def GetAllFunctions(): # get all function in a script
    functionFile = open("dumpedMembers.txt","w+")
    members = inspect.getmembers(sys.modules[__name__]) # the code will take all the members in the __main__ module, the main problem is that it can't dump main code function
    for member in members:
        match = re.search(hexaPattern,str(member[1]))
        if(match):
            functionFile.write("{\"functionName\":\""+str(member[0])+"\",\"functionAddr\":\""+match.group(0)+"\"}\n")
        else:
            functionFile.write("{\"functionName\":\""+str(member[0])+"\",\"functionAddr\":null}\n")
    functionFile.close()

GetAllFunctions()


def __armor_exit__():
	return

gang() # calling function for decrypting it

try:
    print(dis.dis(gang))
except:
    pass
