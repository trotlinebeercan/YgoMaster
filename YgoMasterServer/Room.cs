using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YgoMaster
{
    class Room
    {
        public class Table
        {
            public Player Player1 = null;
            public Player Player2 = null;
            public int stats = 1; // no clue, seems to always be 1?

            public Dictionary<string, object> ToDictionary()
            {
                return new Dictionary<string, object>()
                {
                    { "player1", Player1 == null ? "0" : Player1.CodeAsFormattedString() },
                    { "player2", Player2 == null ? "0" : Player2.CodeAsFormattedString() },
                    { "stats", "1" },
                };
            }
        };

        public class MatchRecord
        {
            public int Draw = 0;
            public int Win  = 0;
            public int Lose = 0;

            public Dictionary<string, object> ToDictionary()
            {
                return new Dictionary<string, object>()
                {
                    { "win", Win },
                    { "lose", Lose },
                    { "draw", Draw },
                };
            }
        };

        public class ConnectedPlayer : Tuple<Player, MatchRecord>
        {
            public ConnectedPlayer(Player p, MatchRecord r) : base(p, r) { }
            public void Won() { this.Item2.Win++; }
            public void Lost() { this.Item2.Lose++; }
            public void Tied() { this.Item2.Draw++; }

            public Player Player => this.Item1;
            public MatchRecord Record => this.Item2;

            public Dictionary<string, object> ToDictionary()
            {
                Player p = Item1;
                MatchRecord r = Item2;
                return new Dictionary<string, object>()
                {
                    { "record", r.ToDictionary() },
                    { "comment", 0 }, // idk if this ever changes
                    { "name", p.Name },
                    { "icon_id", p.IconId },
                    { "icon_frame_id", p.IconFrameId },
                    { "platform_name", "deeznuts" },
                    { "platform", 0 },
                };
            }
        }

        public List<ConnectedPlayer> ConnectedPlayers { get; private set; }
        public List<Table> RoomTables { get; private set; }

        public int Id;             // UUID of room created in server
        public int Comment;        // index in list of preset comments
        public bool IsJoinPlayer;  // no clue
        public bool IsPublic;      // whether or not others can see this room (friend invite only)
        public bool IsSpectral;    // spectate list perms
        public bool IsSpecter;     // spectate game perms
        public bool IsReplay;      // view room replays perms
        public int SpecterId;      // the players spectator ID, it is unique ??
        public int SpecterNum;     // number of spectators?
        public int XPlatformLimit; // no clue
        public int BattleType;     // basic ruleset
        public int LifePoints;     // index: 2=4000, 1=8000
        public int Time;           // index: 1=Normal, 2=Long, 3=Short
        public string LimitTime;   // room expiration date/time, format="3 22 2023\u00a022:01:56", \u00a0 is "no break space"

        // the creator of the room
        public Player Master
        {
            get => ConnectedPlayers.FirstOrDefault()?.Item1;
            set => AddNewPlayerToRoom(value);
        }

        // the maximum number of players allowed to join, equal to the number of tables * 2
        public int MemberMax
        {
            get => RoomTables.Count * 2;
            set => InitializeTables(value);
        }

        public int MemberNum => ConnectedPlayers.Count;

        public Room()
        {
            ConnectedPlayers = new List<ConnectedPlayer>();
            RoomTables = new List<Table>();

            // seems to always be the time of creation of the room
            LimitTime = DateTime.Now.ToString("M d yyyy\u00a0HH:mm:ss");

            // these are the variables i'm not entirely sure about
            IsJoinPlayer = true;
            SpecterId = 0;
            SpecterNum = 0;
            XPlatformLimit = 0;
        }

        public void AddNewPlayerToRoom(Player player)
        {
            foreach (var cp in ConnectedPlayers)
            {
                if (cp.Player == player)
                {
                    return;
                }
            }

            Console.WriteLine($"Adding player {player.Name} to existing room {Id}");
            ConnectedPlayers.Add(new ConnectedPlayer(player, new MatchRecord()));
        }

        public void RemovePlayerFromRoom(Player player)
        {
            foreach (var cp in ConnectedPlayers)
            {
                if (cp.Player == player)
                {
                    ConnectedPlayers.Remove(cp);
                    break;
                }
            }
        }

        public void MovePlayerToTable(Player player, int tableId)
        {
            if (RoomTables[tableId].Player1 == null)
                RoomTables[tableId].Player1 = player;
            else if (RoomTables[tableId].Player2 == null)
                RoomTables[tableId].Player2 = player;
            else
                throw new Exception("We tried to move a player to a missing or full table");
        }

        public void RemovePlayerFromTable(Player player)
        {
            foreach (var table in RoomTables)
            {
                if (table.Player1 == player)
                    table.Player1 = null;
                else if (table.Player2 == player)
                    table.Player2 = null;
            }
        }

        public void SetPlayerWon(Player player)
        {
            ConnectedPlayers.First(cp => cp.Player.Code == player.Code).Won();
        }
        public void SetPlayerLost(Player player)
        {
            ConnectedPlayers.First(cp => cp.Player.Code == player.Code).Lost();
        }
        public void SetPlayerTied(Player player)
        {
            ConnectedPlayers.First(cp => cp.Player.Code == player.Code).Tied();
        }

        public Dictionary<string, object> ToResponse()
        {
            Room room = this;

            var table_info = new HashSet<Dictionary<string, object>>();
            foreach (var table in RoomTables)
            {
                table_info.Add(table.ToDictionary());
            }

            var room_member = new Dictionary<string, object>();
            foreach (var member in ConnectedPlayers)
            {
                room_member.Add(member.Player.CodeAsFormattedString(), member.ToDictionary());
            }

            return new Dictionary<string, object>()
            {
                { "room_id", room.Id },
                { "room_master", room.Master?.CodeAsFormattedString() },
                { "room_comment", room.Comment },
                { "is_join_player", room.IsJoinPlayer },
                { "room_specter_id", room.SpecterId },
                { "specter_num", room.SpecterNum },
                { "is_public", room.IsPublic },
                { "member_num", room.MemberNum },
                { "member_max", room.MemberMax },
                { "is_spectral", room.IsSpectral },
                { "is_specter", room.IsSpecter },
                { "xPlatformLimit", room.XPlatformLimit },
                { "is_replay", room.IsReplay },
                { "battle_setting", new Dictionary<string, object>()
                    {
                        { "rule", room.BattleType },
                        { "lp", room.LifePoints },
                        { "time", room.Time },
                    }
                },
                { "limit_time", room.LimitTime },
                { "table_info", table_info.ToArray() }, // <-- why this is an array
                { "room_member", room_member },         // <-- but this isn't, i will never know
            };
        }

        private void InitializeTables(int maxPlayerNum)
        {
            for (int i = 0; i < maxPlayerNum / 2; i++)
            {
                RoomTables.Add(new Table());
            }
        }
    }
}
