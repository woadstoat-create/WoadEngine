using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WoadEngine.Scenes;

public sealed class SplashScene : Scene
{
    private Texture2D _logo;
    private float _timer;

    private const float TotalDuration = 3f;
    private const float FadeIn = 0.35f;
    private const float FadeOut = 0.45f;
    private Scene _nextScene;

    public SplashScene(Texture2D logo, Scene scene)
    {
        _logo = logo;
        _nextScene = scene;
    }

    public override void LoadContent()
    {
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _timer += dt;

        if (Core.Input.Keyboard.WasKeyPressed(Keys.Space))
        {
            GoNext();
            return;
        }

        if(_timer >= TotalDuration)
            GoNext();
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.White);
        float alpha = ComputeAlpha(_timer);
        var vp = Core.GraphicsDevice.Viewport;
        var screen = new Rectangle(0, 0, vp.Width, vp.Height);
        var dest = FitCentered(_logo.Width, _logo.Height, screen, 0.55f);

        Core.SpriteBatch.Begin();
        base.Draw(gameTime);
        Core.SpriteBatch.Draw(_logo, dest, Color.White * alpha);
        Core.SpriteBatch.End();
    }

    private void GoNext()
    {
        Core.ChangeScene(_nextScene);
    }

    private static float ComputeAlpha(float t)
    {
        if (t < FadeIn)
            return MathHelper.Clamp(t / FadeIn, 0, 1);

        float outStart = TotalDuration - FadeOut;

        if (t > outStart)
            return MathHelper.Clamp((TotalDuration - t) / FadeOut, 0, 1);

        return 1f;
    }

    private static Rectangle FitCentered(int texW, int texH, Rectangle bounds, float scaleOfScreen)
    {
        float targetW = bounds.Width * scaleOfScreen;
        float aspect = texW / (float)texH;
        float targetH = targetW / aspect;

        if (targetH > bounds.Height * scaleOfScreen)
        {
            targetH = bounds.Height * scaleOfScreen;
            targetW = targetH * aspect;
        }

        int w = (int)targetW;
        int h = (int)targetH;

        int x = bounds.X + (bounds.Width - w) / 2;
        int y = bounds.Y + (bounds.Height - h) / 2;

        return new Rectangle(x, y, w, h);
    }
}