// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace osu.Game.Screens.Menu
{
    public partial class ButtonSystem
    {
        private Container? legacyMenuRoot;
        private readonly List<LegacyMainMenuButton> legacyMenuButtons = new List<LegacyMainMenuButton>();

        protected override void LoadComplete()
        {
            base.LoadComplete();

            // Keep the current ButtonSystem alive as the state/routing engine, but
            // stop drawing its stock lazer button strip. In particular, the hidden
            // ButtonArea still owns the logo-flow target used by MainMenu, so none
            // of the modern screen lifecycle/navigation code has to be replaced.
            buttonArea.Alpha = 0;

            AddInternal(legacyMenuRoot = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Depth = float.MinValue,
                Children = new Drawable[]
                {
                    createTopLevelMenu(),
                    createPlayMenu(),
                    createMultiMenu(),
                    createEditMenu(),
                }
            });

            StateChanged += updateLegacyMenuState;
            updateLegacyMenuState(State);
        }

        private FillFlowContainer<LegacyMainMenuButton> createTopLevelMenu()
        {
            var flow = createLegacyFlow();

            flow.Add(createLegacyButton("PLAY", OsuIcon.Logo, new Color4(102, 68, 204, 255),
                ButtonSystemState.TopLevel, () => buttonsTopLevel[0].TriggerClick(), Key.P, Key.M, Key.L));
            flow.Add(createLegacyButton("EDIT", OsuIcon.EditCircle, new Color4(238, 170, 0, 255),
                ButtonSystemState.TopLevel, () => buttonsTopLevel[1].TriggerClick(), Key.E));
            flow.Add(createLegacyButton("BEATMAPS", OsuIcon.Beatmap, new Color4(165, 204, 0, 255),
                ButtonSystemState.TopLevel, () => buttonsTopLevel[2].TriggerClick(), Key.B, Key.D));
            flow.Add(createLegacyButton("OPTIONS", OsuIcon.Settings, new Color4(85, 85, 85, 255),
                ButtonSystemState.TopLevel, () => OnSettings?.Invoke(), Key.O, Key.S));

            if (buttonsTopLevel.Count > 3)
            {
                flow.Add(createLegacyButton("EXIT", OsuIcon.CrossCircle, new Color4(238, 51, 153, 255),
                    ButtonSystemState.TopLevel, () => buttonsTopLevel[3].TriggerClick(), Key.Q));
            }

            return flow;
        }

        private FillFlowContainer<LegacyMainMenuButton> createPlayMenu()
        {
            var flow = createLegacyFlow();

            flow.Add(createLegacyButton("SOLO", OsuIcon.Player, new Color4(102, 68, 204, 255),
                ButtonSystemState.Play, () => buttonsPlay[0].TriggerClick(), Key.P));
            flow.Add(createLegacyButton("MULTIPLAYER", OsuIcon.Online, new Color4(94, 63, 186, 255),
                ButtonSystemState.Play, () => buttonsPlay[1].TriggerClick(), Key.M));
            flow.Add(createLegacyButton("PLAYLISTS", OsuIcon.Tournament, new Color4(94, 63, 186, 255),
                ButtonSystemState.Play, () => buttonsPlay[2].TriggerClick(), Key.L));
            flow.Add(createLegacyButton("DAILY CHALLENGE", FontAwesome.Solid.Bolt, new Color4(94, 63, 186, 255),
                ButtonSystemState.Play, () => buttonsPlay[3].TriggerClick(), Key.D));
            flow.Add(createLegacyButton("BACK", OsuIcon.PrevCircle, new Color4(51, 58, 94, 255),
                ButtonSystemState.Play, () => goBack()));

            return flow;
        }

        private FillFlowContainer<LegacyMainMenuButton> createMultiMenu()
        {
            var flow = createLegacyFlow();

            flow.Add(createLegacyButton("LOUNGE", FontAwesome.Solid.Couch, new Color4(94, 63, 186, 255),
                ButtonSystemState.Multi, () => buttonsMulti[0].TriggerClick(), Key.L, Key.M));
            flow.Add(createLegacyButton("RANKED PLAY", FontAwesome.Solid.Crown, new Color4(94, 63, 186, 255),
                ButtonSystemState.Multi, () => buttonsMulti[1].TriggerClick(), Key.R));
            flow.Add(createLegacyButton("BACK", OsuIcon.PrevCircle, new Color4(51, 58, 94, 255),
                ButtonSystemState.Multi, () => goBack()));

            return flow;
        }

        private FillFlowContainer<LegacyMainMenuButton> createEditMenu()
        {
            var flow = createLegacyFlow();

            flow.Add(createLegacyButton("BEATMAP EDITOR", OsuIcon.Beatmap, new Color4(238, 170, 0, 255),
                ButtonSystemState.Edit, () => buttonsEdit[0].TriggerClick(), Key.B, Key.E));
            flow.Add(createLegacyButton("SKIN EDITOR", OsuIcon.SkinB, new Color4(220, 160, 0, 255),
                ButtonSystemState.Edit, () => buttonsEdit[1].TriggerClick(), Key.S));
            flow.Add(createLegacyButton("BACK", OsuIcon.PrevCircle, new Color4(51, 58, 94, 255),
                ButtonSystemState.Edit, () => goBack()));

            return flow;
        }

        private static FillFlowContainer<LegacyMainMenuButton> createLegacyFlow() => new FillFlowContainer<LegacyMainMenuButton>
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 4),
            Anchor = Anchor.Centre,
            Origin = Anchor.CentreLeft,
            Position = new Vector2(135, 0),
        };

        private LegacyMainMenuButton createLegacyButton(string text, IconUsage icon, Color4 colour,
                                                         ButtonSystemState visibleState, System.Action action,
                                                         params Key[] triggerKeys)
        {
            var button = new LegacyMainMenuButton(text, @"button-default-select", icon, colour, (_, _) => action(), triggerKeys)
            {
                VisibleState = visibleState,
            };

            legacyMenuButtons.Add(button);
            return button;
        }

        private void updateLegacyMenuState(ButtonSystemState newState)
        {
            foreach (var button in legacyMenuButtons)
                button.ButtonSystemState = newState;
        }
    }
}
