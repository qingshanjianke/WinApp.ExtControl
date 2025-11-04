using System.Windows;
using WinApp.ExtControl.Helpers;

namespace WinApp.ExtControl.Controls;

public enum Position
{
    Left,
    Top,
    Right,
    Bottom
}

public static class Message
{
    private static MessageExt? _messageExt;
    private static MessageAdorner? _messageAdorner;
    private static Position _position = Position.Right;

    // 推送桌面消息
    public static void PushDesktop(string message, MessageBoxImage type = MessageBoxImage.Information, bool center = false)
    {
        // 如果窗口已关闭，重新创建窗口
        if (_messageExt == null)
        {
            _messageExt = new MessageExt();
            _messageExt.Closed += delegate { _messageExt = null; };
        }

        // 当窗口未显示时，显示窗口
        if (!_messageExt.IsVisible)
            _messageExt.Show();

        // 设置位置
        if (_messageExt.Position != _position)
        {
            _messageExt.IsPosition = false;
            _messageExt.Position = _position;
        }

        // 推送消息
        _messageExt.Push(message, type, center);
    }

    // 上一次打开的窗口实例
    private static Window? lastWindow = null;

    // 推送当前窗口消息
    public static void Push(Window window, string message, MessageBoxImage type = MessageBoxImage.Information, bool center = false)
    {
        if (!ReferenceEquals(lastWindow, window))
        {
            // 记录本次打开的窗口
            lastWindow = window;
            // 切换了窗口，清除之前窗口的消息
            _messageAdorner = null;
        }

        CreateMessageAdorner(window, message, type, center);
    }

    // 推送默认窗口消息
    public static void Push(string message, MessageBoxImage type = MessageBoxImage.Information, bool center = false)
    {
        CreateMessageAdorner(message: message, type: type, center: center);
    }

    // 创建消息装饰器
    private static void CreateMessageAdorner(Window? owner = null, string? message = null, MessageBoxImage type = MessageBoxImage.Information, bool center = false)
    {
        try
        {
            if (_messageAdorner != null)
            {
                // 如果装饰器的父元素不是当前窗口，重新创建装饰器
                if (_messageAdorner.Position != _position)
                    _messageAdorner.Position = _position;
                _messageAdorner.Push(message ?? string.Empty, type, center);
            }
            else
            {
                // 创建新的装饰器
                owner ??= ControlsHelper.GetDefaultWindow();
                var layer = ControlsHelper.GetAdornerLayer(owner) ?? throw new Exception("AdornerLayer is not empty, it is recommended to use PushDesktop");
                _messageAdorner = new MessageAdorner(layer);
                layer.Add(_messageAdorner);
                if (_messageAdorner.Position != _position)
                    _messageAdorner.Position = _position;
                if (!string.IsNullOrWhiteSpace(message))
                    _messageAdorner.Push(message, type, center);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    // 设置显示位置
    public static void SetPosition(Position position = Position.Top)
    {
        if (_position != position)
            _position = position;
    }

    // 清除当前窗口的消息
    public static void Clear()
    {
        _messageAdorner?.Clear();
    }

    // 清除桌面消息
    public static void ClearDesktop()
    {
        _messageExt?.Clear();
    }
}
