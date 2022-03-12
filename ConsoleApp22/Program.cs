// See https://aka.ms/new-console-template for more information
using ConsoleApp22;
using ConsoleApp22.Components;

Console.WriteLine("Hello, World!");

var renderer = new MjmlRenderer();

renderer.Add(new BodyComponent());
renderer.Add(new ButtonComponent());
renderer.Add(new RootComponent());

var xml = @"
<mjml>
  <mj-body background-color=""red"">

  </mj-body>
</mjml>";

var result = renderer.Render(xml);

Console.WriteLine(result);