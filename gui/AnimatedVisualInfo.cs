namespace Yuusha.gui
{
    public class AnimatedVisualInfo 
    {
        public string AnimationName
        { get; private set; }
        public string PrefixName
        { get; private set; }
        public int NumFrames
        { get; private set; }
        public int Frame
        { get; private set; }
        public int FramesPerSecond
        { get; set; }

        public AnimatedVisualInfo(string animationName, string prefixName, int numFrames, int fps)
        {
            AnimationName = animationName;
            PrefixName = prefixName;// name.Substring(0, name.IndexOf("_") - 1);
            NumFrames = numFrames;
            FramesPerSecond = fps;
        }
    }
}
