// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
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
            // stop drawing its stock lazer button strip. The hidden ButtonArea still
            // owns the logo-flow target used by MainMenu, so modern navigation and
            // screen lifecycle behaviour remain untouched.
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

            flow.Add(createLegacyButton("PLAY", new Color4(255, 105, 180, 255),
                ButtonSystemState.TopLevel, () => buttonsTopLevel[0].TriggerClick(), Key.P, Key.M, Key.L));
            flow.Add(createLegacyButton("EDIT", new Color4(255, 183, 77, 255),
                ButtonSystemState.TopLevel, () => buttonsTopLevel[1].TriggerClick(), Key.E));
            flow.Add(createLegacyButton("BEATMAPS", new Color4(174, 213, 0, 255),
                ButtonSystemState.TopLevel, () => buttonsTopLevel[2].TriggerClick(), Key.B, Key.D));
            flow.Add(createLegacyButton("OPTIONS", new Color4(180, 180, 190, 255),
                ButtonSystemState.TopLevel, () => OnSettings?.Invoke(), Key.O, Key.S));

            if (buttonsTopLevel.Count > 3)
            {
                flow.Add(createLegacyButton("EXIT", new Color4(238, 51, 153, 255),
                    ButtonSystemState.TopLevel, () => buttonsTopLevel[3].TriggerClick(), Key.Q));
            }

            return flow;
        }

        private FillFlowContainer<LegacyMainMenuButton> createPlayMenu()
        {
            var flow = createLegacyFlow();

            flow.Add(createLegacyButton("SOLO", new Color4(255, 105, 180, 255),
                ButtonSystemState.Play, () => buttonsPlay[0].TriggerClick(), Key.P));
            flow.Add(createLegacyButton("MULTIPLAYER", new Color4(186, 104, 200, 255),
                ButtonSystemState.Play, () => buttonsPlay[1].TriggerClick(), Key.M));
            flow.Add(createLegacyButton("PLAYLISTS", new Color4(149, 117, 205, 255),
                ButtonSystemState.Play, () => buttonsPlay[2].TriggerClick(), Key.L));
            flow.Add(createLegacyButton("DAILY CHALLENGE", new Color4(121, 134, 203, 255),
                ButtonSystemState.Play, () => buttonsPlay[3].TriggerClick(), Key.D));
            flow.Add(createLegacyButton("BACK", new Color4(110, 110, 125, 255),
                ButtonSystemState.Play, () => goBack()));

            return flow;
        }

        private FillFlowContainer<LegacyMainMenuButton> createMultiMenu()
        {
            var flow = createLegacyFlow();

            flow.Add(createLegacyButton("LOUNGE", new Color4(171, 71, 188, 255),
                ButtonSystemState.Multi, () => buttonsMulti[0].TriggerClick(), Key.L, Key.M));
            flow.Add(createLegacyButton("RANKED PLAY", new Color4(126, 87, 194, 255),
                ButtonSystemState.Multi, () => buttonsMulti[1].TriggerClick(), Key.R));
            flow.Add(createLegacyButton("BACK", new Color4(110, 110, 125, 255),
                ButtonSystemState.Multi, () => goBack()));

            return flow;
        }

        private FillFlowContainer<LegacyMainMenuButton> createEditMenu()
        {
            var flow = createLegacyFlow();

            flow.Add(createLegacyButton("BEATMAP EDITOR", new Color4(255, 183, 77, 255),
                ButtonSystemState.Edit, () => buttonsEdit[0].TriggerClick(), Key.B, Key.E));
            flow.Add(createLegacyButton("SKIN EDITOR", new Color4(255, 167, 38, 255),
                ButtonSystemState.Edit, () => buttonsEdit[1].TriggerClick(), Key.S));
            flow.Add(createLegacyButton("BACK", new Color4(110, 110, 125, 255),
                ButtonSystemState.Edit, () => goBack()));

            return flow;
        }

        private static FillFlowContainer<LegacyMainMenuButton> createLegacyFlow() => new FillFlowContainer<LegacyMainMenuButton>
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 5),
            Anchor = Anchor.Centre,
            Origin = Anchor.CentreLeft,
            Position = new Vector2(135, 0),
        };

        private LegacyMainMenuButton createLegacyButton(string text, Color4 colour,
                                                         ButtonSystemState visibleState, System.Action action,
                                                         params Key[] triggerKeys)
        {
            var button = new LegacyMainMenuButton(text, colour, action, triggerKeys)
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
