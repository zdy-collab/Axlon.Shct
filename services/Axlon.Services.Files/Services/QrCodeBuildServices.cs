using Axlon.Framework.Core.HttpContextUser;
using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.Models.Files;
using Axlon.Services.Contracts.Promotion;
using Axlon.Services.Contracts.Promotion.Dto;
using Axlon.Services.Contracts.Promotion.Enums;
using Axlon.Services.Contracts.Wechat.Dto;
using Axlon.Services.Files.External;
using Axlon.Services.Files.Services.Interfaces;
using System;
using System.Configuration.Provider;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Axlon.Services.Files.Services
{
    public class QrCodeBuildServices : IQrCodeBuildServices
    {
        private readonly IWechatApi wechatApi;
        private readonly IFileTransferApplication files;

        public QrCodeBuildServices(IWechatApi wechatApi, IFileTransferApplication files)
        {
            this.wechatApi = wechatApi;
            this.files = files;
        }

        public async Task<long> CreatePromotionCodeAsync(CreatePromotionCodeReq req, CancellationToken cancellationToken)
        {

            //var path = string.Empty;
            var qrCodeRes = await wechatApi.GetUnlimitedQRCodeAsync(new GetUnlimitedQRCodeReq { scene = req.Scene, page = req.PagePath });
            
            if (qrCodeRes.buffer == null) return 0;

            await using var stream = new MemoryStream(qrCodeRes.buffer);

            var output = await files.UploadAsync(
                "local",
                $"{req.Scene}."+qrCodeRes.fileType,          // 文件名
                qrCodeRes.buffer.Length,        // 文件大小
                FileVisibilities.Public, // 可见性，根据你的枚举调整
                stream,
                cancellationToken);

            return output.Id;
            //if(qrCodeRes.)
            //if (qrCodeInfo == null)
            //{
            //    var scene = SceneHelper.Generate();



            //    if (qrCodeRes.buffer == null) throw new Exception("获取推广码失败！");

            //    var fileName = $"{scene}.png";

            //    path = $"{catalog}/{fileName}"; // 相对路径

            //    //var rootDirectory = Path.Combine(_environment.WebRootPath, catalog);    // 目录

            //    // 物理保存路径
            //    var saveFilePath = Path.Combine(_environment.WebRootPath, path);

            //    var directory = Path.GetDirectoryName(saveFilePath)!;
            //    Directory.CreateDirectory(directory);

            //    await File.WriteAllBytesAsync(saveFilePath, qrCodeRes.buffer);

            //    var qrScene = QrScene.Create(new CreateQrSceneCommand(user.ID, scene, QrSceneType.推广码, "", path));

            //    await base.Add(qrScene);

            //    var res = await fileClient.InternalUploadAsync(fileName, FileVisibilities.Tenant, qrCodeRes.buffer);
            //}
            //else
            //{
            //    path = qrCodeInfo.FilePath;
            //}

            //return path.CombinFileAccessPath();
            throw new System.NotImplementedException();
        }
    }
}
