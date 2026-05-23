using Strawberry.Components;
using Strawberry.Core;
using Strawberry.Graphics;
using Strawberry.Graphics.Layers;
using Strawberry.Graphics.Text;
using Strawberry.Math;
using Strawberry.Sound;
using Strawberry.Sound.Midi;
using Color = Strawberry.Graphics.Color;

namespace Strawberry.Test
{
    public class MyGameContext : StdGameContext
    {
        public const float ppm = 32f;
        public MyGameContext() : base(1280, 720)
        {

        }

        public override void OnInitialize(IGameLauncher laucnher)
        {
            base.OnInitialize(laucnher);
            var size = GraphicsContext.GetScreenSize();
            Viewport viewport = new Viewport("Default", new Vector2(), size, new Vector2(), new Vector2(1280, 720));
            SpriteLayer layer = new SpriteLayer();
            //BackgroundLayer bg = new BackgroundLayer();
            layer.Sorter = new IsometricRenderingSorter();


            var pixelSprite = new Sprite(GraphicsContext.PixelTexture, 1, new Vector2(1f, 1f), new Vector2(0f, 0f),
                new Vector2(1f, 1f), new Vector2());

            /*bg.Sprite = pixelSprite;
            bg.Scale = new Vector2(32f, 32f);
            bg.TileH = -1;
            bg.TileV = -1;
            bg.Color = Color.Green;*/

            Scene scene = new Scene("Main", 1280, 720);
            scene.ClearColor = Color.CornflowerBlue;
            AddScene(scene);
            SetScene("Main");
            scene.Viewports[0] = viewport;
            scene.EnablePhysics(Vector2.Down() * 9.8f);
            scene.AddLayer("Sprite1", layer);


            Entity entity = new Entity();
            entity.Initialize("test", scene);
            entity.AddComponent<TransformComponent>().Scale = new Vector2(128f, 32f);
            entity.AddComponent<SpriteComponent>().Setup(pixelSprite, 0, new Vector2(), Color.Red, layer);
            entity.GetComponent<TransformComponent>().Position = new Vector2(16f, 320f);
            entity.AddComponent<StaticBodyComponent>();
            entity.AddComponent<MoveToDirectionComponent>().Speed = 64f;
            entity.AddComponent<SoundListenerComponent>();
            var text = entity.AddComponent<TextRendererComponent>();
            text.Layer = layer;
            text.Color = Color.Orange;
            text.Font = new Font(GraphicsContext, laucnher.Storage.ReadAllBytes("BYagut.font"));
            text.Position = new Vector2(1280 - 64f, 16);
            text.Size = 36;
            text.Text = "اسپیس را نگه دارید";


            Entity entity2 = new Entity();
            entity2.Initialize("test2", scene);
            entity2.AddComponent<TransformComponent>().Scale = new Vector2(32f, 32f);
            entity2.AddComponent<SpriteComponent>().Setup(pixelSprite, 0, new Vector2(), Color.Blue, layer);
            entity2.GetComponent<TransformComponent>().Position = new Vector2(112f, 128f);
            entity2.GetComponent<TransformComponent>().Angle = 16f;
            entity2.AddComponent<PhysicsBodyComponent>();


            Entity entity3 = new Entity();
            entity3.Initialize("test3", scene);
            entity3.AddComponent<TransformComponent>().Scale = new Vector2(32f, 32f);
            entity3.AddComponent<SpriteComponent>().Setup(pixelSprite, 0, new Vector2(), Color.Lime, layer);
            entity3.GetComponent<TransformComponent>().Position = new Vector2(144f, -64f);
            entity3.GetComponent<TransformComponent>().Angle = 356f;
            entity3.AddComponent<PhysicsBodyComponent>();

            using (WaveReader waveReader = new WaveReader(Storage.Open("timeup_mono.wav")))
            {
                var buffer = SoundManager.CreateSoundBuffer(waveReader);
                var s = buffer.Play(Vector2.Right() * 256f, true);
                s.Volume = 1f;
                var volume = s.Volume;
            }

            var music01 = SoundManager.CreateStream(new MidiReader(Storage.Open("music.mid"), Storage.Open("8MBGMSFX.SF2")));
            music01.Volume = 0.1f;
            music01.Play(true);
            // music01.CurrentPlayTime = 0;
            // Console.WriteLine(music01.Seconds);
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
}
