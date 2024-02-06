taskkill -im vpnui.exe -f
"%PROGRAMFILES(x86)%\Cisco\Cisco AnyConnect Secure Mobility Client\vpncli.exe" disconnect
"%PROGRAMFILES(x86)%\Cisco\Cisco AnyConnect Secure Mobility Client\vpncli.exe" state
@pause > nul