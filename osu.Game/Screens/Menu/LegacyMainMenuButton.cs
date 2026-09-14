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
    /// Presentation-only main-menu button modelled after the stable-era vertical menu.
    ///
    /// This intentionally does not derive from <see cref="MainMenuButton"/>. The latter
    /// carries lazer-specific wedge geometry, icons and beat-synchronised hover effects.
    /// Keeping this drawable independent lets the stable presentation evolve without
    /// disturbing the modern <see cref="ButtonSystem"/> which remains the routing/state
    /// engine underneath it.
    /// </summary>
    public partial class LegacyMainMenuButton : CompositeDrawable
    {
        private const float button_width = 235;
        private const float button_height = 44;

        private readonly Action action;
        private readonly Key[] triggerKeys;
        private readonly Box background;
        private readonly Box accent;
        private readonly Box hover;
        private readonly OsuSpriteText label;

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

        public LegacyMainMenuButton(string text, Color4 colour, Action action, params Key[] triggerKeys)
        {
            this.action = action;
            this.triggerKeys = triggerKeys;

            Size = new Vector2(button_width, button_height);
            Masking = true;
            CornerRadius = 2;
            Alpha = 0;

            InternalChildren = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = new Color4(24, 24, 30, 235),
                },
                accent = new Box
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
                label = new OsuSpriteText
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Position = new Vector2(22, 0),
                    Text = text,
                    Font = OsuFont.GetFont(size: 20, weight: FontWeight.Regular),
                    Shadow = true,
                },
            };
        }

        protected override bool OnHover(HoverEvent e)
        {
            if (buttonSystemState != visibleState)
                return false;

            hover.FadeTo(0.12f, 80);
            accent.ResizeWidthTo(11, 100, Easing.OutQuint);
            label.MoveToX(28, 100, Easing.OutQuint);
            this.ScaleTo(1.025f, 100, Easing.OutQuint);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.FadeOut(120);
            accent.ResizeWidthTo(6, 120, Easing.OutQuint);
            label.MoveToX(22, 120, Easing.OutQuint);
            this.ScaleTo(1, 120, Easing.OutQuint);
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
            hover.Alpha = 0.35f;
            hover.FadeOut(250, Easing.OutQuint);
        }

        private void updateVisibility(bool instant)
        {
            ClearTransforms();

            bool visible = buttonSystemState == visibleState;

            if (instant)
            {
                Alpha = visible ? 1 : 0;
                X = visible ? 0 : 30;
                return;
            }

            if (visible)
            {
                X = 30;
                Alpha = 0;
                this.MoveToX(0, 220, Easing.OutQuint);
                this.FadeIn(120);
            }
            else
            {
                this.MoveToX(-18, 130, Easing.InQuad);
                this.FadeOut(90);
            }
        }

        public override bool HandlePositionalInput => buttonSystemState == visibleState;
        public override bool HandleNonPositionalInput => buttonSystemState == visibleState;
    }
}
