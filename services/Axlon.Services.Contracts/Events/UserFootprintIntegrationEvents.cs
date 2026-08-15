using Axlon.Framework.Abstractions.Messaging;

namespace Axlon.Services.Contracts.Events;

public static class UserFootprintTopics
{
    public const string ViewedV1 = "axlon.user-footprint.viewed.v1";
}

public static class UserFootprintTargetTypes
{
    public const string Merchant = "merchant";
    public const string Product = "product";
    public const string GroupBuy = "group_buy";
    public const string Content = "content";
    public const string Page = "page";

    public static bool IsBusinessTarget(string value) => value is Merchant or Product or GroupBuy or Content;

    public static bool IsValid(string value) => IsBusinessTarget(value) || value == Page;
}

public sealed record UserFootprintViewedIntegrationEvent(
    long UserId,
    string TargetType,
    long? TargetId = null,
    long? MerchantId = null,
    string? PageCode = null,
    string? TargetTitle = null,
    string? TargetImage = null) : IntegrationEvent;
