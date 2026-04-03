using ApiTestFramework.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace ApiTestFramework.UI.Infrastructure;

public static class GlobalExceptionHandler
{
    public static void Handle(Exception ex)
    {
        if (ex is BusinessException businessEx)
        {
            ShowBusinessError(businessEx);
        }
        else
        {
            Log(ex);
            ShowSystemError();
        }
    }

    private static void ShowBusinessError(BusinessException ex)
    {
        MessageBox.Show(ex.Message, "业务提示");
    }

    private static void ShowSystemError()
    {
        MessageBox.Show("系统异常，请联系管理员", "错误");
    }

    private static void Log(Exception ex)
    {
        Console.WriteLine(ex);
    }
}
