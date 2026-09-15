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
        private const float legacy_field_width = 640;
        private const float legacy_field_height = 480;

        private Container? legacyMenuRoot;
        private Container? legacyField;
        private readonly List<LegacyMainMenuButton> legacyMenuButtons = new List<LegacyMainMenuButton>();

        protected override void LoadComplete()
        {
            base.LoadComplete();

            // Keep lazer's routing/state engine alive, but remove its visual button
            // strip. The replacement below uses stable's 640x480 logical field.
            buttonArea.Alpha = 0;

            AddInternal(legacyMenuRoot = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Depth = float.MinValue,
                Child = legacyField = new Container
                {
                    Size = new Vector2(legacy_field_width, legacy_field_height),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                }
            });

            createTopLevelMenu();
            createPlayMenu();
            createModernFallbackMenus();

            StateChanged += updateLegacyMenuState;
            updateLegacyMenuState(State);
        }

        protected override void Update()
        {
            base.Update();

            if (legacyField == null)
                return;

            // Stable's WindowRatio is WindowHeight / 480. This keeps the authored
            // 640x480 field centred and scales solely from height, including on
            // widescreen displays.
            float scale = DrawHeight / legacy_field_height;
            legacyField.Scale = new Vector2(scale);
        }

        private void createTopLevelMenu()
        {
            // Exact 2011 Menu.cs StandardSnapCentre offsets.
            addLegacyButton("PLAY", new Color4(255, 105, 180, 255), new Vector2(-122, -145),
                ButtonSystemState.TopLevel, () => buttonsTopLevel[0].TriggerClick(), Key.P);

            addLegacyButton("EDIT", new Color4(255, 183, 77, 255), new Vector2(-102, -80),
                ButtonSystemState.TopLevel, () => buttonsTopLevel[1].TriggerClick(), Key.E);

            addLegacyButton("OPTIONS", new Color4(180, 180, 190, 255), new Vector2(-112, -15),
                ButtonSystemState.TopLevel, () => OnSettings?.Invoke(), Key.O);

            if (buttonsTopLevel.Count > 3)
            {
                addLegacyButton("EXIT", new Color4(238, 51, 153, 255), new Vector2(-132, 50),
                    ButtonSystemState.TopLevel, () => buttonsTopLevel[3].TriggerClick(), Key.Q, Key.Escape);
            }
        }

        private void createPlayMenu()
        {
            // The original Play tier uses the same four authored slots as the main
            // tier, with its own intentionally staggered X offsets.
            addLegacyButton("FREEPLAY", new Color4(255, 105, 180, 255), new Vector2(-122, -145),
                ButtonSystemState.Play, () => buttonsPlay[0].TriggerClick(), Key.S, Key.P);

            addLegacyButton("MULTIPLAYER", new Color4(186, 104, 200, 255), new Vector2(-102, -80),
                ButtonSystemState.Play, () => buttonsPlay[1].TriggerClick(), Key.M);

            // Modern lazer has more destinations than 2011 stable. Keep one current
            // action behind the historical SPECIAL slot until its old routing is
            // reproduced separately.
            if (buttonsPlay.Count > 2)
            {
                addLegacyButton("SPECIAL", new Color4(149, 117, 205, 255), new Vector2(-112, -15),
                    ButtonSystemState.Play, () => buttonsPlay[2].TriggerClick(), Key.P);
            }

            addLegacyButton("BACK", new Color4(110, 110, 125, 255), new Vector2(-132, 50),
                ButtonSystemState.Play, goBack, Key.B, Key.Escape);
        }

        private void createModernFallbackMenus()
        {
            // These states do not exist in the 2011 main-menu hierarchy, but keeping
            // compact fallbacks preserves current lazer navigation while the historical
            // equivalents are reconstructed.
            addLegacyButton("LOUNGE", new Color4(171, 71, 188, 255), new Vector2(-122, -145),
                ButtonSystemState.Multi, () => buttonsMulti[0].TriggerClick(), Key.L, Key.M);
            addLegacyButton("RANKED PLAY", new Color4(126, 87, 194, 255), new Vector2(-102, -80),
                ButtonSystemState.Multi, () => buttonsMulti[1].TriggerClick(), Key.R);
            addLegacyButton("BACK", new Color4(110, 110, 125, 255), new Vector2(-132, 50),
                ButtonSystemState.Multi, goBack, Key.B, Key.Escape);

            addLegacyButton("BEATMAP EDITOR", new Color4(255, 183, 77, 255), new Vector2(-122, -145),
                ButtonSystemState.Edit, () => buttonsEdit[0].TriggerClick(), Key.B, Key.E);
            addLegacyButton("SKIN EDITOR", new Color4(255, 167, 38, 255), new Vector2(-102, -80),
                ButtonSystemState.Edit, () => buttonsEdit[1].TriggerClick(), Key.S);
            addLegacyButton("BACK", new Color4(110, 110, 125, 255), new Vector2(-132, 50),
                ButtonSystemState.Edit, goBack, Key.Escape);
        }

        private void addLegacyButton(string text, Color4 colour, Vector2 position,
                                     ButtonSystemState visibleState, System.Action action,
                                     params Key[] triggerKeys)
        {
            if (legacyField == null)
                return;

            var button = new LegacyMainMenuButton(text, colour, position, action, triggerKeys)
            {
                VisibleState = visibleState,
            };

            legacyMenuButtons.Add(button);
            legacyField.Add(button);
        }

        private void updateLegacyMenuState(ButtonSystemState newState)
        {
            foreach (var button in legacyMenuButtons)
                button.ButtonSystemState = newState;
        }
    }
}
