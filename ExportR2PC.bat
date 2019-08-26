for /F "tokens=*" %%A in (mapnames_r2pc.txt) do Build\Raymap.exe -batchmode --lvl %%A --mode r2_pc --folder "D:\Games\Rayman 2\Data" --export "D:\Dev\Raymap\exports"
pause