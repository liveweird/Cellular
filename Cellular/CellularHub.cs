using System;
using Akka.Actor;
using Cellular.Actors;
using Microsoft.AspNet.SignalR;

namespace Cellular
{
    public class CellularHub : Hub
    {
        public void RandomWakeUp()
        {
            var random = new Random((int) DateTime.Now.Ticks);
            MvcApplication.Ecosystem.Tell(new WakeUpCellMessage {DimX = random.Next(10), DimY = random.Next(10)});
        }

        public void WakeMeUp(int x, int y)
        {
            MvcApplication.Ecosystem.Tell(new WakeUpCellMessage { DimX = x, DimY = y });
        }
    }
}