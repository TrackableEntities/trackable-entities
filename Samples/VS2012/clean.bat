rem open cmd prompt at location, run batch file
start for /d /r . %%d in (packages,bin,obj) do @if exist "%%d" rd /s/q "%%d"