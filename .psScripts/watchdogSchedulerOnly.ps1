# ------------------------------------
# Watchdog — проверка окружения
# ------------------------------------

# Настройки
$ProcessesToCheck = @()
$ScheduledTasks = @("1", "1-2", "2")
$MinDiskFreeGB = 3
$MinRAMFreeMB = 500
$Uri = "https://adt.carrothood.ru/api/watchdog"
$PcId = "R1"

Set-StrictMode -Version Latest

function Log($msg) {
    $time = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
    Out-Host -InputObject "$time $msg"
}

function Check(){

    Log "=== Checking ==="

    $checks = @{
        Processes   = $true
        Scheduler   = $true
        Disk        = $true
        User        = $true
        RAM         = $true
    }

    $errors = @{
        Processes   = ""
        Scheduler   = ""
        Disk        = ""
        User        = ""
        RAM         = ""
    }

    # -------------------------
    # 1. Проверка процессов
    # -------------------------

    $missingProcesses = @()
    foreach ($p in $ProcessesToCheck) {
        $running = Get-Process -Name $p -ErrorAction SilentlyContinue
        if (!$running) {
            $missingProcesses += $p
        }
    }

    if($missingProcesses.Count -gt 0)
    {
        $errors["Processes"] = ($missingProcesses -join ", ")
        $checks["Processes"] = $false
    }

    # -------------------------
    # 2. Проверка планировщика
    # -------------------------

    $erroredTasks = @()

    foreach ($task in $ScheduledTasks) {
        try {
            $t = Get-ScheduledTask -TaskName $task -ErrorAction Stop
            if ($t.State -eq "Disabled") {
                $erroredTasks += $task;
            }
        }
        catch {
            $erroredTasks += $task
        }
    }

    if($erroredTasks.Count -gt 0)
    {
        $errors["Scheduler"] = ($erroredTasks -join ", ")
        $checks["Scheduler"] = $false
    }

    # -------------------------
    # 3. Проверка дискового пространства
    # -------------------------

    $drive = Get-PSDrive C
    $freeGB = [math]::Round($drive.Free / 1GB, 2)

    if ($freeGB -lt $MinDiskFreeGB) {
        $checks["Disk"] = $false
        $errors["Disk"] = "($freeGB GB free)"
    }

    # -------------------------
    # 4. Проверка входа в систему
    # -------------------------

    if ((Get-Process logonui -ErrorAction SilentlyContinue) -ne $null) {
        $checks["User"] = $false
        $errors["User"] = "ERROR"
    }

    # -------------------------
    # 5. Проверка свободной RAM
    # -------------------------

    $os = Get-CimInstance Win32_OperatingSystem
    $freeRAM_MB = [math]::Round($os.FreePhysicalMemory / 1024, 0)

    if ($freeRAM_MB -lt $MinRAMFreeMB) {
        $checks["RAM"] = $false
        $errors["RAM"] = "($freeRAM_MB MB free)"
    }

    # -------------------------
    # пауза между проверками
    # -------------------------

    $allOk = $checks.Values | Where-Object { $_ -ne $true } | Measure-Object | Select-Object -Expand Count

    $result = [PSCustomObject]@{
        botId = $PcId
        error  = $allOk -ne 0
        statusChecks = $checks
        errorDescriptions = $errors
    }

    $logInfo = $result | ConvertTo-Json -Depth 5

    Log $logInfo

    Invoke-RestMethod -Uri $Uri `
     -Method Post `
     -ContentType "application/json" `
     -Body $logInfo
}

Log "=== Watchdog ==="

Check