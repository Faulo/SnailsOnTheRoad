call ".steamcmd/steamcmd.exe" +force_install_dir "%cd%\.windows" +@sSteamCmdForcePlatformType windows +login faulolio +app_update 431730 validate +exit
call ".steamcmd/steamcmd.exe" +force_install_dir "%cd%\.linux" +@sSteamCmdForcePlatformType linux +login faulolio +app_update 431730 validate +exit
call .windows\Aseprite.exe --batch --help >README.txt
pause