using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Yosu.Controls;

public class ProgressBar : SKCanvasView
{
    private SKCanvas _canvas = default!;
    private SKRect _drawRect;
    private SKImageInfo _info;

    public float Progress
    {
        get => (float)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public Color ProgressColor
    {
        get => (Color)GetValue(ProgressColorProperty);
        set => SetValue(ProgressColorProperty, value);
    }

    public Color GradientColor
    {
        get => (Color)GetValue(GradientColorProperty);
        set => SetValue(GradientColorProperty, value);
    }

    public Color BaseColor
    {
        get => (Color)GetValue(BaseColorProperty);
        set => SetValue(BaseColorProperty, value);
    }

    public float LowerRangeValue
    {
        get => (float)GetValue(LowerRangeValueProperty);
        set => SetValue(LowerRangeValueProperty, value);
    }

    public float UpperRangeValue
    {
        get => (float)GetValue(UpperRangeValueProperty);
        set => SetValue(UpperRangeValueProperty, value);
    }

    public bool UseGradient
    {
        get => (bool)GetValue(UseGradientProperty);
        set => SetValue(UseGradientProperty, value);
    }

    public bool IsIntermediate
    {
        get => (bool)GetValue(IsIntermediateProperty);
        set => SetValue(IsIntermediateProperty, value);
    }

    public static readonly BindableProperty ProgressProperty = BindableProperty.Create(
        nameof(Progress),
        typeof(float),
        typeof(ProgressBar),
        0.0f,
        propertyChanged: OnBindablePropertyChanged
    );
    public static readonly BindableProperty ProgressColorProperty = BindableProperty.Create(
        nameof(ProgressColor),
        typeof(Color),
        typeof(ProgressBar),
        Colors.BlueViolet,
        propertyChanged: OnBindablePropertyChanged
    );
    public static readonly BindableProperty GradientColorProperty = BindableProperty.Create(
        nameof(GradientColor),
        typeof(Color),
        typeof(ProgressBar),
        Colors.BlueViolet,
        propertyChanged: OnBindablePropertyChanged
    );
    public static readonly BindableProperty BaseColorProperty = BindableProperty.Create(
        nameof(BaseColor),
        typeof(Color),
        typeof(ProgressBar),
        Colors.LightGray,
        propertyChanged: OnBindablePropertyChanged
    );
    public static readonly BindableProperty IsIntermediateProperty = BindableProperty.Create(
        nameof(IsIntermediate),
        typeof(bool),
        typeof(ProgressBar),
        false,
        propertyChanged: OnIsIntermediatePropertyChanged
    );
    public static readonly BindableProperty LowerRangeValueProperty = BindableProperty.Create(
        nameof(LowerRangeValue),
        typeof(float),
        typeof(ProgressBar),
        0.0f,
        propertyChanged: OnBindablePropertyChanged
    );
    public static readonly BindableProperty UpperRangeValueProperty = BindableProperty.Create(
        nameof(UpperRangeValue),
        typeof(float),
        typeof(ProgressBar),
        0.0f,
        propertyChanged: OnBindablePropertyChanged
    );
    public static readonly BindableProperty UseGradientProperty = BindableProperty.Create(
        nameof(UseGradient),
        typeof(bool),
        typeof(ProgressBar),
        false,
        propertyChanged: OnBindablePropertyChanged
    );

    private static void OnBindablePropertyChanged(
        BindableObject bindable,
        object oldValue,
        object newValue
    )
    {
        ((ProgressBar)bindable).InvalidateSurface();
    }

    private static void OnIsIntermediatePropertyChanged(
        BindableObject bindable,
        object oldValue,
        object newValue
    )
    {
        var progressBar = (ProgressBar)bindable;

        if (progressBar.IsIntermediate)
        {
            var lowerAnimation = new Animation(
                v => progressBar.LowerRangeValue = (float)v,
                -0.4,
                1.0
            );
            var upperAnimation = new Animation(
                v => progressBar.UpperRangeValue = (float)v,
                0.0,
                1.4
            );

            lowerAnimation.Commit(
                progressBar,
                "lower",
                length: 1500,
                easing: Easing.CubicInOut,
                repeat: () => true
            );
            upperAnimation.Commit(
                progressBar,
                "upper",
                length: 1500,
                easing: Easing.CubicInOut,
                repeat: () => true
            );
        }
        else
        {
            //progressBar.CancelAnimations();
            progressBar.AbortAnimation("lower");
            progressBar.AbortAnimation("upper");

            //progressBar.Progress = 0;
        }

        ((ProgressBar)bindable).InvalidateSurface();
    }

    public ProgressBar()
    {
        IgnorePixelScaling = false;
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);

        _canvas = e.Surface.Canvas;
        _canvas.Clear();

        _info = e.Info;

        _drawRect = new SKRect(0, 0, _info.Width, _info.Height);

        DrawBase();
        DrawProgress();
    }

    private void DrawBase()
    {
        using var basePath = new SKPath();
        basePath.AddRect(_drawRect);
        _canvas.DrawPath(
            basePath,
            new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = BaseColor.ToSKColor(),
                IsAntialias = true
            }
        );
    }

    private void DrawProgress()
    {
        using var progressPath = new SKPath();

        var progressRect = IsIntermediate
            ? new SKRect(
                _info.Width * LowerRangeValue,
                0,
                _info.Width * UpperRangeValue,
                _info.Height
            )
            : new SKRect(0, 0, _info.Width * Progress, _info.Height);

        progressPath.AddRect(progressRect);

        using var progressPaint = new SKPaint { Style = SKPaintStyle.Fill, IsAntialias = true };

        if (UseGradient)
        {
            progressPaint.Shader = SKShader.CreateLinearGradient(
                new SKPoint(_drawRect.Left, _drawRect.MidY),
                new SKPoint(_drawRect.Right, _drawRect.MidY),
                [GradientColor.ToSKColor(), ProgressColor.ToSKColor()],
                [0.0f, 1.0f],
                SKShaderTileMode.Clamp
            );
        }
        else
        {
            progressPaint.Color = ProgressColor.ToSKColor();
        }

        _canvas.DrawPath(progressPath, progressPaint);
    }
}
