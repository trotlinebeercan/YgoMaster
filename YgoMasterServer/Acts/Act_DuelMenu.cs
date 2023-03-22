using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YgoMaster
{
    partial class GameServer
    {
        private static long GetTimestamp(long offsetInS = 0)
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds() + offsetInS;
        }

        void Act_DuelMenuInfo(GameServerWebRequest request)
        {
            request.Response["Master"]     = Internal_Act_DuelMenuInfo_Master();
            request.Response["DuelMenu"]   = Internal_Act_DuelMenuInfo_DuelMenu();
            request.Response["Room"]       = Internal_Act_DuelMenuInfo_Room();
            request.Response["Regulation"] = Internal_Act_DuelMenuInfo_Regulation();
            request.Remove(
                "DuelMenu",
                "Master.Tournament",
                "Master.Exhibition",
                "Master.Cup.rewards",
                "Room.holding_rule_list",
                "Regulation.rule_list",
                "Master.RankEvent",
                "Master.TeamMatch",
                "Master.DuelTrial"
            );
        }

        Dictionary<string, object> Internal_Act_DuelMenuInfo_Master()
        {
            var response = new Dictionary<string, object>()
            {
                { "Tournament", new object[0] },
                { "Exhibition", new object[0] },
                { "RegulationIcon", new Dictionary<string, object>() },
                { "Cup", new Dictionary<string, object>()
                    {
                        { "rewards", new object[0] }
                    }
                },
                { "RankEvent", new object[0] },
                { "TeamMatch", new object[0] },
                { "DuelTrial", new object[0] },
            };
            return response;
        }

        Dictionary<string, object> Internal_Act_DuelMenuInfo_DuelMenu()
        {
            var response = new Dictionary<string, object>()
            {
                { "Tournament", new object[0] },
                { "Exhibition", new object[0] },
                { "RankEvent",  new object[0] },
                { "Cup",        new object[0] },
                { "TeamMatch",  new object[0] },
                { "DuelTrial",  new object[0] },
                { "Standard", new Dictionary<string, object>()
                    {
                        { "season_id", 69 },
                        { "regulation_id", CardMenuRegulationType.Normal }, // IDS_CARDMENU_REGULATION_NORMAL
                        { "holding", new Dictionary<string, object>()
                            {
                                { "start", "st. datetime text" },
                                { "end", "end datetime text" },
                                { "end_ts", GetTimestamp(-3000) },
                                { "result_end_ts", GetTimestamp(-1000) },
                            }
                        }
                    }
                },
                { "Free", new Dictionary<string, object>()
                    {
                        { "holding", new Dictionary<string, object>()
                            {
                                { "start", "st. datetime text" },
                                { "end", "end datetime text" },
                                { "end_ts", GetTimestamp(-3000) },
                            }
                        }
                    }
                }
            };
            return response;
        }

        Dictionary<string, object> Internal_Act_DuelMenuInfo_Room()
        {
            var room_info = new Dictionary<string, object>()
            {
                { "holding_rule_list", Internal_Act_DuelMenuInfo_RuleList() },
            };

            //if (ActiveDuelRooms.Count > 0)
            //{
            //    // gbx:dhopper TODO: don't force this to be static to the first room in the list...
            //    room_info.Add("room_info", ActiveDuelRooms.First().Value.ToResponse());
            //}

            if (ActiveDuelRoom != null)
            {
                room_info.Add("room_info", ActiveDuelRoom.ToResponse());
            }

            return room_info;
        }

        Dictionary<string, object> Internal_Act_DuelMenuInfo_Regulation()
        {
            return new Dictionary<string, object>()
            {
                { "rule_list", Internal_Act_DuelMenuInfo_RuleList() }
            };
        }

        Dictionary<string, object> Internal_Act_DuelMenuInfo_RuleList()
        {
            var ruleList = new Dictionary<string, object>();
            foreach (CardMenuRegulationType t in Enum.GetValues(typeof(CardMenuRegulationType)))
            {
                if (t == CardMenuRegulationType.Unknown) continue;
                ruleList.Add(((int)t).ToString(), t.GetUnderlyingEnumString());
            }
            return ruleList;
        }
    }
}
