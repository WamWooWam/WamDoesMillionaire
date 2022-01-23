using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;

namespace Millionaire.Effects
{
    public class DesaturateAndTint : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(
            "Input",
            typeof(DesaturateAndTint),
            0);

        public static readonly DependencyProperty StrengthProperty = DependencyProperty.Register(
            "Strength",
            typeof(double),
            typeof(DesaturateAndTint),
            new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty TintColorProperty = DependencyProperty.Register(
            "TintColor",
            typeof(Color),
            typeof(DesaturateAndTint),
            new UIPropertyMetadata(Colors.White, PixelShaderConstantCallback(1)));

        public DesaturateAndTint()
        {
            var pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri("/Millionaire;component/Effects/DesaturateAndTint.ps", UriKind.Relative);
            
            this.PixelShader = pixelShader;
            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(StrengthProperty);
            this.UpdateShaderValue(TintColorProperty);
        }

        public Brush Input
        {
            get => ((Brush)(this.GetValue(InputProperty)));
            set => this.SetValue(InputProperty, value);
        }

        /// <summary>The strength of the effect. 0 is unchanged and 1 is monochrome</summary>
        public double Strength
        {
            get => ((double)(this.GetValue(StrengthProperty)));
            set => this.SetValue(StrengthProperty, value);
        }

        /// <summary>The strength of the effect. 0 is unchanged and 1 is monochrome</summary>
        public Color TintColor
        {
            get => ((Color)(this.GetValue(TintColorProperty)));
            set => this.SetValue(TintColorProperty, value);
        }
    }
}
