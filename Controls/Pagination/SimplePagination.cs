using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WinApp.ExtControl.Controls;

public class SimplePagination : Control
{
    private static readonly Type _typeofSelf = typeof(SimplePagination);

    static SimplePagination()
    {
        InitializeCommands();

        DefaultStyleKeyProperty.OverrideMetadata(_typeofSelf, new FrameworkPropertyMetadata(_typeofSelf));
    }

    #region Command

    public static RoutedCommand? PrevCommand { get; private set; } = null;
    public static RoutedCommand? NextCommand { get; private set; } = null;

    private static void InitializeCommands()
    {
        PrevCommand = new RoutedCommand("Prev", _typeofSelf);
        NextCommand = new RoutedCommand("Next", _typeofSelf);

        CommandManager.RegisterClassCommandBinding(_typeofSelf, new CommandBinding(PrevCommand, OnPrevCommand, OnCanPrevCommand));
        CommandManager.RegisterClassCommandBinding(_typeofSelf, new CommandBinding(NextCommand, OnNextCommand, OnCanNextCommand));
    }

    private static void OnPrevCommand(object sender, RoutedEventArgs e)
    {
        if (sender is SimplePagination ctrl)
            ctrl.Current--;
    }

    private static void OnCanPrevCommand(object sender, CanExecuteRoutedEventArgs e)
    {
        if (sender is SimplePagination ctrl)
            e.CanExecute = ctrl.Current > 1;
    }

    private static void OnNextCommand(object sender, RoutedEventArgs e)
    {
        if (sender is SimplePagination ctrl)
            ctrl.Current++;
    }

    private static void OnCanNextCommand(object sender, CanExecuteRoutedEventArgs e)
    {
        if (sender is SimplePagination ctrl)
            e.CanExecute = ctrl.Current < ctrl.PageCount;
    }

    #endregion



    #region Properties

    // 总页数（只读）
    private static readonly DependencyPropertyKey PageCountPropertyKey =
       DependencyProperty.RegisterReadOnly("PageCount", typeof(int), _typeofSelf,
           new PropertyMetadata(1, OnPageCountPropertyChanged));
    public static readonly DependencyProperty PageCountProperty = PageCountPropertyKey.DependencyProperty;
    public int PageCount
    {
        get { return (int)GetValue(PageCountProperty); }
    }

    private static void OnPageCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SimplePagination ctrl)
        {
            var pageCount = (int)e.NewValue;

            if (d is FrameworkElement fe)
            {
                if ($"{fe.Tag}" == "MainWindow")
                {
                    // 当总页数减少时，自动切换到上一页
                    if (ctrl.Current > pageCount)
                    {
                        ctrl.Current = pageCount;
                    }

                    if (ctrl.Current < pageCount)
                    {
                        ctrl.Current = pageCount;
                    }
                }
            }
        }
    }


    // 总条数
    public static readonly DependencyProperty CountProperty = DependencyProperty.Register("Count", typeof(int), _typeofSelf,
        new PropertyMetadata(0, OnCountPropertyChanged, CoerceCount));
    public int Count
    {
        get { return (int)GetValue(CountProperty); }
        set { SetValue(CountProperty, value); }
    }

    private static void OnCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = d as SimplePagination;
        var count = (int)e.NewValue;

        ctrl?.SetValue(PageCountPropertyKey, (int)Math.Ceiling(count * 1.0 / ctrl.CountPerPage));
    }

    private static object CoerceCount(DependencyObject d, object value)
    {
        var count = (int)value;
        return Math.Max(count, 0);
    }


    // 每页数量
    public static readonly DependencyProperty CountPerPageProperty =
        DependencyProperty.Register("CountPerPage", typeof(int), _typeofSelf,
            new PropertyMetadata(50, OnCountPerPagePropertyChanged, CoerceCountPerPage));
    public int CountPerPage
    {
        get { return (int)GetValue(CountPerPageProperty); }
        set { SetValue(CountPerPageProperty, value); }
    }

    private static object CoerceCountPerPage(DependencyObject d, object value)
    {
        var countPerPage = (int)value;
        return Math.Max(countPerPage, 1);
    }

    private static void OnCountPerPagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SimplePagination ctrl)
        {
            var countPerPage = (int)e.NewValue;

            ctrl.SetValue(PageCountPropertyKey, (int)Math.Ceiling(ctrl.Count * 1.0 / countPerPage));

            if (ctrl.Current != 1)
                ctrl.Current = 1;
        }
    }


    // 当前页
    public static readonly DependencyProperty CurrentProperty =
        DependencyProperty.Register("Current", typeof(int), _typeofSelf,
            new PropertyMetadata(1, OnCurrentPropertyChanged, CoerceCurrent));
    public int Current
    {
        get { return (int)GetValue(CurrentProperty); }
        set { SetValue(CurrentProperty, value); }
    }

    private static object CoerceCurrent(DependencyObject d, object value)
    {
        var current = (int)value;
        return Math.Max(current, 1);
    }

    private static void OnCurrentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SimplePagination ctrl)
        {
            var current = (int)e.NewValue;

        }
    }

    #endregion



    #region Override

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        UnsubscribeEvents();

        Init();

        SubscribeEvents();
    }

    #endregion



    #region Event

    /// <summary>
    /// 分页
    /// </summary>
    private void OnCountPerPageTextBoxChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is int selectedValue)
        {
            //CountPerPage = (int)e.NewValue;
            CountPerPage = selectedValue;
        }
    }

    /// <summary>
    /// 跳转页
    /// </summary>
    private void OnJumpPageTextBoxChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        Current = (int)e.NewValue;
    }

    #endregion



    #region Private

    private void Init()
    {
        SetValue(PageCountPropertyKey, (int)Math.Ceiling(Count * 1.0 / CountPerPage));

    }

    private void UnsubscribeEvents()
    {

    }

    private void SubscribeEvents()
    {
    }

    private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        e.Handled = true;
    }

    #endregion
}