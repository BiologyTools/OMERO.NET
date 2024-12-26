# OMERO.NET
 C# Client for OMERO

# Usage
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