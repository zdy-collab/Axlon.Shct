using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Base
{
    public static class InternalApiBaseAdr
    {
        #region basic

        public const string InternalPromotionRelation = "api/basic/internal/InternalPromotionRelation";

        public const string InternalPromotionCommissionRule = "api/basic/internal/InternalPromotionCommissionRule";

        public const string InternalCategory = "api/basic/internal/InternalCategory";

        public const string InternalUser = "api/basic/internal/InternalUser";

        #endregion



        #region merchant

        public const string InternalMerchant = "api/merchant/internal/InternalMerchant";

        public const string InternalProduct = "api/merchant/internal/InternalProduct";

        #endregion



        #region files

        public const string InternalQrCodeBuild = "api/files/internal/InternalQrCodeBuild";

        #endregion



    }
}
