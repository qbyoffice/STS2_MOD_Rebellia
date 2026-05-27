using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Rebellia.RebelliaCode.Powers.cards;

namespace Rebellia.RebelliaCode.Api.Combat
{
    public partial class RNHealthBar : NHealthBar
    {
        private NinePatchRect? _tmepForeground;
        private NinePatchRect? _tmepHpBackground;

        public override void _Ready()
        {
            base._Ready();

            _tmepForeground = GetNode<NinePatchRect>("%TmepForeground");
            if (_tmepForeground == null)
                GD.PushError("RNHealthBar: %TmepForeground not found!");

            _tmepHpBackground = GetNode<NinePatchRect>("%TmepHpBackground");
            if (_tmepHpBackground == null)
                GD.PushError("RNHealthBar: %TmepHpBackground not found!");

            RebelliaTmepHpPower.OnAnyTempHpChanged += OnTempHpChanged;
        }

        private void OnTempHpChanged(Creature creature, int tempHp)
        {
            if (creature != _creature)
                return;
            bool visible = (_creature != null && _creature.CurrentHp > 0 && tempHp > 0);
            if (!visible)
            {
                if (_tmepForeground != null)
                    _tmepForeground.Visible = false;
                if (_tmepHpBackground != null)
                    _tmepHpBackground.Visible = false;
                return;
            }

            float maxWidth = MaxFgWidth;
            float ratio = Mathf.Min(tempHp / (float)_creature.MaxHp, 1f);
            float leftEdge = maxWidth * (1 - ratio);

            // 临时生命条：从右向左覆盖
            _tmepForeground!.OffsetRight = 0;
            _tmepForeground.OffsetLeft = leftEdge;
            _tmepForeground.Visible = true;

            // 边框（背景层）：完全相同的覆盖区域
            if (_tmepHpBackground != null)
            {
                _tmepHpBackground.OffsetRight = 0;
                _tmepHpBackground.OffsetLeft = leftEdge;
                _tmepHpBackground.Visible = true;
            }
        }

        public override void _ExitTree()
        {
            RebelliaTmepHpPower.OnAnyTempHpChanged -= OnTempHpChanged;
            base._ExitTree();
        }
    }
}
