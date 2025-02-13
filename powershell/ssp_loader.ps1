# provide path to SSP DLL as argument

$DynAssembly = New-Object System.Reflection.AssemblyName('SSPI3')
$AssemblyBuilder = [AppDomain]::CurrentDomain.DefineDynamicAssembly($DynAssembly, [Reflection.Emit.AssemblyBuilderAccess]::Run)
$ModuleBuilder = $AssemblyBuilder.DefineDynamicModule('SSPI3', $False)

$TypeBuilder = $ModuleBuilder.DefineType('SSPI3.Secur32', 'Public, Class')
$PInvokeMethod = $TypeBuilder.DefinePInvokeMethod('AddSecurityPackage',
    'secur32.dll',
    'Public, Static',
    [Reflection.CallingConventions]::Standard,
    [Int32],
    [Type[]] @([String], [IntPtr]),
    [Runtime.InteropServices.CallingConvention]::Winapi,
    [Runtime.InteropServices.CharSet]::Auto)

$Secur32 = $TypeBuilder.CreateType()

if ([IntPtr]::Size -eq 4) {
    $StructSize = 20
} else {
    $StructSize = 24
}

$StructPtr = [Runtime.InteropServices.Marshal]::AllocHGlobal($StructSize)
[Runtime.InteropServices.Marshal]::WriteInt32($StructPtr, $StructSize)

$DllName = $args[0]
$Secur32::AddSecurityPackage($DllName, $StructPtr)

