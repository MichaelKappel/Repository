$ErrorActionPreference = 'Stop'
$dir='C:\Users\mkapp\AppData\Local\Temp\protocol5-edge-debug3'
New-Item -ItemType Directory -Path $dir -Force | Out-Null
$args='--remote-debugging-port=9225 --user-data-dir="'+$dir+'" --headless=new --disable-gpu about:blank'
$p=Start-Process -FilePath 'C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe' -ArgumentList $args -PassThru
$ws=$null
try {
  Start-Sleep -Seconds 2
  $targets = (& curl.exe -s http://127.0.0.1:9225/json/list | Out-String | ConvertFrom-Json)
  $page = $targets | Where-Object { $_.type -eq 'page' } | Select-Object -First 1
  $ws = [System.Net.WebSockets.ClientWebSocket]::new()
  $ws.ConnectAsync([Uri]$page.webSocketDebuggerUrl,[Threading.CancellationToken]::None).Wait()

  function Send-Cdp([int]$id,[string]$method,$params){
    $obj=@{id=$id;method=$method}
    if($null -ne $params){ $obj.params=$params }
    $json=$obj | ConvertTo-Json -Compress -Depth 20
    $bytes=[Text.Encoding]::UTF8.GetBytes($json)
    $seg=[ArraySegment[byte]]::new($bytes)
    $ws.SendAsync($seg,[System.Net.WebSockets.WebSocketMessageType]::Text,$true,[Threading.CancellationToken]::None).Wait()
  }

  function Receive-Text([int]$timeoutMs){
    $buffer = New-Object byte[] 65536
    $sb = New-Object System.Text.StringBuilder
    $cts = [Threading.CancellationTokenSource]::new($timeoutMs)
    try {
      do {
        $seg=[ArraySegment[byte]]::new($buffer)
        $result=$ws.ReceiveAsync($seg,$cts.Token).Result
        if($result.MessageType -eq [System.Net.WebSockets.WebSocketMessageType]::Close){ return $null }
        [void]$sb.Append([Text.Encoding]::UTF8.GetString($buffer,0,$result.Count))
      } while(-not $result.EndOfMessage)
      return $sb.ToString()
    }
    catch {
      return $null
    }
    finally {
      $cts.Dispose()
    }
  }

  function Receive-UntilId([int]$id,[int]$seconds){
    $end=(Get-Date).AddSeconds($seconds)
    $messages = New-Object System.Collections.Generic.List[string]
    while((Get-Date) -lt $end){
      $msg = Receive-Text 1000
      if([string]::IsNullOrWhiteSpace($msg)){ continue }
      $messages.Add($msg) | Out-Null
      if($msg -match ('"id":' + $id + '(,|})')){ return @{ Match = $msg; Messages = $messages } }
    }
    return @{ Match = $null; Messages = $messages }
  }

  $source = @"
window.__protocol5Errors=[];
window.addEventListener('error', function(e){ window.__protocol5Errors.push({ type:'error', message:e.message, filename:e.filename, lineno:e.lineno, colno:e.colno }); });
window.addEventListener('unhandledrejection', function(e){ var r=e.reason; window.__protocol5Errors.push({ type:'unhandledrejection', message:r && r.message ? r.message : String(r), stack:r && r.stack ? String(r.stack) : '' }); });
var __p5ConsoleError = console.error;
console.error = function(){ try { window.__protocol5Errors.push({ type:'console.error', message:Array.prototype.slice.call(arguments).map(function(x){ try { return typeof x === 'string' ? x : JSON.stringify(x); } catch(_) { return String(x); } }).join(' | ') }); } catch(_) {} return __p5ConsoleError.apply(this, arguments); };
"@

  $expr = @"
JSON.stringify({
  errors: window.__protocol5Errors || [],
  appHtml: document.getElementById('app') ? document.getElementById('app').innerHTML : null,
  errorUi: document.getElementById('blazor-error-ui') ? { hidden: document.getElementById('blazor-error-ui').hidden, display: getComputedStyle(document.getElementById('blazor-error-ui')).display } : null,
  load: document.documentElement.style.getPropertyValue('--blazor-load-percentage'),
  path: location.pathname
})
"@

  Send-Cdp 1 'Runtime.enable' @{}
  Send-Cdp 2 'Page.enable' @{}
  Send-Cdp 3 'Page.addScriptToEvaluateOnNewDocument' @{ source = $source }
  Send-Cdp 4 'Page.navigate' @{ url = 'http://127.0.0.1:5088/calculator' }
  Start-Sleep -Seconds 8
  Send-Cdp 7 'Runtime.evaluate' @{ expression = $expr; returnByValue = $true }
  $result = Receive-UntilId 7 10
  'MATCH:'
  $result.Match
  'MESSAGES:'
  $result.Messages
}
finally {
  if($ws){ $ws.Dispose() }
  if($p -and -not $p.HasExited){ Stop-Process -Id $p.Id -Force }
}
