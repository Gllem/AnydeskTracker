$Url = "http://localhost:5185/api/watchdog/BotGames"
$TargetDir = "E:\"
$TargetFile = Join-Path $TargetDir "bt1.txt"

# Создаем папку если нет
if (!(Test-Path $TargetDir)) {
    New-Item -ItemType Directory -Path $TargetDir -Force | Out-Null
}

# Скачиваем с заменой
Invoke-WebRequest -Uri $Url -OutFile $TargetFile -UseBasicParsing