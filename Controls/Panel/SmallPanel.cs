using System.Windows;
using System.Windows.Controls;

namespace WinApp.ExtControl.Controls;

/// <summary>
/// 当不需要使用Grid的分行分列，则可使用 SmallPanel
/// </summary>
public class SmallPanel : Panel
{
    /// <summary>
    /// Content measurement.
    /// </summary>
    /// <param name="constraint">Constraint</param>
    /// <returns>Desired size</returns>
    protected override Size MeasureOverride(Size constraint)
    {
        var gridDesiredSize = new Size();
        var children = InternalChildren;

        for (int i = 0, count = children.Count; i < count; ++i)
        {
            var child = children[i];
            if (child != null)
            {
                child.Measure(constraint);
                gridDesiredSize.Width = Math.Max(gridDesiredSize.Width, child.DesiredSize.Width);
                gridDesiredSize.Height = Math.Max(gridDesiredSize.Height, child.DesiredSize.Height);
            }
        }
        return (gridDesiredSize);
    }

    /// <summary>
    /// Content arrangement.
    /// </summary>
    /// <param name="arrangeSize">Arrange size</param>
    protected override Size ArrangeOverride(Size arrangeSize)
    {
        var children = InternalChildren;
        for (int i = 0, count = children.Count; i < count; ++i)
        {
            var child = children[i];
            child?.Arrange(new Rect(arrangeSize));
        }
        return (arrangeSize);
    }
}
