using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TOR_Core.Extensions;

namespace TOR_Core.Models
{
    public class TORCompanionHiringPriceCalculationModel : DefaultCompanionHiringPriceCalculationModel
    {
        public override int GetCompanionHiringPrice(Hero companion)
        {
            int price = base.GetCompanionHiringPrice(companion);
            if (companion.Template.IsTORTemplate() && companion.IsWanderer)
            {
                if (companion.IsSpellCaster())
                {
                    return Math.Max(20000, price);
                }
                else if(companion.BattleEquipment.GetHumanBodyArmorSum() > 40) 
                {
                    return Math.Max(30000, price);
                }
            }
            return price;
        }
    }
}