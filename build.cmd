@echo off

if not exist .paket\paket.exe (
    del .paket\paket.exe
    dotnet tool install paket --tool-path .paket
)

dotnet build
