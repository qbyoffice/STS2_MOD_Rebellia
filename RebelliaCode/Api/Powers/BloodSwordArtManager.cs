using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Rebellia.RebelliaCode.Powers;

namespace Rebellia.RebelliaCode.Api
{
    public static class BloodSwordArtManager
    {
        /// <summary>
        /// 获取玩家的 BloodSwordArtPower，如果不存在则创建（初始血点为0）。
        /// </summary>
        public static async Task<BloodSwordArtPower> GetOrCreatePower(Creature creature)
        {
            var power = creature.GetPower<BloodSwordArtPower>();
            if (power == null)
            {
                power = await PowerCmd.Apply<BloodSwordArtPower>(creature, 0, creature, null);
                power.MaxPoints = 2; // 默认最大值，可通过其他方式增加
            }
            return power;
        }

        /// <summary>
        /// 增加血点。
        /// </summary>
        public static async Task AddPoints(Creature creature, int amount)
        {
            var power = await GetOrCreatePower(creature);
            power.AddPoints(amount);
        }

        /// <summary>
        /// 尝试消费血点。
        /// </summary>
        public static async Task<bool> TrySpendPoints(Creature creature, int amount)
        {
            var power = await GetOrCreatePower(creature);
            return power.TrySpendPoints(amount);
        }

        /// <summary>
        /// 获取当前血点。
        /// </summary>
        public static async Task<int> GetPoints(Creature creature)
        {
            var power = await GetOrCreatePower(creature);
            return power.GetPoints();
        }
    }
}
