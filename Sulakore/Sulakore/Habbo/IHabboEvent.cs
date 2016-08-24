using Sulakore.Protocol;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public interface IHabboEvent
    {
        Dictionary<string, object> Data { get; }
        HMessage Packet { get; }
        int Header { get; }
    }
}
