# gRPC-dotnetcore-play

## Currently using v3.0.0-preview9 sdk 
[.NET Core 3.0 SDKs](https://dotnet.microsoft.com/download/dotnet-core/3.0)  
[SDK 3.0.100-preview9-014004](https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-3.0.100-preview9-windows-x64-installer)  

```
ps> ./build.ps1
ps> docker-compose -f docker-compose.yml up
```

EXPOSED:  https://localhost:4701

```
Microsoft Visual Studio Enterprise 2019 Preview
Version 16.3.0 Preview 3.0
VisualStudio.16.Preview/16.3.0-pre.3.0+29230.61
Microsoft .NET Framework
Version 4.8.03752
```

# INSECURE

Calling gRPC from inside docker-compose or kubernetes is all over http, so the following needs to be set for those calls to succeed.

// This switch must be set before creating the GrpcChannel/HttpClient.
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
