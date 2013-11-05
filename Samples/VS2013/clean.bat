rem open cmd prompt at location, run batch file
start for /d /r . %%d in (bin,obj) do @if exist "%%d" rd /s/q "%%d"