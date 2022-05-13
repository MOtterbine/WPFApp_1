using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp.Communication;

namespace WpfApp
{
    public class ViewModel : ViewModelBase
    {

        ICommunicationDevice comDev = null;



        public ViewModel()
        {
            // TCP (WiFi) connection
            this.comDev = new TCPSocket() { IPAddress = "192.168.0.10", IPPort = 35000, ConnectMethod = ConnectMethods.Client };

            // Bluetooth
            //this.comDev = new RS232("COM11");

            this.comDev.CommunicationEvent += OnCommEvent;
        }

        private async System.Threading.Tasks.Task OnCommEvent(object sender, ChannelEventArgs e)
        {
            await Task.Delay(0);

            switch (e.Event)
            {
                case CommunicationEvents.ReceiveEnd:
                    Title += Encoding.ASCII.GetString(e.data).Replace("\r", string.Empty);
                    break;
                case CommunicationEvents.ConnectedAsClient:
                    Title = "Connected";
                    break;
                case CommunicationEvents.Disconnected:
                    Title = "Disconnected";
                    break;
                case CommunicationEvents.Error:
                    if (!string.IsNullOrEmpty(e.Description)) SetTitle(e.Description);
                    break;
            }

        }

        public string Title 
        {
            get => title;
            private set
            {
                base.SetProperty(ref title, value);
            } 
        }
        private string title = "Our Class Know Nothing!";


        public void SetTitle(string title)
        {
            Title = title;
        }

        public void SetName(string title)
        {
            Title = title;
        }



        #region RunCommand

        public ICommand OpenCommand => new RelayCommand(param =>
        {
            Task.Run(() =>
            {
                this.Title = "Opening...";
                if (!this.comDev.Open())
                {
                    Title = "Get out of my face.";
                }
            });
        })
        {

        };

        public ICommand CloseCommand => new RelayCommand(param => this.comDev.Close());
        public ICommand SendCommand => new RelayCommand(param => {
            this.Title = string.Empty;
            this.comDev.Send("ATZ\r"); 
        });

        #endregion RunCommand




    }
}
