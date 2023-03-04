:: Check for Python Installation
python --version 2>NUL
if errorlevel 1 goto errorNoPython

:: Reaching here means Python is installed.
:: Execute stuff...
python %1/Scripts/postbuild.py -i "%1/YgoMaster/" -o "%ProgramFiles(x86)%/Steam/steamapps/common/Yu-Gi-Oh!  Master Duel/YgoMaster_Dev"

:: Once done, exit the batch file -- skips executing the errorNoPython section
goto:eof

:errorNoPython
echo.
echo Error^: Python not installed