using System;
using Microsoft.Maui.Controls;

namespace Yosu.Views;

public partial class CustomNavBar : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(CustomNavBar),
        default);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty ShowBackButtonProperty = BindableProperty.Create(
        nameof(ShowBackButton),
        typeof(bool),
        typeof(CustomNavBar),
        default);

    public bool ShowBackButton
    {
        get => (bool)GetValue(ShowBackButtonProperty);
        set => SetValue(ShowBackButtonProperty, value);
    }

    public static readonly BindableProperty ShowTitleLabelProperty = BindableProperty.Create(
        nameof(ShowTitleLabel),
        typeof(bool),
        typeof(CustomNavBar),
        default);

    public bool ShowTitleLabel
    {
        get => (bool)GetValue(ShowTitleLabelProperty);
        set => SetValue(ShowTitleLabelProperty, value);
    }

    public CustomNavBar()
    {
        InitializeComponent();

        ShowTitleLabel = true;
    }

    async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}