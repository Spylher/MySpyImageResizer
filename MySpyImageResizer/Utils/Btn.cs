using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace MySpyImageResizer.Utils;

public class Btn : RadioButton
{
    static Btn()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Btn), new FrameworkPropertyMetadata(typeof(Btn)));
    }
}

public class ToggleBtn : ToggleButton
{
    static ToggleBtn()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleBtn), new FrameworkPropertyMetadata(typeof(ToggleBtn)));
    }
}