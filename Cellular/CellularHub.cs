using System;
using Akka.Actor;
using Cellular.Actors;
using Microsoft.AspNet.SignalR;

namespace Cellular
{
    public class CellularHub : Hub
    {
        public void MakeChange()
        {
            var random = new Random((int) DateTime.Now.Ticks);
            MvcApplication.Ecosystem.Tell(new WakeUpCellMessage {DimX = random.Next(5), DimY = random.Next(5)});
        }
    }
}