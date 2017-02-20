using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigClownAppTV.Base.Commands
{
    public class ConnectCommand : BasicCommand
    {
        private Action<ConnectStatus> _action;
        public ConnectCommand(Action<ConnectStatus> status)
        {
            this._action = status;
        }

        public override void Execute(object parameter)
        {
            string value = parameter.ToString();
            if (value == "Connect")
            {
                this._action?.Invoke(ConnectStatus.Connect);
            }
            else if (value == "Disconnect")
            {
                this._action?.Invoke(ConnectStatus.Disconnect);
            }
            
        }

        public enum ConnectStatus
        {
            Connect,
            Disconnect
        }
    }
}
