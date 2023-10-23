using Comp.DataIngestionProject.DataSimulator.Bll;

namespace Comp.DataIngestionProject.DataSimulator
{
    public partial class MainForm : Form
    {
        private readonly LoadService _loadService;
        public MainForm()
        {
            InitializeComponent();
            _loadService = new LoadService();
        }

        private async void SendDataButton_Click(object sender, EventArgs e)
        {
            var loadData = _loadService.GetLoadData();
            //await _loadService.SendLoadDataToIoTHub(loadData);
            await _loadService.SendLoadDataToEventHub(loadData);
        }
    }
}