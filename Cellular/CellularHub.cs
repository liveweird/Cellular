using System;
using Microsoft.AspNet.SignalR;

namespace Cellular
{
    public class CellularHub : Hub
    {
        public void MakeChange()
        {
            var random = new Random((int) DateTime.Now.Ticks);
            Clients.All.addCellChange(random.Next(5), random.Next(5), random.Next(256));
        }
    }
}