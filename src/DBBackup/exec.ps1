$datetime = Get-Date -Format "yyyyMMdd HH-mm"
pg_dump --file "$datetime.tar" --host "$env:PGHOST" --port "$env:PGPORT" --username "postgres" --format=t --large-objects --verbose "aspnet-ProjectManager"
./azcopy copy './*' $env:CONTAINER --include-pattern '*.tar'
Write-Host "Completed"