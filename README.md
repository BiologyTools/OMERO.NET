# OMERO.NET
OMERO converted to .NET with IKVM.

# Client Usage
```
string hostname = "demo.openmicroscopy.org"; // OMERO server host
int port = 4064;                             // OMERO server port
string username = "username";           // Your username
string password = "password";           // Your password

client omeroClient = null;
try
{
    // Initialize the OMERO client
    omeroClient = new client(hostname, port);
    omeroClient.createSession(username, password);
    int t = 0;
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}
```
# Usage with BioFormats.NET8
Use aliases to specify which Bio-Formats library to use in your project file.
```
<PackageReference Include="OMERO.NET" Version="1.0.4">
	<Aliases>Omero</Aliases>
</PackageReference>
<PackageReference Include="BioFormats.NET8" Version="8.0.1">
	<Aliases>BioFormats</Aliases>
</PackageReference>
```
Then in your C# code.
```
extern alias BioFormats;
using BioFormats::loci.common.services;
//and for OMERO
extern alias Omero;
using Omero::omero.api;
```
