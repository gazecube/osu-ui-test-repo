// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Screens.Menu
{
    public partial class MainMenu
    {
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
        }
    }
}
