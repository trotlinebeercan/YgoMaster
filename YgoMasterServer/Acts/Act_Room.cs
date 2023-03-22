using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YgoMaster
{
    partial class GameServer
    {
        void Act_RoomCreate(GameServerWebRequest request)
        {
            if (ActiveDuelRoom != null)
            {
                WriteRoom(request, ActiveDuelRoom);
                return;
            }

            Dictionary<string, object> args;
            if (Utils.TryGetValue(request.ActParams, "room_settings", out args))
            {
                Room room = new Room();
                foreach (KeyValuePair<string, object> arg in args)
                {
                    switch (arg.Key)
                    {
                        case "room_comment":
                            room.Comment = (int)Convert.ChangeType(arg.Value, typeof(int));
                            break;
                        case "member_max":
                            room.MemberMax = (int)Convert.ChangeType(arg.Value, typeof(int));
                            break;
                        case "battle_rule":
                            room.BattleType = (int)Convert.ChangeType(arg.Value, typeof(int));
                            break;
                        case "battle_lp":
                            room.LifePoints = (int)Convert.ChangeType(arg.Value, typeof(int));
                            break;
                        case "is_public":
                            room.IsPublic = (bool)Convert.ChangeType(arg.Value, typeof(bool));
                            break;
                        case "is_spectral":
                            room.IsSpectral = (bool)Convert.ChangeType(arg.Value, typeof(bool));
                            break;
                        case "is_specter":
                            room.IsSpecter = (bool)Convert.ChangeType(arg.Value, typeof(bool));
                            break;
                        case "is_replay":
                            room.IsReplay = (bool)Convert.ChangeType(arg.Value, typeof(bool));
                            break;
                        case "battle_time":
                            room.Time = (int)Convert.ChangeType(arg.Value, typeof(int));
                            break;
                        default:
                            Utils.LogWarning("Unhandled Room arg '" + arg.Key + "' = " + MiniJSON.Json.Serialize(arg.Value));
                            break;
                    }
                }

                ActiveDuelRoom = new Room();
                //do
                {
                    int newRoomId = rand.Next(100000, 999999);
                    Console.WriteLine($"Creating new room {room.Id}");
                    //if (!ActiveDuelRooms.ContainsKey(newRoomId))
                    {
                        // assign the new UUID to the room created by the current player
                        room.Id = newRoomId;
                        room.Master = request.Player;
                        //break;
                    }
                } //while (true);

                // store room on server
                ActiveDuelRoom = room;
                WriteRoom(request, room);
            }
        }

        void Act_RoomTablePolling(GameServerWebRequest request)
        {
            // gbx:dhopper TODO: this is a hack, fix it
            if (ActiveDuelRoom != null)
            {
                ActiveDuelRoom.AddNewPlayerToRoom(request.Player);
            }

            var res = Internal_Act_DuelMenuInfo_Room();
            res.Add("new_comment", new object[0]);
            request.Response["Room"] = res;
            request.Remove("Room.room_info.new_comment");//, "Room.room_info.room_member");
        }

        void Act_RoomTableArrive(GameServerWebRequest request)
        {
            int tableId;
            if (Utils.TryGetValue(request.ActParams, "table_no", out tableId))
            {
                //if (ActiveDuelRooms.Count > 0)
                {
                    //Room room = ActiveDuelRooms.First().Value;
                    ActiveDuelRoom?.MovePlayerToTable(request.Player, tableId);
                    WriteRoom(request, ActiveDuelRoom);
                }
            }
        }

        void Act_RoomTableLeave(GameServerWebRequest request)
        {
            //if (ActiveDuelRooms.Count > 0)
            {
                //Room room = ActiveDuelRooms.First().Value;
                ActiveDuelRoom?.RemovePlayerFromTable(request.Player);
                WriteRoom(request, ActiveDuelRoom);
            }
        }

        void Act_RoomExit(GameServerWebRequest request)
        {
            // should only ever be 1, but you never know...
            //var rooms = ActiveDuelRooms.Where(x => x.Value.Master.Code == request.Player.Code);
            //foreach (var room in rooms.ToList())
            //{
            //    ActiveDuelRooms.Remove(room.Value.Id);
            //}

            ActiveDuelRoom?.RemovePlayerFromRoom(request.Player);
            if (ActiveDuelRoom?.MemberNum == 0)
            {
                Console.WriteLine($"==> Deleting room with id {ActiveDuelRoom.Id}");
                ActiveDuelRoom = null;
            }

            request.Remove("Room.room_info");
        }
    }
}
