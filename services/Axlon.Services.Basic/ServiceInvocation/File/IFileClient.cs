using Axlon.Framework.Abstractions;
using Axlon.Framework.Core.DependencyInjection;
using Axlon.Services.Basic.OutInput.Output.File;
using Axlon.Services.Contracts.Promotion.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Axlon.Services.Basic.ServiceInvocation.File
{
    public interface IFileClient: IScopedDependency
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="visibility"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        //Task<FileMetadataOutput> InternalUploadAsync(string fileName, string? visibility, byte[] bytes);

        /// <summary>
        /// 创建微信小程序二维码
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<MessageModel<string>> CreatePromotionCodeAsync(CreatePromotionCodeReq req);
    }
}
