// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.Menu
{
    public partial class MainMenu
    {
        private FillFlowContainer<Container>? legacyBottomBanners;

        /// <summary>
        /// Keep modern lazer's screen/navigation implementation, while suppressing
        /// presentation elements that do not belong to the stable-style main menu.
        ///
        /// This is deliberately kept separate from the normal MainMenu implementation
        /// so the compatibility layer remains easy to remove or gate behind a setting.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            // Several of these are animated back in by MainMenu when returning from
            // another screen, so keep their presentation disabled here rather than
            // fighting each individual transition path.
            if (bottomElementsFlow != null)
                bottomElementsFlow.Alpha = 0;

            if (supporterDisplay != null)
                supporterDisplay.Alpha = 0;

            if (songTicker != null)
                songTicker.Alpha = 0;

            if (sideFlashes != null)
                sideFlashes.Alpha = 0;

            if (legacyBottomBanners == null)
                AddInternal(createLegacyBottomBanners());
        }

        private Drawable createLegacyBottomBanners()
        {
            legacyBottomBanners = new FillFlowContainer<Container>
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(6, 0),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Position = new Vector2(0, -34),
                Depth = float.MinValue,
            };

            legacyBottomBanners.Add(createLegacyBanner("osu! news", new Color4(238, 51, 153, 255)));
            legacyBottomBanners.Add(createLegacyBanner("beatmap packs", new Color4(174, 213, 0, 255)));
            legacyBottomBanners.Add(createLegacyBanner("support osu!", new Color4(186, 104, 200, 255)));

            return legacyBottomBanners;
        }

        private static Container createLegacyBanner(string text, Color4 accentColour) => new Container
        {
            Size = new Vector2(190, 42),
            Masking = true,
            CornerRadius = 2,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = new Color4(24, 24, 30, 220),
                },
                new Box
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 3,
                    Colour = accentColour,
                },
                new OsuSpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = text,
                    Shadow = true,
                },
            }
        };
    }
}
