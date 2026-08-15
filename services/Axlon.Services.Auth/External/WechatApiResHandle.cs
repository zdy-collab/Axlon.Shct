using Axlon.Framework.Abstractions;
using Axlon.Services.Contracts.Wechat.Base;

namespace Axlon.Services.Auth.External
{
    public class WechatApiResHandle
    {
        public static MessageModel<T>? ReturnMsg<T>(WechatBaseRes res)
        {
            string msg = "";
            switch (res.errcode)
            {
                case -1:
                    msg = "系统繁忙，请稍候再试";
                    break;
                case 40029:
                    msg = "用户code无效";
                    break;
                case 40226:
                    msg = "用户账号状态异常";
                    break;
                case 45011:
                    msg = "API调用频繁";
                    break;
                default:
                    break;
            }
            var model = MessageModel<T>.Fail(msg);
            model.status = 500;

            if (!string.IsNullOrEmpty(msg)) return model;
            return null;
        }
    }
}
