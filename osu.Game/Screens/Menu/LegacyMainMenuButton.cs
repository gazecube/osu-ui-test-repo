// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace osu.Game.Screens.Menu
{
    /// <summary>
    /// First-stage compatibility button for the stable-era main menu presentation.
    ///
    /// This deliberately keeps <see cref="MainMenuButton"/>'s input, state and
    /// transition behaviour so the modern lazer menu routing remains untouched.
    /// Only presentation geometry/background styling lives here for now.
    /// </summary>
    public partial class LegacyMainMenuButton : MainMenuButton
    {
        public LegacyMainMenuButton(LocalisableString text, string sampleName, IconUsage symbol, Color4 colour,
                                    Action<MainMenuButton, UIEvent>? clickAction = null, params Key[] triggerKeys)
            : base(text, sampleName, symbol, colour, clickAction, triggerKeys)
        {
            // stable's menu buttons read as longer, flatter strips alongside the logo.
            // Keep the current ButtonArea height so all inherited state animation and
            // logo-flow calculations continue to work unchanged.
            BaseSize = new Vector2(185, ButtonArea.BUTTON_AREA_HEIGHT);
        }

        protected override Drawable CreateBackground(Colour4 accentColour) => new Container
        {
            Children = new Drawable[]
            {
                // A deliberately simple, flat stable-style base. This is code-only
                // until the historical resource licensing/packaging path is settled.
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = accentColour,
                },
                new Box
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 2,
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    Colour = Color4.White,
                    Alpha = 0.18f,
                },
                new Box
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 3,
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Colour = Color4.Black,
                    Alpha = 0.28f,
                },
            }
        };
    }
}
