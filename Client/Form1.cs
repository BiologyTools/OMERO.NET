
using omero;
using omero.api;
using omero.model;
using omero.sys;
using omero.gateway;
using System.Data;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            String hostname = "demo.openmicroscopy.org"; // OMERO server host
            int port = 4064;                             // OMERO server port
            String username = "ErikRepo";           // Your username
            String password = "4uRGxtQ5EE8W";           // Your password

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
        }
    }
}
