using System.Globalization;
using System.Xml.Linq;

namespace Rando
{
    public partial class Rando : Form
    {
        List<Trackpoint> Trackpoints = new List<Trackpoint>();
        Color[] gradient = new Color[]
        {
            Color.FromArgb(255, 144, 238, 144),
            Color.FromArgb(255, 162, 216, 128),
            Color.FromArgb(255, 180, 194, 112),
            Color.FromArgb(255, 198, 172, 96),
            Color.FromArgb(255, 216, 150, 80),
            Color.FromArgb(255, 234, 128, 64),
            Color.FromArgb(255, 244, 106, 48),
            Color.FromArgb(255, 248,  84, 36),
            Color.FromArgb(255, 252,  62, 24),
            Color.FromArgb(255, 254,  48, 18),
            Color.FromArgb(255, 255,  32, 12),
            Color.FromArgb(255, 255,  16,  6),
            Color.FromArgb(255, 255,   0,  0)
        };

        public Rando()
        {
            InitializeComponent();
        }

        private void Rando_Load(object sender, EventArgs e)
        {
            ReadGPXFile("gemmikandersteg.gpx");
        }
        private Point ConvertGpsToPoint(double lat, double lon, int width, int height)
        {
            double minLat = Trackpoints.Min(p => p.Latitude);
            double maxLat = Trackpoints.Max(p => p.Latitude);
            double minLon = Trackpoints.Min(p => p.Longitude);
            double maxLon = Trackpoints.Max(p => p.Longitude);

            double xNorm = (lon - minLon) / (maxLon - minLon);
            double yNorm = (maxLat - lat) / (maxLat - minLat);

            int usableWidth = width - 2 * 20;
            int usableHeight = height - 2 * 20;

            int x = 20 + (int)(xNorm * usableWidth);
            int y = 20 + (int)(yNorm * usableHeight);

            return new Point(x, y);
        }

        private void Rando_Form_Paint(object sender, PaintEventArgs e)
        {
            if (Trackpoints.Count < 2) return;

            Pen myPen = new Pen(Color.Red, 2);

            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            Point[] points = Trackpoints.Select(tp => ConvertGpsToPoint(tp.Latitude, tp.Longitude, width, height)).ToArray();
            this.CreateGraphics().DrawLines(myPen, points);
        }

        private void ReadGPXFile(string filename)
        {
            const string BASE_PATH = @"C:\Users\pt70wvh\Documents\GitHub\323-Programmation_fonctionnelle\Emma\Rando\gpx\";

            XNamespace ns = "http://www.topografix.com/GPX/1/1";
            XDocument doc = XDocument.Load(BASE_PATH + filename);

            Trackpoints.AddRange(doc.Descendants(ns + "trkpt").Select(x => new Trackpoint { Latitude = double.Parse(x.Attribute("lat").Value, CultureInfo.InvariantCulture), Longitude = double.Parse(x.Attribute("lon").Value, CultureInfo.InvariantCulture), Elevation = double.Parse(x.Element(ns + "ele").Value, CultureInfo.InvariantCulture) }));
        }
    }
}