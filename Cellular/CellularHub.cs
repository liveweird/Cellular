using System;
using Akka.Actor;
using Cellular.Actors;
using Microsoft.AspNet.SignalR;

namespace Cellular
{
    public class CellularHub : Hub
    {
        public void RandomWakeUp(int cnt)
        {
            var random = new Random((int) DateTime.Now.Ticks);
            for (var i = 0; i < cnt; i++)
            {
                MvcApplication.Ecosystem.Tell(new WakeUpCellMessage {DimX = random.Next(10), DimY = random.Next(10)});
            }
        }

        public void WakeMeUp(int x, int y)
        {
            MvcApplication.Ecosystem.Tell(new WakeUpCellMessage { DimX = x, DimY = y });
        }
    }
}