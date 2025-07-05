using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerManager.BAL.DTO
{
    public class ObjectUpdatedEventArgs : EventArgs
    {
        public string UpdateType { get; set; }
    }
}
