using MDS.Communication.Messages.ControllerMessages;
using MDS.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSCore
{
    public class AppRegister : IDisposable
    {
        private MDSController _controller;
        private string _applicationName;

        public AppRegister(string applicationName,string serverIP, int serverPort)
        {
            _applicationName = applicationName;

            _controller = new MDSController(serverIP, serverPort);
            _controller.Connect();
        }

        public void RegisterApp()
        {
            _controller.SendMessage(
                new AddNewApplicationMessage
                {
                    ApplicationName = _applicationName
                });
        }

        public void UnregisterApp()
        {
            _controller.SendMessage(
                new RemoveApplicationMessage
                {
                    ApplicationName = _applicationName
                });
        }

        public void Dispose()
        {
            if (_controller != null)
            {
                _controller.Disconnect();
                _controller.Dispose();
            }
        }
    }
}
