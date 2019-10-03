using System;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class AnimatedVisual
    {
        private int m_currentFrame;
        private int m_totalFrames;
        private float m_timeUntilNextFrame = 0;

        public bool IsPlaying { get; private set; }
        public bool IsPaused { get; private set; }
        public Point Position
        { get; private set; }
        public int FrameInterval { get; set; }
        public int FramesPerSecond { get; set; }
        public bool IsLooping { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color TintColor { get; private set; }
        public int VisualAlpha { get; private set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }

        public AnimatedVisualInfo AnimationInfo { get; private set; }

        public AnimatedVisual(AnimatedVisualInfo animationInfo, Point pos, int width, int height, Color tintColor, int visualAlpha)
        {
            AnimationInfo = animationInfo;
            m_currentFrame = 0;
            FramesPerSecond = animationInfo.FramesPerSecond;
            m_totalFrames = animationInfo.NumFrames;
            Position = pos;
            Width = width;
            Height = height;
            TintColor = tintColor;
            VisualAlpha = visualAlpha;

            IsLooping = true;
            IsPlaying = true;
            IsPaused = false;
        }

        public AnimatedVisual(AnimatedVisualInfo animationInfo, Point pos, int width, int height, Color tintColor, int visualAlpha, bool randomFrame)
        {
            AnimationInfo = animationInfo;
            m_currentFrame = randomFrame ? new Random(Guid.NewGuid().GetHashCode()).Next(0, animationInfo.NumFrames - 1) : 0;
            FramesPerSecond = animationInfo.FramesPerSecond;
            m_totalFrames = animationInfo.NumFrames;
            Position = pos;
            Width = width;
            Height = height;
            TintColor = tintColor;
            VisualAlpha = visualAlpha;

            IsLooping = true;
            IsPlaying = true;
            IsPaused = false;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsPlaying || IsPaused) return;

            float animationTimeFrame = 1f / (float)FramesPerSecond;
            float gameFrameTime = (float)Program.Client.ClientGameTime.ElapsedGameTime.TotalSeconds;
            m_timeUntilNextFrame -= gameFrameTime;

            if (m_timeUntilNextFrame <= 0)
            {
                m_currentFrame++;

                if (m_currentFrame == m_totalFrames)
                {
                    m_currentFrame = 0;

                    if(!IsLooping)
                        IsPlaying = false;
                }

                m_timeUntilNextFrame += animationTimeFrame;
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (IsPlaying && !IsPaused)
            {
                string vkName = AnimationInfo.PrefixName + m_currentFrame;
                if (GuiManager.Visuals.ContainsKey(vkName))
                {
                    VisualInfo vi = GuiManager.Visuals[vkName];
                    Rectangle sourceRect = new Rectangle(vi.X, vi.Y, vi.Width, vi.Height);
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], new Rectangle(Position, new Point(Width, Height)), sourceRect, TintColor);
                }
                else Utils.LogOnce("Visuals does not contain AnimatedVisual key [ " + vkName + " ]");
            }
        }

        public void SetPosition(Point pt)
        {
            Position = new Point(pt.X + XOffset, pt.Y + YOffset);
        }
    }
}
