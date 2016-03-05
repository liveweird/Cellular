using System;
using Akka.Actor;
using Microsoft.AspNet.SignalR;

namespace Cellular.Actors
{
    public class MakeChangeInCellMessage
    {
        public int DimX { get; set; }
        public int DimY { get; set; }
    }

    public class EcosystemActor : ReceiveActor
    {
        private readonly IActorRef[,] _cells = new IActorRef[5,5];

        public EcosystemActor()
        {
            for (var x = 0; x < 5; x++)
            {
                for (var y = 0; y < 5; y++)
                {
                    _cells[x, y] = Context.ActorOf(Props.Create(typeof (CellActor)), $"{x}:{y}");
                }
            }

            Receive<MakeChangeInCellMessage>(message =>
            {
                _cells[message.DimX, message.DimY].Tell(message);
            });
        }
    }

    public class CellActor : ReceiveActor
    {
        public CellActor()
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<CellularHub>();
            var random = new Random((int)DateTime.Now.Ticks);

            Receive<MakeChangeInCellMessage>(message =>
            {
                hub.Clients.All.addCellChange(message.DimX, message.DimY, random.Next(256));
            });
        }
    }
}