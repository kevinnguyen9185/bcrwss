namespace desktop_app_core
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.chromiumWebBrowser1.LoadUrl("https://vnexpress.net/");
        }
    }
}