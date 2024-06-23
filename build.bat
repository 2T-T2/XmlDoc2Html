@echo off

goto %1

:release
csc /nologo^
 /define:RELEASE^
 /out:out\release\XmlDoc2Html.exe^
 /doc:out\doc\XmlDoc2Html.xml^
 /optimize^
 /debug-^
 @cmnopt.txt

goto end

:debug
csc /nologo^
 /define:DEBUG^
 /out:out\debug\XmlDoc2Html.exe^
 /doc:out\doc\XmlDoc2Html.xml^
 /debug:full^
 @cmnopt.txt

goto end

:end
 
