using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerManager.DAL
{
    public interface IUpdatedObject
    {
        DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
