using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.Rendering;

public class Animation
{
    public List<TextureRegion> Frames { get; set; }
    public TimeSpan Delay { get; set; }

    public Animation()
    {
        Frames = new List<TextureRegion>();
        Delay = TimeSpan.FromMilliseconds(100);
    }

    public Animation(List<TextureRegion> frames, TimeSpan delay)
    {
        Frames = frames;
        Delay = delay;
    }
}