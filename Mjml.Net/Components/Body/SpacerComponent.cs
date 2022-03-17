﻿namespace Mjml.Net.Components.Body
{
    public partial struct SpacerProps
    {
        [Bind("border")]
        public string? Border;

        [Bind("border-bottom")]
        public string? BorderBottom;

        [Bind("border-left")]
        public string? BorderLeft;

        [Bind("border-right")]
        public string? BorderRight;

        [Bind("border-top")]
        public string? BorderTop;

        [Bind("container-background-color", BindType.Color)]
        public string? ContainerBackgroundColor;

        [Bind("height", BindType.Pixels)]
        public string Height = "20px";

        [Bind("padding", BindType.PixelsOrPercent)]
        public string? Padding;

        [Bind("padding-bottom", BindType.PixelsOrPercent)]
        public string? PaddingBottom;

        [Bind("padding-left", BindType.PixelsOrPercent)]
        public string? PaddingLeft;

        [Bind("padding-right", BindType.PixelsOrPercent)]
        public string? PaddingRight;

        [Bind("padding-top", BindType.PixelsOrPercent)]
        public string? PaddingTop;
    }

    public sealed class SpacerComponent : BodyComponentBase<SpacerProps>
    {
        public override string ComponentName => "mj-spacer";

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var props = new SpacerProps(node);

            renderer.ElementStart("div")
                .Style("height", props.Height)
                .Style("line-height", props.Height);

            renderer.Content("&#8202;");

            renderer.ElementEnd("div");
        }
    }
}
