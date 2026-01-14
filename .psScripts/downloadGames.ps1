$Url = "https://adt.carrothood.ru/api/watchdog/BotGames"
$TargetDir = "E:\"
$TargetFile = Join-Path $TargetDir "bt1.txt"

if (!(Test-Path $TargetDir)) {
    New-Item -ItemType Directory -Path $TargetDir -Force | Out-Null
}

Invoke-WebRequest -Uri $Url -OutFile $TargetFile -UseBasicParsing