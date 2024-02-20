﻿using HandyControl.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SqlImport.Utils
{
    /// <summary>
    /// 增加Password扩展属性
    /// </summary>
    public static class PasswordBoxHelper
    {
        public static string GetPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordProperty, value);
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(name: "Password", propertyType: typeof(string), ownerType: typeof(PasswordBoxHelper), new PropertyMetadata(defaultValue: "", OnPasswordPropertyChanged));

        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox box = sender as PasswordBox;
            string password = (string)e.NewValue;
            if (box != null && box.Password != password)
            {
                box.Password = password;
            }
        }
    }

    /// <summary>
    /// 接收PasswordBox的密码修改事件
    /// </summary>
    public class PasswordBoxBehavior : Behavior<PasswordBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PasswordChanged += OnPasswordChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PasswordChanged -= OnPasswordChanged;
        }

        private static void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox box = sender as PasswordBox;
            string password = PasswordBoxHelper.GetPassword(box);
            if (box != null && box.Password != password)
            {
                PasswordBoxHelper.SetPassword(box, box.Password);
            }
        }
    }
}
