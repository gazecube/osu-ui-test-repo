// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace osu.Game.Screens.Menu
{
    /// <summary>
    /// Stable-era main-menu button presentation using the geometry and transition
    /// timings from the 2011 client. The temporary vector presentation can be
    /// replaced by the original menu-button-* textures without changing layout or
    /// interaction behaviour.
    /// </summary>
    public partial class LegacyMainMenuButton : CompositeDrawable
    {
        // 2011 menu-button PNGs are 583x100 and are authored for the 1024x768
        // sprite field. Stable displays them at 5/8 scale in its 640x480 field.
        private const float button_width = 583f * 5f / 8f;
        private const float button_height = 100f * 5f / 8f;

        private readonly Action action;
        private readonly Key[] triggerKeys;
        private readonly Vector2 basePosition;
        private readonly Box hover;

        private ButtonSystemState visibleState;
        private ButtonSystemState buttonSystemState = ButtonSystemState.Initial;

        public ButtonSystemState VisibleState
        {
            get => visibleState;
            set
            {
                visibleState = value;
                updateVisibility(true);
            }
        }

        public ButtonSystemState ButtonSystemState
        {
            get => buttonSystemState;
            set
            {
                if (buttonSystemState == value)
                    return;

                buttonSystemState = value;
                updateVisibility(false);
            }
        }

        public LegacyMainMenuButton(string text, Color4 colour, Vector2 basePosition, Action action, params Key[] triggerKeys)
        {
            this.action = action;
            this.triggerKeys = triggerKeys;
            this.basePosition = basePosition;

            Anchor = Anchor.Centre;
            Origin = Anchor.TopLeft;
            Position = basePosition;
            Size = new Vector2(button_width, button_height);
            Alpha = 0;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = new Color4(20, 20, 26, 225),
                },
                new Box
                {
                    RelativeSizeAxes = Axes.Y,
                    Width = 6,
                    Colour = colour,
                },
                hover = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.White,
                    Alpha = 0,
                },
                new OsuSpriteText
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Position = new Vector2(22, 0),
                    Text = text,
                    Shadow = true,
                },
            };
        }

        protected override bool OnHover(HoverEvent e)
        {
            if (buttonSystemState != visibleState)
                return false;

            // Stable overshoots to +30 over 140 ms, then settles to +20 over the
            // following 140 ms. The separate -over image fades in at the same time.
            this.MoveToX(basePosition.X + 30, 140, Easing.InQuad)
                .Then()
                .MoveToX(basePosition.X + 20, 140, Easing.OutQuad);
            hover.FadeTo(0.16f, 150);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            // 2011 stable returns the base sprite more slowly than it enters and
            // lets the hover overlay linger while fading away.
            this.MoveToX(basePosition.X, 400, Easing.OutQuad);
            hover.FadeOut(300);
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (buttonSystemState != visibleState)
                return false;

            flash();
            action();
            return true;
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (buttonSystemState != visibleState || e.Repeat || e.ControlPressed || e.ShiftPressed || e.AltPressed || e.SuperPressed)
                return false;

            foreach (Key key in triggerKeys)
            {
                if (e.Key != key)
                    continue;

                flash();
                action();
                return true;
            }

            return false;
        }

        private void flash()
        {
            hover.ClearTransforms();
            hover.Alpha = 0.3f;
            hover.FadeOut(300, Easing.OutQuad);
        }

        private void updateVisibility(bool instant)
        {
            ClearTransforms();
            hover.ClearTransforms();
            hover.Alpha = 0;

            bool visible = buttonSystemState == visibleState;

            if (instant)
            {
                Position = basePosition;
                Alpha = visible ? 1 : 0;
                return;
            }

            if (visible)
            {
                // ChangeTier() restores the authored position immediately and fades
                // the incoming tier over 300 ms.
                Position = basePosition;
                Alpha = 0;
                this.FadeIn(300);
            }
            else
            {
                // Outgoing tier slides 50 logical pixels left while fading for 400 ms.
                this.MoveToX(basePosition.X - 50, 400, Easing.InQuad);
                this.FadeOut(400, Easing.InQuad);
            }
        }

        public override bool HandlePositionalInput => buttonSystemState == visibleState;
        public override bool HandleNonPositionalInput => buttonSystemState == visibleState;
    }
}
