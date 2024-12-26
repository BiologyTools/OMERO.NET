
using loci.common.services;
using omero;
using omero.api;
using omero.gateway;
using omero.gateway.facility;
using omero.gateway.model;
using omero.log;
using omero.model;
using System.Data;
using BioLib;
using AForge;
using System.Drawing.Imaging;
namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string hostname = "demo.openmicroscopy.org"; // OMERO server host
            int port = 4064;                             // OMERO server port
            string username = "ErikRepo";           // Your username
            string password = "GohkpOeOHC";           // Your password

            client omeroClient = null;
            List<BioImage> bms = new List<BioImage>();
            try
            {
                BioImage b = new BioImage("test.ome.tif");
                // Initialize the OMERO client
                omeroClient = new client(hostname, port);
                var session = omeroClient.createSession(username, password);
                var meta = session.getMetadataService();

                ExperimenterGroupI sec = (ExperimenterGroupI)session.getSecurityContexts().get(0);
                RLong id = sec.getId();
                // Initialize OMERO client and gateway
                var gateway = new Gateway(new SimpleLogger());
                LoginCredentials credentials = new LoginCredentials();
                credentials.getServer().setHostname(hostname);
                credentials.getServer().setPort(port);
                credentials.getUser().setUsername(username);
                credentials.getUser().setPassword(password);
                ExperimenterData dat = gateway.connect(credentials);
                SecurityContext sc = new SecurityContext(id.getValue());
                // Access BrowseFacility
                java.lang.Class cl = java.lang.Class.forName("omero.gateway.facility.BrowseFacility");
                BrowseFacility facility = (BrowseFacility)gateway.getFacility(cl);
                java.lang.Class cl2 = java.lang.Class.forName("omero.gateway.facility.MetadataFacility");
                MetadataFacility mf = (MetadataFacility)gateway.getFacility(cl2);
                var fols = facility.getFolders(sc);
                var d = facility.getDatasets(sc);
                var uims = facility.getUserImages(sc);
                var itr = uims.iterator();
                java.util.List li = new java.util.ArrayList();
                java.util.ArrayList imgs = new java.util.ArrayList();

                bool chans = false;
                do
                {
                    java.util.ArrayList list = new java.util.ArrayList();
                    omero.gateway.model.ImageData o = (omero.gateway.model.ImageData)itr.next();
                    string name = o.getName();
                    RawPixelsStorePrx store = gateway.getPixelsStore(sc);
                    PixelsData pd = o.getDefaultPixels();
                    int xs = pd.getSizeX();
                    int ys = pd.getSizeY();
                    int zs = pd.getSizeZ();
                    int cs = pd.getSizeC();
                    int ts = pd.getSizeT();
                    long ind = o.getId();
                    long ll = pd.getId();
                    list.add(java.lang.Long.valueOf(ind));
                    try
                    {
                        var acq = mf.getChannelData(sc, ind);
                        for (int i = 0; i < acq.size(); i++)
                        {
                            var ch = acq.get(i);
                            var chan = (omero.model.Channel)ch;
                            var lch = chan.getLogicalChannel();
                            AForge.Color col = AForge.Color.FromArgb(chan.getRed().getValue(), chan.getGreen().getValue(), chan.getBlue().getValue());
                            var px = pd.asPixels().getPixelsType();
                            int bits = px.getBitSize().getValue();
                            AForge.PixelFormat pxx = AForge.PixelFormat.Format8bppIndexed;
                            if (bits == 8)
                                pxx = AForge.PixelFormat.Format8bppIndexed;
                            else if (bits < 16 || bits == 16)
                                pxx = AForge.PixelFormat.Format16bppGrayScale;
                            else if (bits == 24)
                                pxx = AForge.PixelFormat.Format24bppRgb;
                            else if (bits == 32)
                                pxx = AForge.PixelFormat.Format32bppArgb;
                            else if (bits > 32)
                                pxx = AForge.PixelFormat.Format48bppRgb;
                            AForge.Channel cch = null;
                            if (pxx == AForge.PixelFormat.Format8bppIndexed || pxx == AForge.PixelFormat.Format16bppGrayScale)
                                cch = new AForge.Channel(i, bits, 1);
                            else if (pxx == AForge.PixelFormat.Format24bppRgb || pxx == AForge.PixelFormat.Format48bppRgb)
                                cch = new AForge.Channel(i, bits, 3);
                            else if(pxx == AForge.PixelFormat.Format32bppArgb)
                                cch = new AForge.Channel(i, bits, 4);
                            cch.Fluor = lch.getFluor().getValue();
                            cch.Emission = (int)lch.getEmissionWave().getValue();
                            cch.Color = col;
                            b.Channels.Add(cch);
                        }
                    }
                    catch (Exception exx)
                    {

                    }
                    var cha = mf.getChannelData(sc, ll);

                    var stage = o.asImage().getStageLabel();
                    try
                    {
                        for (int l = 0; l < store.getResolutionLevels(); l++)
                        {
                            store.setResolutionLevel(l);
                            ResolutionDescription[] res = store.getResolutionDescriptions();
                            AForge.PixelFormat px = AForge.PixelFormat.Format8bppIndexed;
                            Pixels pxs = pd.asPixels();
                            var pxt = pxs.getPixelsType();
                            int bits = pxt.getBitSize().getValue();
                            if (bits == 8)
                                px = AForge.PixelFormat.Format8bppIndexed;
                            else if (bits < 16 || bits == 16)
                                px = AForge.PixelFormat.Format16bppGrayScale;
                            else if (bits > 16 && bits <= 32)
                                px = AForge.PixelFormat.Format32bppArgb;
                            else if (bits > 32)
                                px = AForge.PixelFormat.Format48bppRgb;
                            double pxx = pxs.getPhysicalSizeX().getValue();
                            double pyy = pxs.getPhysicalSizeX().getValue();
                            double pzz = pxs.getPhysicalSizeX().getValue();
                            if (stage.isLoaded())
                            {
                                Length? sxx = stage.getPositionX();
                                Length? syy = stage.getPositionY();
                                Length? szz = stage.getPositionZ();
                                b.Resolutions.Add(new Resolution(xs, ys, px, pxx, pyy, pzz, sxx.getValue(), syy.getValue(), szz.getValue()));
                            }
                            else
                                b.Resolutions.Add(new Resolution(xs, ys, px, pxx, pyy, pzz, 0, 0, 0));
                        }
                    }
                    catch (Exception ex)
                    {
                        AForge.PixelFormat px = AForge.PixelFormat.Format8bppIndexed;
                        var pxt = pd.asPixels().getPixelsType();
                        int bits = pxt.getBitSize().getValue();
                        if (bits == 8)
                            px = AForge.PixelFormat.Format8bppIndexed;
                        else if (bits < 16 || bits == 16)
                            px = AForge.PixelFormat.Format16bppGrayScale;
                        else if (bits > 16 && bits <= 32)
                            px = AForge.PixelFormat.Format32bppArgb;
                        else if (bits > 32)
                            px = AForge.PixelFormat.Format48bppRgb;
                        double pxx = pd.asPixels().getPhysicalSizeX().getValue();
                        double pyy = pd.asPixels().getPhysicalSizeX().getValue();
                        double pzz = pd.asPixels().getPhysicalSizeX().getValue();
                        if (stage.isLoaded())
                        {
                            Length? sxx = stage.getPositionX();
                            Length? syy = stage.getPositionY();
                            Length? szz = stage.getPositionZ();
                            b.Resolutions.Add(new Resolution(xs, ys, px, pxx, pyy, pzz, sxx.getValue(), syy.getValue(), szz.getValue()));
                        }
                        else
                            b.Resolutions.Add(new Resolution(xs, ys, px, pxx, pyy, pzz, 0, 0, 0));
                    }

                    for (int z = 0; z < zs; z++)
                    {
                        for (int c = 0; c < cs; c++)
                        {
                            for (int t = 0; t < ts; t++)
                            {
                                Pixels ps = pd.asPixels();
                                int chc = ps.sizeOfChannels();
                                store.setPixelsId(ps.getId().getValue(), true);
                                byte[] bts = store.getPlane(z, c, t);
                                PixelsType pxt = ps.getPixelsType();
                                AForge.PixelFormat px = AForge.PixelFormat.Format8bppIndexed;
                                int bits = pxt.getBitSize().getValue();
                                if (bits == 8)
                                    px = AForge.PixelFormat.Format8bppIndexed;
                                else if (bits < 16 || bits == 16)
                                    px = AForge.PixelFormat.Format16bppGrayScale;
                                else if (bits > 16 && bits <= 32)
                                    px = AForge.PixelFormat.Format32bppArgb;
                                else if (bits > 32)
                                    px = AForge.PixelFormat.Format48bppRgb;
                                if (chc < 0 && !chans)
                                {
                                    if (px == AForge.PixelFormat.Format16bppGrayScale || px == AForge.PixelFormat.Format8bppIndexed)
                                        b.Channels.Add(new AForge.Channel(0, bits, 1));
                                    if (px == AForge.PixelFormat.Format24bppRgb || px == AForge.PixelFormat.Format48bppRgb)
                                        b.Channels.Add(new AForge.Channel(0, bits, 3));
                                    chans = true;
                                }
                                b.Buffers.Add(new AForge.Bitmap(xs, ys, px, bts, new AForge.ZCT(z, c, t), ""));

                                if (chc > 0 && !chans)
                                {
                                    var chh = ps.getChannel(0);
                                    var chs = ps.copyChannels();
                                    for (int ch = 0; ch < chs.size(); ch++)
                                    {
                                        omero.model.Channel pxs = (omero.model.Channel)chs.get(ch);
                                        b.Channels.Add(new AForge.Channel(ch, bits, bits / 8));
                                    }
                                    chans = true;
                                }
                            }
                        }
                    }

                    b.UpdateCoords(zs, cs, ts);
                    b.series = o.getSeries();
                    bms.Add(b);
                }
                while (itr.hasNext());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
