using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Extensions
{
    public static string ToADebugString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
    {
        return "{" + string.Join(",", dictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
    }
}

namespace YgoMaster
{
    partial class GameServer
    {
        void Act_FriendGetList(GameServerWebRequest request)
        {
            var follow = new Dictionary<string, object>();
            var follower = new Dictionary<string, object>();

            int currentFriendIndex = 0;
            foreach (var player in playersByToken.Values)
            {
                var entry_follow = new Dictionary<string, object>()
                {
                    { "name", player.Name },
                    { "pcode", player.Code },
                    { "icon_id", player.IconId },
                    { "icon_frame_id", player.IconFrameId },
                    { "wallpaper", player.Wallpaper },
                    { "mutual", true },
                    { "pin", false },
                    { "watch", false },
                    { "online_time", 1677909365 },
                    { "login_time", 1677817039 }
                };
                follow.Add(player.CodeAsFormattedString(), entry_follow);

                var entry_follower = new Dictionary<string, object>()
                {
                    { "name", player.Name },
                    { "pcode", player.Code },
                    { "icon_id", player.IconId },
                    { "icon_frame_id", player.IconFrameId },
                    { "wallpaper", player.Wallpaper },
                    { "mutual", true },
                    { "followed_date", 0 },
                    { "watch", false }
                };
                follower.Add(currentFriendIndex.ToString(), entry_follower);

                currentFriendIndex++;
            }
            request.Response["Friend"] = new Dictionary<string, object>()
            {
                { "follow", follow },
                { "follower", follower },
                { "block", new object[0] },
                { "tag", new int[] {
                    
                } },   // idk what this is
                { "refresh_sec", 15 }
            };
            request.Remove(
                "Friend.follow",
                "Friend.follower",
                "Friend.block",
                "Friend.tag",
                "Friend.refresh_sec",
                "Friend.refresh");
        }

        void Act_FriendRefreshInfo(GameServerWebRequest request)
        {
            // the live client takes in a 'pcode_list' of friends to check the status of
            // and populates the dictionary with only those that have updated
            request.Response["Friend"] = new Dictionary<string, object>()
            {
                { "refresh", new Dictionary<string, object>() },
            };
            request.Remove("Friend.refresh");
        }
    }
}

/*
{
    "code":0,
    "res":[
        [
            96,
            {
                "Friend":{
                    "refresh":{
                        
                    }
                }
            },
            0,
            0
        ]
    ],
    "remove":[
        "Friend.refresh"
    ]
}
*/

/* request:
{"acts":[{"act":"Friend.get_list","id":17,"params":{"all":true}}],"v":"1.4.2","ua":"Windows.Steam/10.0.19044/B450M DS3H WIFI (Gigabyte Technology Co., Ltd.)","h":558161692}
 */

/* response:
{
    "code":0,
    "res":[
        [
            17,
            {
                "Friend":{
                    "follow":{
                        "994224833":{
                            "name":"Mezzy",
                            "pcode":994224833,
                            "icon_id":1013001,
                            "icon_frame_id":1031004,
                            "wallpaper":1130017,
                            "mutual":true,
                            "pin":false,
                            "watch":false,
                            "online_time":1677909365,
                            "login_time":1677905479
                        },
                        "987570522":{
                            "name":"DigBickChado",
                            "pcode":987570522,
                            "icon_id":1010011,
                            "icon_frame_id":1030001,
                            "wallpaper":1130001,
                            "mutual":true,
                            "pin":false,
                            "watch":false,
                            "login_time":1644474907
                        },
                        "6754179":{
                            "name":"OmicronCeti",
                            "pcode":6754179,
                            "icon_id":1012039,
                            "icon_frame_id":1030007,
                            "wallpaper":1130001,
                            "mutual":true,
                            "pin":false,
                            "watch":false,
                            "login_time":1677817039
                        },
                        "382374556":{
                            "name":"Beastie-J",
                            "pcode":382374556,
                            "icon_id":1011003,
                            "icon_frame_id":1030009,
                            "wallpaper":1130017,
                            "mutual":true,
                            "pin":false,
                            "watch":false,
                            "login_time":1677866976
                        }
                    },
                    "follower":{
                        "0":{
                            "name":"OmicronCeti",
                            "pcode":6754179,
                            "icon_id":1012039,
                            "icon_frame_id":1030007,
                            "wallpaper":1130001,
                            "mutual":true,
                            "followed_date":1644826587,
                            "watch":false
                        },
                        "1":{
                            "name":"Beastie-J",
                            "pcode":382374556,
                            "icon_id":1011003,
                            "icon_frame_id":1030009,
                            "wallpaper":1130017,
                            "mutual":true,
                            "followed_date":1643515539,
                            "watch":false
                        },
                        "2":{
                            "name":"Mezzy",
                            "pcode":994224833,
                            "icon_id":1013001,
                            "icon_frame_id":1031004,
                            "wallpaper":1130017,
                            "mutual":true,
                            "online_time":1677909365,
                            "followed_date":1642831077,
                            "watch":false
                        },
                        "3":{
                            "name":"DigBickChado",
                            "pcode":987570522,
                            "icon_id":1010011,
                            "icon_frame_id":1030001,
                            "wallpaper":1130001,
                            "mutual":true,
                            "followed_date":1642660554,
                            "watch":false
                        },
                        "is_terminal":true
                    },
                    "block":[
                        
                    ],
                    "tag":[
                        1020001,
                        1020002,
                        1020003,
                        1020004,
                        1020005,
                        1020006,
                        1020007,
                        1020008,
                        1020009,
                        1020010,
                        1020011,
                        1020012,
                        1020013,
                        1020014,
                        1020015,
                        1020016,
                        1020017,
                        1020018,
                        1020019,
                        1020020,
                        1020022,
                        1020023,
                        1020024,
                        1020025,
                        1020026,
                        1020027,
                        1020028,
                        1020029,
                        1020030,
                        1020031,
                        1020032,
                        1020033,
                        1020034,
                        1020035,
                        1020036,
                        1020037,
                        1020038,
                        1020039,
                        1020040,
                        1020041,
                        1020042,
                        1020043,
                        1020044,
                        1020045,
                        1020046,
                        1020053
                    ],
                    "refresh_sec":15
                }
            },
            0,
            0
        ]
    ],
    "remove":[
        "Friend.follow",
        "Friend.follower",
        "Friend.block",
        "Friend.tag",
        "Friend.refresh_sec",
        "Friend.refresh"
    ]
}

*/