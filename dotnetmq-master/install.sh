#!/bin/bash

DEST_PATH="/mnt/c/Program Files/DotNetMQ/"
SRCE_PATH="./binaries/"

# echo  $DEST_PATH
# echo  $SRCE_PATH

echo cp ./src/DotNetMQ/bin/Debug/DotNetMQ.exe                                               "$SRCE_PATH""DotNetMQ.exe"                ; cp ./src/DotNetMQ/bin/Debug/DotNetMQ.exe                                               "$SRCE_PATH""DotNetMQ.exe"
echo cp ./src/MDSManager/bin/Debug/MDSManager.exe                                           "$SRCE_PATH""MDSManager.exe"              ; cp ./src/MDSManager/bin/Debug/MDSManager.exe                                           "$SRCE_PATH""MDSManager.exe"
echo cp ./src/Tools/MDSServiceProxyGenerator/bin/Debug/MDSServiceProxyGenerator.exe         "$SRCE_PATH""MDSServiceProxyGenerator.exe"; cp ./src/Tools/MDSServiceProxyGenerator/bin/Debug/MDSServiceProxyGenerator.exe         "$SRCE_PATH""MDSServiceProxyGenerator.exe"
echo cp ./src/DotNetMQ/bin/Debug/MDSCommonLib.dll                                           "$SRCE_PATH""MDSCommonLib.dll"            ; cp ./src/DotNetMQ/bin/Debug/MDSCommonLib.dll                                           "$SRCE_PATH""MDSCommonLib.dll"
echo cp ./src/DotNetMQ/bin/Debug/MDSCore.dll                                                "$SRCE_PATH""MDSCore.dll"                 ; cp ./src/DotNetMQ/bin/Debug/MDSCore.dll                                                "$SRCE_PATH""MDSCore.dll"
echo cp ./src/DotNetMQ/bin/Debug/Topshelf.dll                                               "$SRCE_PATH""Topshelf.dll"                ; cp ./src/DotNetMQ/bin/Debug/Topshelf.dll                                               "$SRCE_PATH""Topshelf.dll"

echo cp "$SRCE_PATH""DotNetMQ.exe"                         "$DEST_PATH""DotNetMQ.exe"                ; cp ./src/DotNetMQ/bin/Debug/DotNetMQ.exe                                               "$DEST_PATH""DotNetMQ.exe"
echo cp "$SRCE_PATH""MDSManager.exe"                       "$DEST_PATH""MDSManager.exe"              ; cp ./src/MDSManager/bin/Debug/MDSManager.exe                                           "$DEST_PATH""MDSManager.exe"
echo cp "$SRCE_PATH""MDSServiceProxyGenerator.exe"         "$DEST_PATH""MDSServiceProxyGenerator.exe"; cp ./src/Tools/MDSServiceProxyGenerator/bin/Debug/MDSServiceProxyGenerator.exe         "$DEST_PATH""MDSServiceProxyGenerator.exe"
echo cp "$SRCE_PATH""MDSCommonLib.dll"                     "$DEST_PATH""MDSCommonLib.dll"            ; cp ./src/DotNetMQ/bin/Debug/MDSCommonLib.dll                                           "$DEST_PATH""MDSCommonLib.dll"
echo cp "$SRCE_PATH""MDSCore.dll"                          "$DEST_PATH""MDSCore.dll"                 ; cp ./src/DotNetMQ/bin/Debug/MDSCore.dll                                                "$DEST_PATH""MDSCore.dll"
echo cp "$SRCE_PATH""Topshelf.dll"                         "$DEST_PATH""Topshelf.dll"                ; cp ./src/DotNetMQ/bin/Debug/Topshelf.dll                                               "$DEST_PATH""Topshelf.dll"

