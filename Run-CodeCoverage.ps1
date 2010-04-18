& "$env:vsinstalldir\Team Tools\Performance Tools\vsinstr" -coverage GenesisEngine.Specs\bin\x86\Debug\GenesisEngine.exe

& "$env:vsinstalldir\Team Tools\Performance Tools\vsperfcmd" /start:coverage /output:GenesisEngine.coverage
dependencies\Machine.Specifications\mspec GenesisEngine.Specs\bin\x86\Debug\GenesisEngine.Specs.dll
& "$env:vsinstalldir\Team Tools\Performance Tools\vsperfcmd" -shutdown

& .\GenesisEngine.coverage
