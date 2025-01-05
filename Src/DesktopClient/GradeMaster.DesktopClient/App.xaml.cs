﻿namespace GradeMaster.DesktopClient;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new MainPage()) { Title = "GradeMaster", MinimumHeight = 640, MinimumWidth = 500 };
    }
}
