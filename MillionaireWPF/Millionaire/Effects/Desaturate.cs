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
    public class Desaturate : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty(
            "Input",
            typeof(Desaturate),
            0);

        public static readonly DependencyProperty StrengthProperty = DependencyProperty.Register(
            "Strength",
            typeof(double),
            typeof(Desaturate),
            new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public Desaturate()
        {
            var pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri("/Millionaire;component/Effects/Desaturate.ps", UriKind.Relative);
            
            this.PixelShader = pixelShader;
            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(StrengthProperty);
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
    }
}
