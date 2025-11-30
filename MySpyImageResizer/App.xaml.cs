using System.Windows;
using Syncfusion.SfSkinManager;
using Syncfusion.Themes.Windows11Dark.WPF;
using MessageBox = System.Windows.Forms.MessageBox;

namespace MySpyImageResizer;

public partial class App : Application
{
    public App()
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH5dcHVWRmhdU01+V0NWYEg=");
    }

    public static void InitializeTheme(DependencyObject dependencyObject)
    {
        //SfSkinManager.ApplyStylesOnApplication = true;
        SfSkinManager.SetTheme(dependencyObject, new Theme("Windows11Dark"));
        var defaultTheme = new Windows11DarkThemeSettings { Palette = Windows11Palette.PurpleShadowDark };
        SfSkinManager.RegisterThemeSettings("Windows11Dark", defaultTheme);
    }
}