using System;
using Akka.Actor;
using Microsoft.AspNet.SignalR;

namespace Cellular.Actors
{
    public class RefreshWorldMessage { }

    public class WakeUpCellMessage
    {
        public int DimX { get; set; }
        public int DimY { get; set; }
    }

    public class NeighborListMessage
    {
        public IActorRef[] Neighbors { get; set; }
    }

    public class CellHasDied { }
    public class CellIsAlive { }

    public class EcosystemActor : ReceiveActor
    {
        private const int Size = 5;
        private readonly IActorRef[,] _cells = new IActorRef[Size, Size];
        private IHubContext _hub;

        private Tuple<int, int> FindLocation(IActorRef sender)
        {
            for (var x = 0; x < Size; x++)
            {
                for (var y = 0; y < Size; y++)
                {
                    if (ReferenceEquals(_cells[x,
                                               y],
                                        sender))
                    {
                        return new Tuple<int, int>(x,
                                                   y);
                    }
                }
            }

            return new Tuple<int, int>(-1, -1);
        }

        public EcosystemActor()
        {
            _hub = GlobalHost.ConnectionManager.GetHubContext<CellularHub>();

            for (var x = 0; x < Size; x++)
            {
                for (var y = 0; y < Size; y++)
                {
                    _cells[x, y] = Context.ActorOf(Props.Create(typeof (CellActor)), $"{x}:{y}");
                }
            }

            for (var x = 0; x < Size; x++)
            {
                for (var y = 0; y < Size; y++)
                {
                    var _x = (x >= 1) ? x - 1 : Size - 1;
                    var x_ = (x + 1 < Size) ? x + 1 : 0;
                    var _y = (y >= 1) ? y - 1 : Size - 1;
                    var y_ = (y + 1 < Size) ? y + 1 : 0;
                    _cells[x, y].Tell(new NeighborListMessage
                                      {
                                          Neighbors = new[]
                                                      {
                                                          _cells[_x, _y], _cells[_x, y], _cells[_x, y_],
                                                          _cells[x, _y], _cells[x, y_],
                                                          _cells[x_, _y], _cells[x_, y], _cells[x_, y_],
                                                      }
                                      });
                }
            }

            Receive<WakeUpCellMessage>(message =>
            {
                _cells[message.DimX, message.DimY].Tell(message);
            });

            Receive<RefreshWorldMessage>(message =>
                                         {
                                             for (var x = 0; x < Size; x++)
                                             {
                                                 for (var y = 0; y < Size; y++)
                                                 {
                                                     _cells[x, y].Tell(message);
                                                 }
                                             }
                                         });

            Receive<CellIsAlive>(message =>
                                 {
                                     var location = FindLocation(Sender);
                                     _hub.Clients.All.addCellChange(location.Item1,
                                                                    location.Item2,
                                                                    128);
                                 });

            Receive<CellHasDied>(message =>
                                 {
                                     var location = FindLocation(Sender);
                                     _hub.Clients.All.addCellChange(location.Item1,
                                                                    location.Item2,
                                                                    0);
                                 });
        }
    }

    public class CellActor : ReceiveActor
    {
        private IActorRef _ecosystem;
        private IActorRef[] _neighbors;
        private bool _alive = false;
        private short _neighborsAlive = 0;

        private void Die()
        {
            foreach (var neighbor in _neighbors)
            {
                neighbor.Tell(new CellHasDied());
            }

            _alive = false;
            _ecosystem.Tell(new CellHasDied());
        }

        private void Live()
        {
            foreach (var neighbor in _neighbors)
            {
                neighbor.Tell(new CellIsAlive());
            }

            _alive = true;
            _ecosystem.Tell(new CellIsAlive());
        }

        public CellActor()
        {
            Receive<NeighborListMessage>(message =>
                                         {
                                             _ecosystem = Sender;
                                             _neighbors = (IActorRef[]) message.Neighbors.Clone();
                                         });

            Receive<WakeUpCellMessage>(message =>
                                       {
                                           if (!_alive)
                                           {
                                               Live();
                                           }
                                       });

            Receive<CellIsAlive>(message => { _neighborsAlive++; });
            Receive<CellHasDied>(message => { _neighborsAlive--; });

            Receive<RefreshWorldMessage>(message =>
                                         {
                                             if (_alive)
                                             {
                                                 switch (_neighborsAlive)
                                                 {
                                                     case 2:
                                                     case 3:
                                                         break;
                                                     default:
                                                         Die();
                                                         break;
                                                 }
                                             }
                                             else
                                             {
                                                 if (_neighborsAlive == 3)
                                                 {
                                                     Live();
                                                 }
                                             }
                                         });
        }
    }
}