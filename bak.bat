@echo off
for /d /r . %%d in (bin,obj) do @if exist "%%d" rd /s/q "%%d"
set file=bak_%date:~0,4%%date:~5,2%%date:~8,2%_%time:~0,2%%time:~3,2%%time:~6,2%%time:~9,2%.tar.gz
tar -czf %file% --exclude=%file% --exclude=[Bb]ackup --exclude=.git --exclude=.vs --exclude=.vscode *
exit