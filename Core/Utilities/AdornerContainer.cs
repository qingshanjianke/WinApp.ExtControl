using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WinApp.ExtControl.Utilities;

public class AdornerContainer : Adorner
{
    public AdornerContainer(UIElement adornedElement) : base(adornedElement) { }

    private UIElement? _child;
    public UIElement? Child
    {
        get => _child;
        set
        {
            if (value == null)
            {
                RemoveVisualChild(_child);
                _child = value;
                return;
            }
            AddVisualChild(value);
            _child = value;
        }
    }

    protected override int VisualChildrenCount => _child != null ? 1 : 0;

    protected override Size ArrangeOverride(Size finalSize)
    {
        _child?.Arrange(new Rect(finalSize));
        return finalSize;
    }

    protected override Visual GetVisualChild(int index)
    {
        return index == 0 && _child != null ? _child : base.GetVisualChild(index);
    }
}
