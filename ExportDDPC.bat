for /F "tokens=*" %%A in (mapnames_ddpc.txt) do Build\Raymap.exe -batchmode --lvl %%A --mode dd_pc --folder "D:\Games\Donald Duck Quack Attack\Data" --export "D:\Dev\Raymap\exports_dd"
pause