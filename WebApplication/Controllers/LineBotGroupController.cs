using isRock.LineBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication.Controllers
{
    public class LineBotGroupController : ApiController
    {
        [HttpPost]
        public IHttpActionResult POST()
        {
            //todo:請換成你自己的token
            string ChannelAccessToken = "!!!!!!請換成你自己的token!!!!";

            try
            {
                //取得 http Post RawData(should be JSON)
                string postData = Request.Content.ReadAsStringAsync().Result;
                //剖析JSON
                var ReceivedMessage = isRock.LineBot.Utility.Parsing(postData);
                var item = ReceivedMessage.events.FirstOrDefault();
                string Message = "";

                switch (item.type)
                {
                    case "join":
                        Message = $"有人把我加入{item.source.type}中了，大家好啊~";
                      
                        //回覆用戶
                        isRock.LineBot.Utility.ReplyMessage(ReceivedMessage.events[0].replyToken, Message, ChannelAccessToken);
                        break;
                    case "message":
                        if (item.message.text == "bye")
                        {
                            //回覆用戶
                            isRock.LineBot.Utility.ReplyMessage(item.replyToken, "bye-bye", ChannelAccessToken);
                            //離開
                            if (item.source.type.ToLower() == "room")
                                isRock.LineBot.Utility.LeaveRoom(item.source.roomId, ChannelAccessToken);
                            if (item.source.type.ToLower() == "group")
                                isRock.LineBot.Utility.LeaveGroup(item.source.roomId, ChannelAccessToken);

                            break;
                        }
                        Message = "你說了:" + ReceivedMessage.events[0].message.text;
                        //取得用戶名稱 
                        LineUserInfo UserInfo = null;
                        if (item.source.type.ToLower() == "room")
                            UserInfo = isRock.LineBot.Utility.GetRoomMemberProfile(
                                item.source.roomId, item.source.userId, ChannelAccessToken);
                        if (item.source.type.ToLower() == "group")
                            UserInfo = isRock.LineBot.Utility.GetGroupMemberProfile(
                                item.source.groupId, item.source.userId, ChannelAccessToken);
                        //顯示用戶名稱
                        if (item.source.type.ToLower() != "user")
                            Message += "\n你是:" + UserInfo.displayName;

                        //回覆用戶
                        isRock.LineBot.Utility.ReplyMessage(item.replyToken, Message, ChannelAccessToken);
                        break;
                    default:
                        break;
                }

                //回覆API OK
                return Ok();
            }

            catch (Exception ex)
            {
                //todo:請自行處理exception
                return Ok();
            }

        }
    }
}
