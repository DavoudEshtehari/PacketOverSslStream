# PacketOverSslStream
The aim of this project is to demonstrate a probable dotnet core bug and you will get `Stack Overflow` exception by purpose.

* If you change the `TargetFramework` of the project `ClassLibraryA` to `nercoreapp3.1` from `netcoreapp2.1` or change `TargetFramework` of the project `Demo` to `nercoreapp2.1`, it will work without exception.

- I used Microsoft sample on [Microsoft Documentation](https://docs.microsoft.com/en-us/dotnet/api/system.net.security.sslstream?view=netcore-3.1) and added my changes.

- I describe how to use it with `Visual Studio 2019` and `Windows 10` (You can manage how to use Visual Studio Code and Linux).
- This repository includes two solutions and three projects.
- Before running the projects you need to create a cretificate with private key to use in server application.

## MServerSSL
This project has responsibility to receive streams from client and decrypt it and show it on the command prompt window.
If you can run it successfully with a valid certification you will see `Waiting for a client to connect...` message.

- Run `Visual Studio` as `Administrator` and open this project or run in `Command Prompt` as `Administrator`.
- If you have a certificate file included a private key you can pass it with argument to the MServerSsl at command prompt.
- You can define a certificate in `localmachine` store with your machine name with [makecert](https://docs.microsoft.com/en-us/windows/win32/seccrypto/makecert).

   `[makecert] -sr LocalMachine -ss root -r -n "CN=your machine name" -sky exchange -sk 123456`
   
- Do not forget to replace your certificate `Thumbprint` in `RunServer` function or find by other methods from cetificate store.
- If firewall ask you to grant access to the MServerSSL, approve it.

## PacketOverSslClient
There are two projects on client side. And the purpose of adding `ClassLibraryA` is just for demonstrating the issue.

- If you use a same machine to run the client you do not need manupulate code. It automatically uses the machine name.
- You will pass over from `AuthenticateAsClient` if your server is running with a valid certificate as mentioned above. And you will see the stream information on the server console before seeing the `Stack Overflow` exception on client console.
