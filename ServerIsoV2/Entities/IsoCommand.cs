using System.Net.Sockets;

namespace ServerIsoV2
{
    public partial class IsoCommand
    {
        public bool Ended { get; set; }

        public bool Running { get; set; }

        public string Id { get; set; }

        public string Text { get; set; }

        public Socket handler { get; set; }

        public bool ChannelOpen { get; set; }
    }
}
