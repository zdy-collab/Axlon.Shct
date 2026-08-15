using Axlon.Framework.Abstractions;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Framework.Data.IRepository.Base;
using Axlon.Framework.Data.Services.Base;
using Axlon.Services.Basic.External;
using Axlon.Services.Basic.Helper;
using Axlon.Services.Basic.IServices;
using Axlon.Services.Basic.ServiceInvocation.File;
using Axlon.Services.Basic.Services.Interfaces;
using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.Models.Files;
using Axlon.Services.Contracts.Promotion;
using Axlon.Services.Contracts.Promotion.Dto;
using Axlon.Services.Contracts.Promotion.Enums;
using Axlon.Services.Contracts.Wechat;
using Mapster;
using Microsoft.Extensions.Options;
using static Grpc.Core.Metadata;

namespace Axlon.Services.Basic.Services
{
    public class QrSceneServices : BaseServices<QrScene>, IQrSceneServices
    {
        private readonly IWechatApi wechatApi;
        private readonly IWebHostEnvironment _environment;
        private readonly IUser user;
        private readonly IFileClient fileClient;
        private readonly WechatOptions wechatOptions;
        public QrSceneServices(IBaseRepository<QrScene> repository, IWechatApi wechatApi, 
            IWebHostEnvironment environment, IUser user, IFileClient fileClient
            ,IOptions<WechatOptions> wechatOptions) : base(repository)
        {
            this.wechatApi = wechatApi;
            _environment = environment;
            this.user = user;
            this.fileClient = fileClient;
            this.wechatOptions = wechatOptions.Value;
        }

        public async Task<(bool,string)> GetPromotionQrCodeAsync()
        {

            var qrCodeInfo = (await base.Query(whereExpression: x => x.UserId == user.ID
                && x.Type == QrSceneType.个人推广码
                && x.Status == DisableEnable.启用))
                .FirstOrDefault();

            string fileGetPath = string.Empty;

            if (qrCodeInfo == null) 
            {
                var scene = SceneHelper.Generate();

                // 调用文件服务生成二维码
                var createPCMsg = await fileClient.CreatePromotionCodeAsync(new CreatePromotionCodeReq(scene, wechatOptions.HomePage));
                if (createPCMsg.status != 200) return (false,createPCMsg.msg);

                var imageFileId = long.Parse(createPCMsg.response);

                var entity = QrScene.Create(new CreateQrSceneCommand(user.ID, imageFileId, scene, QrSceneType.个人推广码, wechatOptions.HomePage));
                await base.Add(entity);
                fileGetPath = entity.ImageFileId.ToString().CombinFileAccessPath();
                
            }
            else 
            {
                fileGetPath = qrCodeInfo.ImageFileId.ToString().CombinFileAccessPath();
            }

            return (true, fileGetPath);
        }

        public async Task<QrSceneBasicDto> GetQrSceneBySceneAsync(string scene)
        {
            var data = (await base.Query(whereExpression: x => x.Scene == scene)).FirstOrDefault();

            var res = data.Adapt<QrSceneBasicDto>();

            return res;
        }
    }
}
