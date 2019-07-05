﻿using Microsoft.Xna.Framework;
using System;

namespace Yuusha.gui
{
    public class EffectLabel : Label
    {
        private int m_originalVisualAlpha;
        private bool m_fadeOut;
        private bool m_fadeIn;
        private const int m_fadeSpeed = 4;

        public string EffectName
        { get; set; }

        public DateTime TimeCreated
        { get; set; }

        public int Duration
        { get; set; }

        public bool Timeless
        { get; set; }

        public EffectLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, System.Collections.Generic.List<Enums.EAnchorType> anchors, string popUpText) : base(name, owner, rectangle, text,
                textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, textAlpha, textAlignment,
                xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride, anchors,  popUpText)
        {
            m_originalVisualAlpha = visualAlpha;
            m_fadeIn = false;
            m_fadeOut = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // effects with duration 0 at start will not have an expiration time
            if (!Timeless)
            {
                bool sendEffectsRequest = false;
                TimeSpan timeSinceCreation = DateTime.Now - TimeCreated;
                TimeSpan timeRemaining = Utils.RoundsToTimeSpan(Duration) - timeSinceCreation;

                if (timeRemaining > TimeSpan.FromSeconds(0) && timeRemaining < TimeSpan.FromSeconds(30))
                {
                    if (VisualAlpha == m_originalVisualAlpha)
                    {
                        m_fadeIn = false;
                        m_fadeOut = true;
                    }
                    else if (m_fadeOut && VisualAlpha <= 20)
                    {
                        m_fadeIn = true;
                        m_fadeOut = false;
                    }
                }
                else if (timeRemaining < TimeSpan.FromSeconds(0))
                {
                    IsVisible = false; // should be removed from EffectsWindow
                    sendEffectsRequest = true; // wondering if this is a good idea... 6/26/2019
                }

                if (IsVisible)
                {
                    if (timeRemaining < TimeSpan.FromMinutes(60))
                        PopUpText = Utils.FormatEnumString(EffectName) + " [" + string.Format("{0:D2}", timeRemaining.Minutes) + ":" + string.Format("{0:D2}", timeRemaining.Seconds) + "]";
                    else PopUpText = Utils.FormatEnumString(EffectName) + " [" + timeRemaining.ToString() + "]";

                    if (m_fadeOut)
                        VisualAlpha -= m_fadeSpeed;
                    else if (m_fadeIn)
                        VisualAlpha += m_fadeSpeed;

                    if (VisualAlpha > m_originalVisualAlpha) VisualAlpha = m_originalVisualAlpha;
                    if (VisualAlpha > 255) VisualAlpha = 255;
                    if (VisualAlpha < 0) VisualAlpha = 0;

                    // border fades in and out with the effect icon
                    if (Border != null) Border.VisualAlpha = VisualAlpha;
                }

                if (sendEffectsRequest)
                    IO.Send(Protocol.REQUEST_CHARACTER_EFFECTS);
            }
        }
    }
}