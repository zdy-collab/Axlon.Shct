using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Extensions;

namespace Axlon.Services.Contracts.Promotion.Dto.Mini
{
    public class GetMyPromotionInfoRes
    {
        /// <summary>
        /// 我的直推（人）
        /// </summary>
        public int DirectPromotion { get; set; }

        /// <summary>
        /// 我的团队（人）
        /// </summary>
        public int PromotionTeam { get; set; }

        /// <summary>
        /// 今日新增（人）
        /// </summary>
        public int TodayPromotion { get; set; }

        /// <summary>
        /// 累计新增
        /// </summary>
        public int TotalPromotion { get; set; }

        private PrompterLevel prompterLevel { get; set; }

        /// <summary>
        /// 推广等级
        /// </summary>
        public string PrompterLevel
        {
            get
            {
                return prompterLevel.GetDescription();
            }
            set
            {
                prompterLevel = (PrompterLevel)System.Enum.Parse(typeof(PrompterLevel), value);
            }
        }

        /// <summary>
        /// 直推人数进度
        /// </summary>
        public double DirectPromotionPlan { get; set; }

        ///
        private PrompterLevel nextLevel { get; set; }

        /// <summary>
        /// 下一等级
        /// </summary>
        public string NextLevel
        {
            get
            {
                return nextLevel.GetDescription();
            }
            set
            {
                nextLevel = (PrompterLevel)System.Enum.Parse(typeof(PrompterLevel), value);
            }
        }
    }
}
